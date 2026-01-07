using JCarrollOnlineV2.Entities;

namespace JCarrollOnlineV2.EmailViewModels
{
    public class EmailViewModelBase
    {
        public ApplicationUser TargetUser { get; set; }
        public string Content { get; set; }
    }
}
