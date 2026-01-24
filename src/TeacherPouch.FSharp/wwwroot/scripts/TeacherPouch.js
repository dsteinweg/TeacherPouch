function PlayIntroVideo(placeholder) {
    var videoURL = $(placeholder).data("video");
    var video = "<div class='embed-responsive embed-responsive-4by3'>" +
        "<iframe width='420' height='315' src='" + videoURL + "' allowfullscreen></iframe>" +
        "</div>";
    $(placeholder).replaceWith(video);
}
$(function () {
    var searchBoxHasFocus = false;
    $("#q").on("click", function () {
        if (searchBoxHasFocus)
            return;
        searchBoxHasFocus = true;
        this.focus();
    }).on("blur", function () {
        searchBoxHasFocus = false;
    }).autocomplete({
        appendTo: "#suggest",
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
//# sourceMappingURL=TeacherPouch.js.map