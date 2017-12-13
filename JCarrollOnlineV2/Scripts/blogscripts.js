$(document).ready(function () {
    $('.blogComments').hide();
    $(document).on('mouseenter', '.showCommentsButton', function () {
        var commentList = event.target.id;
        var cmtList = event.target.getAttribute("data-BlogItemId");
        $("#" + cmtList).toggle(400);
        return false;
    });
    $(document).on('mouseleave', '.blogEntryContainer', function () {
        var commentList = event.target.id;
        var cmtList = event.target.getAttribute("data-BlogItemId");
        $("#" + cmtList).toggle(400);
    });
});
