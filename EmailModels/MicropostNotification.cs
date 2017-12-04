using JCarrollOnlineV2.ViewModels;

namespace JCarrollOnlineV2.EmailViewModels
{
    public class MicroPostNotificationEmailViewModel : EmailViewModelBase
    {
        public ApplicationUserViewModel MicroPostAuthor { get; set; }
        public string MicroPostContent { get; set; }
    }
}
