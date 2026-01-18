using JCarrollOnlineV2.Entities;
using JCarrollOnlineV2.EntityFramework;
using Microsoft.AspNet.Identity;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Services
{
    public class EmailService : IEmailService
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
                            await SendEmailAsync(smtp, email, subject, body, isHtml).ConfigureAwait(false);
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

        public async Task<bool> SendEmailAsync(string toAddress, string subject, string body, bool isHtml)
        {
            try
            {
                using (SmtpClient smtp = CreateSmtpClient())
                {
                    await SendEmailAsync(smtp, toAddress, subject, body, isHtml).ConfigureAwait(false);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to send email to {toAddress}");
                return false;
            }
        }

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new System.Net.NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
            };
        }

        private async Task SendEmailAsync(SmtpClient smtp, string toAddress, string subject, string body, bool isHtml)
        {
            using (MailMessage message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail, "Administrator - JCarrollOnline"),
                Subject = subject,
                Body = isHtml ? BuildHtmlEmail(subject, body) : body,
                IsBodyHtml = isHtml
            })
            {
                message.To.Add(toAddress);
                await smtp.SendMailAsync(message).ConfigureAwait(false);
            }
        }

        private string BuildHtmlEmail(string subject, string body)
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

        public async Task SendAsync(IdentityMessage message)
        {
            await SendEmailAsync(message.Destination, message.Subject, message.Body, true);
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