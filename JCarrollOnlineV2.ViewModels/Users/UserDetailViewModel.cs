using NLog;

namespace JCarrollOnlineV2.ViewModels.Users
{
    public class UserDetailViewModel : UserViewModelBase
    {
        public UserDetailViewModel()
        {
            UserInfoViewModel = new UserItemViewModel();
            UserStatsViewModel = new UserStatsViewModel();
        }
        public UserItemViewModel UserInfoViewModel { get; set; }
        public UserStatsViewModel UserStatsViewModel { get; set; }
    }
}
