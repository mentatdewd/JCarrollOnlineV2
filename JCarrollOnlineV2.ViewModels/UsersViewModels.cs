using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Display(Name="MicroPost Email Notifications")]
        public bool MicroPostEmailNotifications { get; set; }

        [Display(Name="MicroPost SMS Notifications")]
        public bool MicroPostSMSNotifications { get; set; }

        public int? MicroPostsAuthored { get; set; }
        public List<MicroPostFeedItemViewModel> MicroPosts { get; set; }
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
