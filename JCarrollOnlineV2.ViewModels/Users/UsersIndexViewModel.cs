using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Users
{
    public class UsersIndexViewModel : UserViewModelBase
    {
        public UsersIndexViewModel()
        {
            Users = new List<UserItemViewModel>();
        }
        public List<UserItemViewModel> Users { get; set; }
    }
}
