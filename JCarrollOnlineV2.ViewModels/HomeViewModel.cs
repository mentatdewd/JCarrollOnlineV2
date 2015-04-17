using JCarrollOnlineV2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public MicropostCreateViewModel MicropostCreateVM { get; set; }
        public MicropostFeedViewModel MicropostFeedVM { get; set; }
        public UserStatsViewModel UserStatsVM { get; set; }
        public UserInfoViewModel UserInfoVM { get; set; }
        public RssFeedViewModel RssFeedVM { get; set; }
    }
}
