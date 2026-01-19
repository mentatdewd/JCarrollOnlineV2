using JCarrollOnlineV2.ViewModels.Email;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Infrastructure
{
    public interface IEmailService1Wrapper
    {
        Task SendEmailViaHostGatorAsync(string toEmail, string content);
    }
}