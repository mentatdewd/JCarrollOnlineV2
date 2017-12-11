namespace JCarrollOnlineV2.ViewModels.Users
{
    public class UserStatsViewModel : UserViewModelBase
    {
        public UserStatsViewModel()
        {
            UserFollowers = new UserFollowersViewModel();
            UsersFollowing = new UserFollowingViewModel();
        }
        public UserFollowersViewModel UserFollowers { get; set; }
        public UserFollowingViewModel UsersFollowing { get; set; }
    }
}
