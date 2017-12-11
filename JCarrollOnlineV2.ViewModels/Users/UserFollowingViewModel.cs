using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Users
{
    public class UserFollowingViewModel : UserViewModelBase
    {
        public UserFollowingViewModel()
        {
            Users = new List<UserItemViewModel>();
        }
        public List<UserItemViewModel> Users { get; set; }
    }
}
