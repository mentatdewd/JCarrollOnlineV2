using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.ViewModels
{
    public class UserViewModelBase : ViewModelBase
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class UserStatsViewModel : UserViewModelBase
    {
        public int Following { get; set; }
        public int Followed { get; set; }
    }

    public class UserInfoViewModel : UserViewModelBase
    {
        public int Microposts { get; set; }
    }

    public class UsersIndexViewModel : UserViewModelBase
    {
        public List<UserIndexItemViewModel> Users { get; set; }
    }

    public class UserIndexItemViewModel : UserViewModelBase
    {
        public bool Followed { get; set; }
    }

    public class UserDetailViewModel : UserViewModelBase
    {

    }

    public class UsersFollowingViewModel : UserViewModelBase
    {

    }
}
