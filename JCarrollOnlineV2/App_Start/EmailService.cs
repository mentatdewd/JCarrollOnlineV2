using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.Threading.Tasks;

namespace JCarrollOnlineV2
{
    public class MailService : IIdentityMessageService
    {
        Task IIdentityMessageService.SendAsync(IdentityMessage message)
        {
            return SendAsync("Excited User <administrator@mail.JCarrollOnline.com>", message.Destination, message.Subject, message.Body);
        }

        public static async Task SendAsync(string fromString, string toString, string subjectString, string message)
        {
            //using (MailMessage msg = new MailMessage())
            //{
            //    msg.Subject = subjectString;
            //    msg.From = new MailAddress(fromString);
            //    msg.Body = message;
            //    msg.To.Add(new MailAddress(toString));

            //    //var plainTextContent = "and easy to do anywhere, even with C#";
            //    using (SmtpClient smtp = new SmtpClient())
            //    {
            //        smtp.Host = "mail.jcarrollonline.com";
            //        smtp.Port = 25;
            //        smtp.EnableSsl = false;
            //        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //        smtp.UseDefaultCredentials = true;
            //        //NetworkCredential credential = new NetworkCredential
            //        //{
            //        //    UserName = "",  // replace with valid value
            //        //    Password = ""  // replace with valid value
            //        //};
            //        //smtp.Credentials = credential;
            //        await smtp.SendMailAsync(msg).ConfigureAwait(false);
            //    }
            //}
        }
    }
}