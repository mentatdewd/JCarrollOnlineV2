using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly JCarrollOnlineV2DbContext _context;

        public ChatHub()
        {
            _context = new JCarrollOnlineV2DbContext();
        }

        public async Task SendMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message) || message.Length > 500)
            {
                return;
            }

            string userId = Context.User.Identity.GetUserId();
            ApplicationUser user = await _context.ApplicationUser.FindAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                return;
            }

            ChatMessage chatMessage = new ChatMessage
            {
                Message = message,
                AuthorId = userId,
                Author = user,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // Broadcast to all connected clients
            await Clients.All.receiveMessage(
                user.UserName,
                message,
                chatMessage.CreatedAt.ToUniversalTime().ToString("o")
            ).ConfigureAwait(false);
        }

        public override Task OnConnected()
        {
            string userName = Context.User.Identity.Name;
            Clients.All.userConnected(userName);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = Context.User.Identity.Name;
            Clients.All.userDisconnected(userName);
            return base.OnDisconnected(stopCalled);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}