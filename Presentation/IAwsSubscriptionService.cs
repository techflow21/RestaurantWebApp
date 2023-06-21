namespace Presentation
{
    public interface IAwsSubscriptionService
    {
        Task SendSubscriptionEmailAsync(string message);
        Task SubscribeUserAsync(string emailAddress);
        Task<List<string>> GetSubscribedUsersAsync();
        Task UnsubscribeUserAsync(string emailAddress);
    }
}