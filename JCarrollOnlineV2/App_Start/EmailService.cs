using Microsoft.AspNet.Identity;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace JCarrollOnlineV2
{
    public class MailService : IIdentityMessageService
    {
        Task IIdentityMessageService.SendAsync(IdentityMessage message)
        {
            return SendAsync("admin@JCarrollOnline.com", message.Destination, message.Subject, message.Body);
        }

        public static async Task SendAsync(string fromString, string toString, string subjectString, string message)
        {
            using (MailMessage msg = new MailMessage())
            {
                msg.Subject = subjectString;
                msg.From = new MailAddress(fromString);
                msg.Body = message;
                msg.To.Add(new MailAddress(toString));
                msg.IsBodyHtml = true;

                //var plainTextContent = "and easy to do anywhere, even with C#";
                using (SmtpClient smtp = new SmtpClient())
                {
                    NetworkCredential credential = new NetworkCredential
                    {
                        UserName = "admin@jcarrollonline.com",  // replace with valid value
                        Password = "jV41wt^5"  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.UseDefaultCredentials = false;
                    smtp.Host = "mail.jcarrollonline.com";
                    smtp.Port = 587;
                    await smtp.SendMailAsync(msg).ConfigureAwait(false);
                }
            }
        }
    }
}