
function PlayIntroVideo(placeholder) {
    var videoURL = $(placeholder).data("video");

    var video =
        "<div class='embed-responsive embed-responsive-4by3'>" +
            "<iframe width='420' height='315' src='" + videoURL + "' allowfullscreen></iframe>" +
        "</div>";

    $(placeholder).replaceWith(video);
}


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
