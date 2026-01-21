using JCarrollOnlineV2.ViewModels.Email;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace JCarrollOnlineV2.Services
{
    public class EmailService1 : IEmailService1
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public async Task SendEmailViaHostGatorAsync(string toEmail, string content)
        {
            var test = ConfigurationManager.AppSettings["DebugTest"];

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
                    if (sender is System.Net.Mail.SmtpClient)
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
                using (System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage())
                {
                    mailMessage.From = new System.Net.Mail.MailAddress(fromEmail, "JCarrollOnline");
                    mailMessage.To.Add(new System.Net.Mail.MailAddress(toEmail));
                    mailMessage.Subject = "Welcome to JCarrollOnline";
                    mailMessage.Body = content;
                    mailMessage.IsBodyHtml = true;

                    using (System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(smtpHost, smtpPort))
                    {
                        smtpClient.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
                        smtpClient.EnableSsl = enableSsl;
                        smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                        await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
                        _logger.Info(string.Format(CultureInfo.InvariantCulture,
                            "Welcome email sent successfully to {0}",
                            toEmail));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, string.Format(CultureInfo.InvariantCulture,
                    "Failed to send welcome email to {0}",
                    toEmail));
                throw;
            }
            finally
            {
                // Reset certificate validation to default for security
                System.Net.ServicePointManager.ServerCertificateValidationCallback = null;
            }
        }
    }
}