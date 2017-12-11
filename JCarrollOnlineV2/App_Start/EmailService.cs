using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.Threading.Tasks;

namespace JCarrollOnlineV2
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            var mailMessage = new MailMessage("jcarrollonline@gmail.com", message.Destination, message.Subject, message.Body);
            mailMessage.IsBodyHtml = true;

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
        }

    }
}