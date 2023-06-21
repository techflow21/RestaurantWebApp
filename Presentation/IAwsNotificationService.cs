
namespace Presentation
{
    public interface IAwsNotificationService
    {
        //Task SendTextMessageAsync(string phoneNumber, string message);
        Task<bool> SendSmsAsync(string phoneNumber, string textMessage);
    }
}
