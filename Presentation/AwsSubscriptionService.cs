using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;

namespace Presentation
{
    public class AwsSubscriptionService : IAwsSubscriptionService
    {
        private readonly AmazonSimpleNotificationServiceClient _snsClient;
        private readonly string _topicArn;

        public AwsSubscriptionService(IConfiguration configuration)
        {
            var awsRegion = configuration["AWS:SNS:Region"];
            var snsTopicArn = configuration["AWS:SNS:TopicArn"];

            _snsClient = new AmazonSimpleNotificationServiceClient(RegionEndpoint.GetBySystemName(awsRegion));
            _topicArn = snsTopicArn;
        }


        public async Task SendSubscriptionEmailAsync(string message)
        {
            var request = new PublishRequest
            {
                TopicArn = _topicArn,
                Message = message
            };

            var response = await _snsClient.PublishAsync(request);
        }


        public async Task SubscribeUserAsync(string emailAddress)
        {
            var subscribeRequest = new SubscribeRequest
            {
                TopicArn = _topicArn,
                Protocol = "email",
                Endpoint = emailAddress
            };

            await _snsClient.SubscribeAsync(subscribeRequest);
        }


        public async Task UnsubscribeUserAsync(string emailAddress)
        {
            var listRequest = new ListSubscriptionsByTopicRequest
            {
                TopicArn = _topicArn
            };
            var response = await _snsClient.ListSubscriptionsByTopicAsync(listRequest);

            var subscription = response.Subscriptions.FirstOrDefault(s => s.Protocol == "email" && s.Endpoint == emailAddress);
            if (subscription != null)
            {
                var unsubscribeRequest = new UnsubscribeRequest
                {
                    SubscriptionArn = subscription.SubscriptionArn
                };
                await _snsClient.UnsubscribeAsync(unsubscribeRequest);
            }
        }


        public async Task<List<string>> GetSubscribedUsersAsync()
        {
            var listRequest = new ListSubscriptionsByTopicRequest
            {
                TopicArn = _topicArn
            };

            var response = await _snsClient.ListSubscriptionsByTopicAsync(listRequest);

            var subscribedUsers = response.Subscriptions
                .Where(s => s.Protocol == "email")
                .Select(s => s.Endpoint)
                .ToList();

            return subscribedUsers;
        }
    }
}
