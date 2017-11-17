(function () {

    var baseUri = "api/analyzer";

    $(document).ready(function () {
        $("#submit").on("click", postSources);
    });

    
    function postSources(e) {
        e.preventDefault();
        var files = document.getElementById('uploadFiles').files;
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    data.append("file" + x, files[x]);
                }

                $.ajax({
                    type: "POST",
                    url: baseUri,
                    contentType: false,
                    processData: false,
                    data: data,

                    statusCode: {
                        201: function (message) {
                            alert("Analyzed succesfully");
                        },
                        400: function () {
                            alert("Bad Request. Operation not executed");
                        }
                    },
                    success: function (data, textStatus, xhr) {
                        var result = $("#result");
                        result.empty();
                        result.append("<ol>" + data + "</ol>");
                    },
                    error: function (xhr, status, p3) {
                        alert(p3);
                    }
                });
            } else {
                alert("Brouser does not support upploading HTML5 files");
            }
        }
    }
    

    function getDiagnostic(diagnostic) {
        if (solutionLocation == "")
        {
            alert("Select and submit files first");
        }
        else
        {
            $.ajax({
                type: "GET",
                url: solutionLocation,

                success: function (data) {
                    $("#result").text(data);
                },

                error: function (xhr) {
                    if (xhr.status == "400") {
                        alert("Server error");
                        $("#result").text(xhr.responseText);
                    }
                }
            });
        }
    }


})();