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
