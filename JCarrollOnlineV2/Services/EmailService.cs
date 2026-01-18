using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using JCarrollOnlineV2.ViewModels.Email;
using Microsoft.AspNet.Identity;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace JCarrollOnlineV2.Services
{
    public class EmailService : IEmailService, IIdentityMessageService
    {
        private readonly JCarrollOnlineV2DbContext _context;
        private readonly ILogger _logger;
        private readonly SmtpSettings _smtpSettings;

        public EmailService(JCarrollOnlineV2DbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            _smtpSettings = LoadSmtpSettings();
        }

        private SmtpSettings LoadSmtpSettings()
        {
            return new SmtpSettings
            {
                Host = ConfigurationManager.AppSettings["SmtpHost"],
                Port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587"),
                Username = ConfigurationManager.AppSettings["SmtpUsername"],
                Password = ConfigurationManager.AppSettings["SmtpPassword"],
                FromEmail = ConfigurationManager.AppSettings["SmtpFromEmail"],
                EnableSsl = bool.Parse(ConfigurationManager.AppSettings["SmtpEnableSsl"] ?? "true")
            };
        }

        public async Task<EmailResult> SendMassEmailAsync(string subject, string body, bool isHtml)
        {
            EmailResult result = new EmailResult();
            
            // Use batching to avoid loading all users into memory
            const int batchSize = 100;
            int skip = 0;
            bool hasMore = true;

            while (hasMore)
            {
                List<string> userBatch = await _context.ApplicationUser
                    .Where(u => !string.IsNullOrEmpty(u.Email))
                    .OrderBy(u => u.Id)
                    .Skip(skip)
                    .Take(batchSize)
                    .Select(u => u.Email)
                    .ToListAsync()
                    .ConfigureAwait(false);

                hasMore = userBatch.Count == batchSize;
                skip += batchSize;

                using (SmtpClient smtp = CreateSmtpClient())
                {
                    foreach (string email in userBatch)
                    {
                        try
                        {
                            //await SendEmailAsync().ConfigureAwait(false);
                            result.SuccessCount++;
                        }
                        catch (Exception ex)
                        {
                            result.FailureCount++;
                            result.FailedRecipients.Add(email);
                            _logger.Error(ex, $"Failed to send email to {email}");
                        }
                    }
                }
            }

            return result;
        }

        //public async Task<bool> SendEmailAsync(string toAddress, string subject, string body, bool isHtml)
        //{
        //    try
        //    {
        //        using (SmtpClient smtp = CreateSmtpClient())
        //        {
        //            await SendEmailAsync(smtp, toAddress, subject, body, isHtml).ConfigureAwait(false);
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex, $"Failed to send email to {toAddress}");
        //        return false;
        //    }
        //}

        //private async Task SendEmailViaHostGatorAsync(EmailViewModelBase emailViewModel)
        //{
        //    // Read SMTP settings from web.config/appSettings
        //    string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
        //    string smtpPortStr = ConfigurationManager.AppSettings["SmtpPort"];
        //    string smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
        //    string smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
        //    string fromEmail = ConfigurationManager.AppSettings["SmtpFromEmail"];
        //    string enableSslStr = ConfigurationManager.AppSettings["SmtpEnableSsl"];

        //    // Validate configuration
        //    if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpPassword))
        //    {
        //        _logger.Error("SMTP configuration is incomplete. Check Web.config appSettings.");
        //        throw new InvalidOperationException("SMTP configuration is missing required values.");
        //    }

        //    int smtpPort = int.Parse(smtpPortStr);
        //    bool enableSsl = bool.Parse(enableSslStr);

        //    // Configure certificate validation callback before creating SMTP client
        //    System.Net.ServicePointManager.ServerCertificateValidationCallback =
        //        (sender, certificate, chain, sslPolicyErrors) =>
        //        {
        //            // If there are no SSL policy errors, accept the certificate
        //            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
        //            {
        //                return true;
        //            }

        //            // Only bypass validation for our specific SMTP server
        //            if (sender is SmtpClient)
        //            {
        //                // Accept certificate from our known HostGator mail server despite errors
        //                _logger.Warn(string.Format(CultureInfo.InvariantCulture,
        //                    "Accepting certificate from {0} despite SSL errors: {1}",
        //                    smtpHost, sslPolicyErrors));
        //                return true;
        //            }

        //            // Reject all other certificates with errors
        //            _logger.Error(string.Format(CultureInfo.InvariantCulture,
        //                "Certificate validation failed for unknown sender. SSL errors: {0}",
        //                sslPolicyErrors));
        //            return false;
        //        };

        //    try
        //    {
        //        using (MailMessage mailMessage = new MailMessage())
        //        {
        //            mailMessage.From = new MailAddress(fromEmail, "JCarrollOnline");
        //            mailMessage.To.Add(new MailAddress(emailViewModel?.TargetUser.Email));
        //            mailMessage.Subject = "Welcome to JCarrollOnline";
        //            mailMessage.Body = emailViewModel.Body;
        //            mailMessage.IsBodyHtml = true;

        //            using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
        //            {
        //                smtpClient.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
        //                smtpClient.EnableSsl = enableSsl;
        //                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

        //                await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
        //                _logger.Info(string.Format(CultureInfo.InvariantCulture,
        //                    "Welcome email sent successfully to {0}",
        //                    emailViewModel.TargetUser.Email));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex, string.Format(CultureInfo.InvariantCulture,
        //            "Failed to send welcome email to {0}",
        //            emailViewModel.TargetUser.Email));
        //        throw;
        //    }
        //    finally
        //    {
        //        // Reset certificate validation to default for security
        //        System.Net.ServicePointManager.ServerCertificateValidationCallback = null;
        //    }
        //}

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new System.Net.NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
            };
        }

        public async Task SendAsync(IdentityMessage identityMessage)
        {
            // Read SMTP settings from web.config/appSettings
            string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            string smtpPortStr = ConfigurationManager.AppSettings["SmtpPort"];
            string smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
            string smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
            string fromEmail = ConfigurationManager.AppSettings["SmtpFromEmail"];
            string enableSslStr = ConfigurationManager.AppSettings["SmtpEnableSsl"];

            // Validate configuration
            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpPassword))
            {
                _logger.Error("SMTP configuration is incomplete. Check Web.config appSettings.");
                throw new InvalidOperationException("SMTP configuration is missing required values.");
            }

            int smtpPort = int.Parse(smtpPortStr);
            bool enableSsl = bool.Parse(enableSslStr);

            // Configure certificate validation callback before creating SMTP client
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) =>
                {
                    // If there are no SSL policy errors, accept the certificate
                    if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                    {
                        return true;
                    }

                    // Only bypass validation for our specific SMTP server
                    if (sender is SmtpClient)
                    {
                        // Accept certificate from our known HostGator mail server despite errors
                        _logger.Warn(string.Format(CultureInfo.InvariantCulture,
                            "Accepting certificate from {0} despite SSL errors: {1}",
                            smtpHost, sslPolicyErrors));
                        return true;
                    }

                    // Reject all other certificates with errors
                    _logger.Error(string.Format(CultureInfo.InvariantCulture,
                        "Certificate validation failed for unknown sender. SSL errors: {0}",
                        sslPolicyErrors));
                    return false;
                };

            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(fromEmail, "JCarrollOnline");
                    mailMessage.To.Add(new MailAddress(identityMessage?.Destination));
                    mailMessage.Subject = "Welcome to JCarrollOnline";
                    mailMessage.Body = BuildHtmlEmail(identityMessage.Body);
                    mailMessage.IsBodyHtml = true;

                    using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
                    {
                        smtpClient.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
                        smtpClient.EnableSsl = enableSsl;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                        await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
                        _logger.Info(string.Format(CultureInfo.InvariantCulture,
                            "Welcome email sent successfully to {0}",
                            identityMessage.Destination));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, string.Format(CultureInfo.InvariantCulture,
                    "Failed to send welcome email to {0}",
                    identityMessage.Destination));
                throw;
            }
            finally
            {
                // Reset certificate validation to default for security
                System.Net.ServicePointManager.ServerCertificateValidationCallback = null;
            }
            //using (MailMessage message = new MailMessage
            //{
            //    From = new MailAddress(_smtpSettings.FromEmail, "Administrator - JCarrollOnline"),
            //    Subject = subject,
            //    Body = isHtml ? BuildHtmlEmail(body) : body,
            //    IsBodyHtml = isHtml
            //})
            //{
            //    message.To.Add(toAddress);
            //    await smtp.SendMailAsync(message).ConfigureAwait(false);
            //}
        }

        private string BuildHtmlEmail(string body)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }");
            sb.AppendLine(".header { background-color: #4CAF50; color: white; padding: 20px; text-align: center; }");
            sb.AppendLine(".content { padding: 20px; }");
            sb.AppendLine(".footer { background-color: #f1f1f1; padding: 10px; text-align: center; font-size: 12px; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class='header'>");
            sb.AppendLine("<h1>JCarrollOnline</h1>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='content'>");
            sb.AppendLine(body);
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='footer'>");
            sb.AppendLine("<p>This message was sent by the Administrator of JCarrollOnline</p>");
            sb.AppendLine($"<p>&copy; {DateTime.Now.Year} JCarrollOnline. All rights reserved.</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }

        private class SmtpSettings
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string FromEmail { get; set; }
            public bool EnableSsl { get; set; }
        }
    }
}