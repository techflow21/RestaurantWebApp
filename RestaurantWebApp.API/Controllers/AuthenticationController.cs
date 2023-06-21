using Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services.ServiceInterfaces;
using Twilio.Jwt.AccessToken;

namespace RestaurantWebApp.API.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }


        [HttpPost("register-user")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var newUser = await _authenticationService.Register(model);
            //return Ok(newUser);
            if (newUser == null)
            {
                return BadRequest(new RegisterResponseDto { Errors = "Registration not successful" });
            }
            return StatusCode(201);
        }


        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                await _authenticationService.ConfirmEmailAsync(userId, token);
                return Ok("Email confirmed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthenticationRequestDto model)
        {
            var response = await _authenticationService.Authenticate(model);

            if (!response.IsAuthSuccessful)
            {
                return BadRequest("Email is not confirmed. Please check your email and confirm your account.");
            }
            if (response == null)
            {
                return Unauthorized(new AuthenticationResponseDto { ErrorMessage = "Invalid Authentication" });
            }
            return Ok(new AuthenticationResponseDto { IsAuthSuccessful = true, AccessToken = response.AccessToken });
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authenticationService.Logout();
            return NoContent();
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            await _authenticationService.ForgotPassword(model);
            return Ok();
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            await _authenticationService.ResetPassword(model);
            return Ok();
        }
    }
}
