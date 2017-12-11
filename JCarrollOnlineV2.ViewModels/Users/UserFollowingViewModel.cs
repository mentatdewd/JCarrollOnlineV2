using System.Collections.Generic;

namespace JCarrollOnlineV2.ViewModels.Users
{
    public class UserFollowingViewModel : UserViewModelBase
    {
        public UserFollowingViewModel()
        {
            Users = new List<UserItemViewModel>();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<UserItemViewModel> Users { get; set; }
    }
}
