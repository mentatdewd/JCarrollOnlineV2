function InitializeBlogComments() {
    $('.blogComments').hide();
    $(document).on('mouseenter', '.showCommentsButton', function () {
        var commentList = event.target.id;
        var cmtList = event.target.getAttribute("data-BlogItemId");
        var commentTarget = "#" + cmtList;
        $("#" + cmtList).show(400);
    });
    $(document).on('mouseleave', '.blogEntryContainer', function () {
        $('.blogComments').hide(400);
        var commentList = event.target.id;
        var cmtList = event.target.getAttribute("data-BlogItemId");
        $("#" + cmtList).hide(400);
    });
    $(".ShowCommentsDialogButton").click(function (event) {
        var data = event.target.id;
        var blogItemId = event.target.getAttribute("data-BlogItemId");
        ShowMyDialog(blogItemId);
        $(this).find("[type=submit]").hide();
    });
}

function ShowMyDialog(item) {
    var targetForm = "#commentForm" + item;

    $("#commentForm" + item).dialog({
        width: 450,
        open: function (event, ui) {
            $(this).find("[type=submit]").hide();
        },

        buttons:
        [
            {
                text: "Add a Comment",
                click: function () {
                    var mydata = $("#commentForm" + item).serialize();

                    if ($("#commentForm" + item).valid()) {
                        console.log("serialized commentForm: ", mydata);
                        $.ajax(
                            {
                                url: "../Blog/CreateComment",
                                type: "POST",
                                data: mydata,
                                success: function (data) {
                                    if (data.success) {
                                        $(item).load(item)
                                    }
                                }
                            }
                        );
                        $(this).dialog("close");
                    }
                }
            },
            {
                text: "Close",
                click: function () {
                    $(this).dialog("close");
                }
            }
        ]
    });
}

$(function () {
    //or to give the parser some context, supply it with a selector
    //jQuery validator will parse all child elements (deep) starting
    //from the selector element supplied
    jQuery.validator.unobtrusive.parse("#commentForm");
});

// Find all YouTube videos
var $allVideos = $("iframe[src^='https://www.youtube.com']"),
    // The element that is fluid width
    $fluidEl = $("div[class^='forum_thread_content']");

// Figure out and save aspect ratio for each video
$allVideos.each(function () {

    $(this)
        .data('aspectRatio', this.height / this.width)

        // and remove the hard coded width/height
        .removeAttr('height')
        .removeAttr('width');

});

// When the window is resized
$(window).resize(function () {

    var newWidth = $fluidEl.width();

    // Resize all videos according to their own aspect ratio
    $allVideos.each(function () {

        var $el = $(this);
        $el
            .width(newWidth)
            .height(newWidth * $el.data('aspectRatio'));

    });

    // Kick off one resize to fix all videos on page load
}).resize();

$(function () {
    $("textarea.mdd_editor").MarkdownDeep({
        help_location: "/Scripts/mdd_help.htm",
        HtmlClassTitledImages: "markdown_image"
    })
});

$('#postDownArrow').click(function () {
    $(this)
        .closest('tbody')
        .next('.section')
    $(".section").toggle(400);
    return false;
});

