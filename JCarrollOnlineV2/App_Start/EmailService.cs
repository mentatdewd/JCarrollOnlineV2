using Microsoft.AspNet.Identity;
using NLog;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace JCarrollOnlineV2
{
    public class MailService : IIdentityMessageService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        Task IIdentityMessageService.SendAsync(IdentityMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            string fromEmail = ConfigurationManager.AppSettings["SmtpFromEmail"];
            return SendAsync(fromEmail, message.Destination, message.Subject, message.Body);
        }

        public static async Task SendAsync(string fromString, string toString, string subjectString, string messageBody)
        {
            if (string.IsNullOrWhiteSpace(fromString))
            {
                throw new ArgumentException("From email address is required", nameof(fromString));
            }

            if (string.IsNullOrWhiteSpace(toString))
            {
                throw new ArgumentException("To email address is required", nameof(toString));
            }

            if (string.IsNullOrWhiteSpace(subjectString))
            {
                throw new ArgumentException("Subject is required", nameof(subjectString));
            }

            try
            {
                _logger.Info($"Sending email to {toString} with subject: {subjectString}");

                using (MailMessage msg = new MailMessage())
                {
                    msg.Subject = subjectString;
                    msg.From = new MailAddress(fromString);
                    msg.Body = messageBody;
                    msg.To.Add(new MailAddress(toString));
                    msg.IsBodyHtml = true;

                    using (SmtpClient smtp = CreateSmtpClient())
                    {
                        _logger.Info($"SMTP client created, attempting to send...");

                        // Add a timeout wrapper
                        Task sendTask = smtp.SendMailAsync(msg);
                        if (await Task.WhenAny(sendTask, Task.Delay(35000)) == sendTask)
                        {
                            await sendTask; // Propagate any exceptions
                            _logger.Info($"Email sent successfully to {toString}");
                        }
                        else
                        {
                            _logger.Error($"Email send timed out after 35 seconds to {toString}");
                            throw new TimeoutException("Email sending operation timed out");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to send email to {toString}");
                throw; // Re-throw to let ASP.NET Identity handle the error
            }
        }

        private static SmtpClient CreateSmtpClient()
        {
            //#if DEBUG
            //// DEVELOPMENT ONLY: Bypass SSL certificate validation
            //// Remove this in production once you have a valid SSL certificate
            //_logger.Warn("SSL certificate validation is bypassed in DEBUG mode");
            //System.Net.ServicePointManager.ServerCertificateValidationCallback =
            //    (sender, certificate, chain, sslPolicyErrors) => true;
            //#endif

            string host = ConfigurationManager.AppSettings["SmtpHost"];
            string port = ConfigurationManager.AppSettings["SmtpPort"];
            string username = ConfigurationManager.AppSettings["SmtpUsername"];
            string password = ConfigurationManager.AppSettings["SmtpPassword"];
            string enableSsl = ConfigurationManager.AppSettings["SmtpEnableSsl"];

            return string.IsNullOrWhiteSpace(host)
                ? throw new ConfigurationErrorsException("SmtpHost configuration is missing")
                : string.IsNullOrWhiteSpace(username)
                ? throw new ConfigurationErrorsException("SmtpUsername configuration is missing")
                : string.IsNullOrWhiteSpace(password)
                ? throw new ConfigurationErrorsException("SmtpPassword configuration is missing")
                : new SmtpClient(host, int.Parse(port))
            {
                Credentials = new NetworkCredential(username, password),
                UseDefaultCredentials = false,
                EnableSsl = bool.Parse(enableSsl),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000 // 30 seconds timeout
            };
        }
    }
}