using JCarrollOnlineV2.ViewModels.Email;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Services
{
    public interface IEmailService1
    {
        Task SendEmailViaHostGatorAsync(string toEmail, string content);
   }
}
