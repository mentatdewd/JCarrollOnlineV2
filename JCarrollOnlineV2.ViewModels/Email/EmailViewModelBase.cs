using JCarrollOnlineV2.Entities;
using Microsoft.AspNet.Identity;

namespace JCarrollOnlineV2.ViewModels.Email
{ 
    public class EmailViewModelBase : IdentityMessage, IEmailViewModel
    {
        public ApplicationUser TargetUser { get; set; }
        public string CallbackUrl { get; set; }
    }
}
