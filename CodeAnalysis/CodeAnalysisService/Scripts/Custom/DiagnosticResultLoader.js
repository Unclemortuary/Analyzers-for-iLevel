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
                    url: "Home/Upload",
                    contentType: false,
                    processData: false,
                    data: data,

                    statusCode: {
                        400: function (message) {
                            alert(message);
                        }
                    },
                    success: function (data, textStatus, xhr) {
                        var result = $("#names");
                        result.empty();
                        if (typeof data == 'string') {
                            result.append("<ol>" + data + "</ol>");
                        }
                        else {
                            for (var i = 0; i < data.length; i++) {
                                result.append("<ol>" + data[i] + "</ol>");
                            }
                        }

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
})();