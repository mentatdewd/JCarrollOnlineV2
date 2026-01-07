using JCarrollOnlineV2.Entities;

namespace JCarrollOnlineV2.EmailViewModels
{
    public class MicroPostNotificationViewModel : EmailViewModelBase
    {
        public ApplicationUser MicroPostAuthor { get; set; }
        public string MicroPostContent { get; set; }
    }
}
