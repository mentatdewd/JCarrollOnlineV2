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

function textCounter(field, field2, maxlimit) {
    var countfield = document.getElementById(field2);
    if (field.value.length > maxlimit) {
        field.value = field.value.substring(0, maxlimit);
        return false;
    } else {
        countfield.value = maxlimit - field.value.length;
    }
    if (countfield.value == 140) {
        document.getElementById("submitbutton").disabled = true;
    }
    else {
        document.getElementById("submitbutton").disabled = false;
    }
}
