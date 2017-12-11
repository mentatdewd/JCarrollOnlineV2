namespace JCarrollOnlineV2.ViewModels.Users
{
    public class UserViewModelBase : ViewModelBase
    {
        public UserViewModelBase()
        {
            User = new ApplicationUserViewModel();
        }
        public ApplicationUserViewModel User { get; set; }
    }
}
