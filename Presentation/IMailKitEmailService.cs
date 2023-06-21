using MimeKit;

namespace Presentation
{
    public interface IMailKitEmailService
    {
        Task SendEmailAsync(MimeMessage message);
    }
}
