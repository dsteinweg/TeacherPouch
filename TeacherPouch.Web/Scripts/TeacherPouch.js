

$(document).ready(function () {
    var searchBoxHasFocus = false;

    $("#q").click(function () {
        if (searchBoxHasFocus)
            return;

        searchBoxHasFocus = true;

        this.select();
    }).blur(function () {
        searchBoxHasFocus = false;
    }).autocomplete({
        appendTo: "#suggest",
        messages: {
            noResults: '',
            results: function() {}
        },
        source: function (request, response) {
            $.ajax({
                url: "/api/tags?q=" + request.term.toString(),
                success: function (data) {
                    response(data);
                }
            });
        }
    });
});
