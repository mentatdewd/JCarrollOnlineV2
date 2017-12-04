
using System.Collections.Generic;
namespace JCarrollOnlineV2.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public MicroPostCreateViewModel MicroPostCreateVM { get; set; }
        public MicroPostFeedViewModel MicroPostFeedVM { get; set; }
        public UserStatsViewModel UserStatsVM { get; set; }
        public UserItemViewModel UserInfoVM { get; set; }
        public RssFeedViewModel RssFeedVM { get; set; }
        public int MicroPosts { get; set; }
        public BlogFeedViewModel BlogFeed { get; set; }

        public int? MicroPostPage { get; set; }
        public List<MicroPostFeedItemViewModel> OnePageOfMicroPosts { get; set; }
        public int? PageNumber { get; set; }
    }
}
