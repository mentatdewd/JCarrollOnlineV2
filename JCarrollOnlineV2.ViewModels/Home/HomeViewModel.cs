using JCarrollOnlineV2.ViewModels.Blog;
using JCarrollOnlineV2.ViewModels.Chat;
using JCarrollOnlineV2.ViewModels.MicroPosts;
using JCarrollOnlineV2.ViewModels.Rss;
using JCarrollOnlineV2.ViewModels.Users;
using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public MicroPostCreateViewModel MicroPostCreateViewModel { get; set; }
        public MicroPostFeedViewModel MicroPostFeedViewModel { get; set; }
        public UserStatsViewModel UserStatsViewModel { get; set; }
        public UserItemViewModel UserInfoViewModel { get; set; }
        public RssFeedViewModel RssFeedViewModel { get; set; }
        public int MicroPosts { get; set; }
        public BlogFeedViewModel BlogFeed { get; set; }
        public LatestForumThreadsViewModel LatestForumThreadsViewModel { get; set; }
        public ChatViewModel ChatViewModel { get; set; }
        public int? MicroPostPage { get; set; }
 //       public List<MicroPostFeedItemViewModel> OnePageOfMicroPosts { get; private set; }
        public int? PageNumber { get; set; }
    }
}
