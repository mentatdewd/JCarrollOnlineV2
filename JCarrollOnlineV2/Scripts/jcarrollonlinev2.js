$(function () {
    // Define the dialog
    $("#dialog-message").dialog({
        autoOpen: false,
        resizable: false,
        show: 'blind',
        hide: 'blind',
        width: 450,
        dialogClass: 'commentForm',
        open: function () {
            $(this).load($(this).data('param_1'));
        },
        
        buttons:
    [
    {
        text: "Add Comment",
        click: function () {
            if ($("#dialog-message").valid())
                $(this).dialog("close");
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
});
