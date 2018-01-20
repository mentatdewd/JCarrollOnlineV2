using Microsoft.AspNet.Identity;
using RestSharp;
using RestSharp.Authenticators;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2
{
    public class MailService : IIdentityMessageService
    {
        // the domain name you have verified in your Mailgun account
        const string DOMAIN = "mail.JCarrollOnline.com";

        // your API Key used to send mail through the Mailgun API
        const string API_KEY = "key-c4bc39d9819a0b89e2c1e8f77b4ddd1c";

        Task IIdentityMessageService.SendAsync(IdentityMessage message)
        {
            return SendAsync("Excited User <mailgun@mail.JCarrollOnline.com>", message.Destination, message.Subject, message.Body);
        }

        public async Task SendAsync(string fromString, string toString, string subjectString, string message)
        {
            MailMessage msg = new MailMessage();
            msg.Subject = subjectString;
            msg.From = new MailAddress(fromString);
            msg.Body = message;
            msg.To.Add(new MailAddress(toString));

            //var plainTextContent = "and easy to do anywhere, even with C#";
            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                var credential = new NetworkCredential
                {
                    UserName = "",  // replace with valid value
                    Password = ""  // replace with valid value
                };
                smtp.Credentials = credential;
                await smtp.SendMailAsync(msg);
            }
        }
    }
}