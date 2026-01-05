using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Chat
{
    public class ChatViewModel
    {
        public ChatViewModel()
        {
            RecentMessages = new List<ChatMessageViewModel>();
        }

        public List<ChatMessageViewModel> RecentMessages { get; set; }
    }

    public class ChatMessageViewModel
    {
        public string UserName { get; set; }
        public string Message { get; set; }
        public string TimeAgo { get; set; }
    }
}