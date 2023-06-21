
using Contracts.DTOs;

namespace Services.ServiceInterfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponseDto> Authenticate(AuthenticationRequestDto model);
        Task<RegisterDto> Register(RegisterDto model);
        Task ConfirmEmailAsync(string userId, string token);
        Task<AuthenticationResponseDto> ResetPassword(ResetPasswordDto model);
        Task<AuthenticationResponseDto> ForgotPassword(ForgotPasswordDto model);
        Task Logout();
    }
}
