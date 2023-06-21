
namespace Presentation
{
    public interface INexmoSmsService
    {
        void SendSms(string phoneNumber, string textMessage);
    }
}
