using Microsoft.AspNetCore.Mvc;

namespace RestaurantWebApp.API.Controllers
{
    public class ProfileController : ControllerBase
    {
        public IActionResult SuperAdmin()
        {
            return Ok("This is SuperAdmin");
        }


        public IActionResult Admin()
        {
            return Ok("This is Admin");
        }

        public IActionResult Moderator()
        {
            return Ok("This is Moderator ");
        }

        public IActionResult User()
        {
            return Ok("This is User Profile ");
        }
    }
}
