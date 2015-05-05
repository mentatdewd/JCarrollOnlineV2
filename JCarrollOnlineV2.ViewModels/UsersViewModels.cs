using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels
{
    public class UserViewModelBase : ViewModelBase
    {
        public UserViewModelBase()
        {
            User = new ApplicationUserViewModel();
        }
        public ApplicationUserViewModel User { get; set; }
    }

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

    public class UsersIndexViewModel : UserViewModelBase
    {
        public UsersIndexViewModel()
        {
            Users = new List<UserItemViewModel>();
        }
        public List<UserItemViewModel> Users { get; set; }
    }
    public class UserDetailViewModel : UserViewModelBase
    {
        public UserDetailViewModel()
        {
            UserInfoVM = new UserItemViewModel();
            UserStatsVM = new UserStatsViewModel();
        }
        public UserItemViewModel UserInfoVM { get; set; }
        public UserStatsViewModel UserStatsVM { get; set; }
    }

    public class UserItemViewModel : UserViewModelBase
    {
        public string UserId { get; set; }
        public bool MicropostEmailNotifications { get; set; }
        public bool MicropostSMSNotifications { get; set; }
        public int? MicropostsAuthored { get; set; }
        public List<MicropostFeedItemViewModel> Microposts { get; set; }
    }

    public class UserUnfollowViewModel : UserViewModelBase
    {
        public ApplicationUserViewModel UserUnfollow { get; set; }
    }
    public class UserFollowViewModel : UserViewModelBase
    {
        public ApplicationUserViewModel UserFollow { get; set; }
    }

    public class UserFollowingViewModel : UserViewModelBase
    {
        public UserFollowingViewModel()
        {
            Users = new List<UserItemViewModel>();
        }
        public List<UserItemViewModel> Users { get; set; }
    }
    public class UserFollowersViewModel : UserViewModelBase
    {
        public UserFollowersViewModel()
        {
            Users = new List<UserItemViewModel>();
        }
        public List<UserItemViewModel> Users { get; set; }
    }
}
