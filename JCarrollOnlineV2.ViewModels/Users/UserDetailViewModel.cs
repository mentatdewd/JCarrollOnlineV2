using NLog;

namespace JCarrollOnlineV2.ViewModels.Users
{
    public class UserDetailViewModel : UserViewModelBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public UserDetailViewModel()
        {
            UserInfoViewModel = new UserItemViewModel(logger);
            UserStatsViewModel = new UserStatsViewModel();
        }
        public UserItemViewModel UserInfoViewModel { get; set; }
        public UserStatsViewModel UserStatsViewModel { get; set; }
    }
}
