using Microsoft.AspNetCore.Mvc;
using Presentation;

namespace RestaurantWebApp.API.Controllers
{
    [ApiController]
    [Route("api/subscription")]
    public class SubscriptionController : ControllerBase
    {
        IAwsSubscriptionService _subscriptionService;
        public SubscriptionController(IAwsSubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }


        [HttpPost("send-subscription-email")]
        public async Task<IActionResult> SendSubscriptionEmail([FromBody] string message)
        {
            await _subscriptionService.SendSubscriptionEmailAsync(message);
            return Ok("Email sent to all subscribed users");
        }


        [HttpPost("subscribe-user")]
        public async Task<IActionResult> SubscribeUser([FromBody] string emailAddress)
        {
            await _subscriptionService.SubscribeUserAsync(emailAddress);
            return Ok("User subscription successful.");
        }


        [HttpPost("unsubscribe-user")]
        public async Task<IActionResult> UnsubscribeUser(string emailAddress)
        {
            try
            {
                await _subscriptionService.UnsubscribeUserAsync(emailAddress);
                return Ok("User unsubscribed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to unsubscribe user.");
            }
        }


        [HttpGet("get-subscribed-users")]
        public async Task<ActionResult<List<string>>> GetSubscribedUsers()
        {
            var subscribedUsers = await _subscriptionService.GetSubscribedUsersAsync();
            return Ok(subscribedUsers);
        }
    }
}
