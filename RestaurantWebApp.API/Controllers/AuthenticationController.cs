using Microsoft.AspNetCore.Mvc;

namespace RestaurantWebApp.API.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
