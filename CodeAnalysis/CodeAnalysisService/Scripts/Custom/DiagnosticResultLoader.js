(function () {

    $(document).ready(function () {
        $("#submit").on("click", postSources);
    });

    function asincPost(data) {
        return new Promise(function (resolve, reject) {
            $.ajax({
                type: "POST",
                url: "Home/Upload",
                contentType: false,
                processData: false,
                data: data,

                success: function (responce) {
                    resolve(responce);
                },
                error: function (text) {
                    reject(text);
                }
            });
        });
    }

    function formatResult(data) {
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
    }


    function postSources(e) {
        e.preventDefault();
        var files = document.getElementById('uploadFiles').files;
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    data.append("file" + x, files[x]);
                }
                asincPost(data).then(
                    result => { formatResult(result); },
                    error => { alert(error); }
                );
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