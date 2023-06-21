
using Microsoft.Extensions.Options;
using Persistence;
using Vonage;
using Vonage.Request;

namespace Presentation
{
    public class NexmoSmsService : INexmoSmsService
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        public NexmoSmsService(IOptions<NexmoConfiguration> nexmoConfig)
        {
            _apiKey = nexmoConfig.Value.ApiKey;
            _apiSecret = nexmoConfig.Value.ApiSecret;
        }


        public void SendSms(string phoneNumber, string textMessage)
        {
            var credentials = Credentials.FromApiKeyAndSecret(_apiKey, _apiSecret);

            var vonageClient = new VonageClient(credentials);

            vonageClient.SmsClient.SendAnSms(new Vonage.Messaging.SendSmsRequest()
            {
                To = $"{phoneNumber}",
                From = "SOB-Foods",
                Text = $"{textMessage}"
            });
        }
    }
}
