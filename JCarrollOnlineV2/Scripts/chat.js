$(function () {
    // Reference the auto-generated proxy for the hub
    var chat = $.connection.chatHub;

    // Create a function that the hub can call to broadcast messages
    chat.client.receiveMessage = function (userName, message, timestamp) {
        // Add the message to the page
        var $messageDiv = $('<div class="chat-message">')
            .append($('<strong>').text(userName + ': '))
            .append($('<span>').text(message))
            .append(' ')
            .append($('<small class="text-muted">').attr('data-livestamp', timestamp));

        $('#chatMessages').append($messageDiv);
        
        // Scroll to bottom
        var chatMessages = document.getElementById('chatMessages');
        chatMessages.scrollTop = chatMessages.scrollHeight;
        
        // Activate livestamp for new message
        $('[data-livestamp]').livestamp();
    };

    // Notify when users connect/disconnect
    chat.client.userConnected = function (userName) {
        var $notification = $('<div class="text-success small">')
            .text(userName + ' joined the chat')
            .fadeIn();
        $('#chatMessages').append($notification);
        setTimeout(function () {
            $notification.fadeOut(function () {
                $(this).remove();
            });
        }, 3000);
    };

    chat.client.userDisconnected = function (userName) {
        var $notification = $('<div class="text-muted small">')
            .text(userName + ' left the chat')
            .fadeIn();
        $('#chatMessages').append($notification);
        setTimeout(function () {
            $notification.fadeOut(function () {
                $(this).remove();
            });
        }, 3000);
    };

    // Start the connection
    $.connection.hub.start().done(function () {
        $('#sendButton').click(function () {
            var message = $('#chatInput').val();
            if (message) {
                chat.server.sendMessage(message);
                $('#chatInput').val('').focus();
            }
        });

        // Send on Enter key
        $('#chatInput').keypress(function (e) {
            if (e.which === 13) {
                $('#sendButton').click();
                return false;
            }
        });
    });
});