using JCarrollOnlineV2.Entities;

namespace JCarrollOnlineV2.ViewModels.Email
{ 
    public class EmailViewModelBase
    {
        public ApplicationUser TargetUser { get; set; }
        public string Content { get; set; }
    }
}
