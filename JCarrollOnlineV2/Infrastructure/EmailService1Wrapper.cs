using JCarrollOnlineV2.Services;
using JCarrollOnlineV2.ViewModels.Email;
using System;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.Infrastructure
{
    public class EmailService1Wrapper : IEmailService1Wrapper
    {
        private readonly EmailService1 _emailService1;

        public EmailService1Wrapper(EmailService1 emailService1)
        {
            _emailService1 = emailService1 ?? throw new ArgumentNullException(nameof(emailService1));
        }

        public async Task SendEmailViaHostGatorAsync(string toEmail, string content)
        {
            await _emailService1.SendEmailViaHostGatorAsync(toEmail, content).ConfigureAwait(false);
        }
    }
}
