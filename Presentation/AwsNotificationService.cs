
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Tls;
using Persistence;

namespace Presentation
{
    public class AwsNotificationService : IAwsNotificationService
    {
        /*private readonly string _awsKeyId;
        private readonly string _awsKeySecret;
        private readonly RegionEndpoint _regionEndpoint;*/

        private readonly IAmazonSimpleNotificationService _snsClient;

        //public AwsNotificationService(IOptions<SNSConfiguration> awsConfiguration)
        public AwsNotificationService(IAmazonSimpleNotificationService snsClient)
        {
           /* _awsKeyId = awsConfiguration.Value.AwsKeyId;
            _awsKeySecret = awsConfiguration.Value.AwsKeySecret;
            _regionEndpoint = awsConfiguration.Value.RegionEndpoint;*/

            _snsClient = snsClient;

        }

        /*public async Task SendTextMessageAsync(string phoneNumber, string message)
        {
            var awsCredentials = new BasicAWSCredentials(_awsKeyId, _awsKeySecret);
            var snsClient = new AmazonSimpleNotificationServiceClient(awsCredentials, _regionEndpoint);

            var request = new PublishRequest
            {
                PhoneNumber = phoneNumber,
                Message = message
            };

            var response = await snsClient.PublishAsync(request);
        }*/


        public async Task<bool> SendSmsAsync(string phoneNumber, string textMessage)
        {
            var request = new PublishRequest
            {
                Message = textMessage,
                PhoneNumber = phoneNumber
            };

            var response = await _snsClient.PublishAsync(request);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }

}
