using JCarrollOnlineV2.ViewModels.Email;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Services
{
    public interface IEmailService
    {
        Task<EmailResult> SendMassEmailAsync(string subject, string body, bool isHtml);
        Task SendAsync(IdentityMessage identityMessage);
    }

    public class EmailResult
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> FailedRecipients { get; set; } = new List<string>();
    }
}