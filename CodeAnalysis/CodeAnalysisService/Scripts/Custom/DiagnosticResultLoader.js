(function () {

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
                    url: 'home/upload',
                    contentType: false,
                    processData: false,
                    data: data,

                    statusCode: {
                        400: function (message) {
                            alert(message);
                        }
                    },
                    success: function (data, textStatus, xhr) {
                        var result = $(".results");
                        result.empty();
                        if (typeof data == 'string') {
                            result.append('<div class="textResult">' + data + '</div>');
                        }
                        else {
                            for (var i = 0; i < data.length; i++) {
                                result.append('<div class="textResult">' + data[i] + '</div>');
                            }
                        }

                    },
                    error: function (xhr, status, p3) {
                        alert(p3);
                    }
                });
            } else {
                alert("Browser does not support upploading HTML5 files");
            }
        }
        else
        {
            alert("Choose files first!");
        }
    }
})();