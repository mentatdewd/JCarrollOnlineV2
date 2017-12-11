using JCarrollOnlineV2.ViewModels.Users;
using System;

namespace JCarrollOnlineV2.ViewModels.MicroPosts
{
    public class MicroPostFeedItemViewModel : MicroPostViewModelBase
    {
        public MicroPostFeedItemViewModel()
        {
            Author = new ApplicationUserViewModel();
        }
        public ApplicationUserViewModel Author { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; }
    }
}
