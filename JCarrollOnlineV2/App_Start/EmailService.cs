using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.Threading.Tasks;

namespace JCarrollOnlineV2
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            var mailMessage = new MailMessage("jcarrollonline@gmail.com", message.Destination, message.Subject, message.Body)
            {
                IsBodyHtml = true
            };

            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch(SmtpException smtpException)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to send email. Exception: {0}", smtpException);
                }
            }
        }

    }
}