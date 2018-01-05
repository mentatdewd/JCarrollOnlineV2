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

        public async Task<bool> SendAsync(string fromString, string toString, string subjectString, string message)
        {
            var apiKey = "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";//Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("administrator@mail.JCarrollOnline.com", "John");
            var subject = subjectString;
            var to = new EmailAddress(toString.Split(' ')[1], toString.Split(' ')[0]);
            //var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = message;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Debug.WriteLine("Success");
                return true;
            }
            else
            {
                Debug.WriteLine("StatusCode: " + response.StatusCode);
                return false;
            }
        }
    }
}