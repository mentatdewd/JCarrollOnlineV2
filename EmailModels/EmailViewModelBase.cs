using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.ViewModels.Users;

namespace JCarrollOnlineV2.EmailViewModels
{
    public class EmailViewModelBase
    {
        public ApplicationUser TargetUser { get; set; }
        public string Content { get; set; }
    }
}
