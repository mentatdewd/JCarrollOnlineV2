using JCarrollOnlineV2.Entities;
using Microsoft.AspNet.Identity;

namespace JCarrollOnlineV2.ViewModels.Email
{
    public interface IEmailViewModel
    {
        ApplicationUser TargetUser { get; set; }
        string CallbackUrl { get; set; }
        string Destination { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
    }
}