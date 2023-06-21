using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Persistence;

namespace Presentation
{
    public class MailKitEmailService : IMailKitEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;

        public MailKitEmailService(IOptions<EmailConfiguration> emailConfiguration)
        {
            _smtpHost = emailConfiguration.Value.SmtpHost;
            _smtpPort = emailConfiguration.Value.SmtpPort;
            _smtpUsername = emailConfiguration.Value.SmtpUsername;
            _smtpPassword = emailConfiguration.Value.SmtpPassword;
        }

        public async Task SendEmailAsync(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpHost, _smtpPort, false);
                await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
