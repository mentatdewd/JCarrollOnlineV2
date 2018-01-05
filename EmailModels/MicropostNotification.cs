using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels.Users;

namespace JCarrollOnlineV2.EmailViewModels
{
    public class MicroPostNotificationEmailViewModel : EmailViewModelBase
    {
        public ApplicationUser MicroPostAuthor { get; set; }
        public string MicroPostContent { get; set; }
    }
}
