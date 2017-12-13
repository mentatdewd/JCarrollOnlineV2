function InitializeBlogComments() {
    $('#blogComments').hide();
    $(document).on('mouseenter', '.showCommentsButton', function () {
        var commentList = event.target.id;
        var cmtList = event.target.getAttribute("data-BlogItemId");
        $("#" + cmtList).show(400);
        return false;
    });
    $(document).on('mouseleave', '.blogEntryContainer', function () {
        $('.blogComments').hide(400);
        var commentList = event.target.id;
        var cmtList = event.target.getAttribute("data-BlogItemId");
        $("#" + cmtList).hide(400);
    });
    $(".ShowCommentsDialogButton").click(function (event) {
        var data = event.target.id;
        var blogItemId = event.target.getAttribute("data-BlogItemId")
        ShowMyDialog();
        $(this).find("[type=submit]").hide();
    });
}

function ShowMyDialog()
{
    $("#comment-dialog").dialog({
        width: 450,
        dialogClass: 'commentForm',
        open: function (event, ui) {
            $(this).find("[type=submit]").hide();
        },

        buttons:
        [
            {
                text: "Add a Comment or 2",
                click: function () {
                    if ($("#commentForm").valid()) {
                        var mydata = {
                            Author: $('.Author').val,
                            Content: $('.Content').val
                        }
                        $.ajax({
                            url: 'Blog/CreateComment',
                            type: 'POST',
                            data: mydata
                            });
                        $(this).dialog("close");
                    }
                },
                type: "submit",
                form: "commentForm" // <-- Make the association
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
