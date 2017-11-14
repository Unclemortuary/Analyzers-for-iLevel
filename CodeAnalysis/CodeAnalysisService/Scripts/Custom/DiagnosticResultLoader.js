(function () {
    var baseUri = "api/analyzer";

    function getTestDiagnostic() {
        $.ajax({
            url: baseUri,

            type: "GET",

            success: function () {
                var list = $("names");
                list.empty();
                list.append("<li>" + "test Message" + "</li>");
            }
        });

        getTestDiagnostic();
    }
})();