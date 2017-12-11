using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Users
{
    public class UserFollowersViewModel : UserViewModelBase
    {
        public UserFollowersViewModel()
        {
            Users = new List<UserItemViewModel>();
        }
        public List<UserItemViewModel> Users { get; set; }
    }
}
