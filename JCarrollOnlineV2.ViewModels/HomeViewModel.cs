
using System.Collections.Generic;
namespace JCarrollOnlineV2.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public MicropostCreateViewModel MicropostCreateVM { get; set; }
        public MicropostFeedViewModel MicropostFeedVM { get; set; }
        public UserStatsViewModel UserStatsVM { get; set; }
        public UserItemViewModel UserInfoVM { get; set; }
        public RssFeedViewModel RssFeedVM { get; set; }
        public int Microposts { get; set; }

        public int? MicropostPage { get; set; }
        public List<MicropostFeedItemViewModel> OnePageOfMicroposts { get; set; }
        public int? PageNumber { get; set; }
    }
}
