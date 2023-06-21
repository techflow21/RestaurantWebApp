
namespace Contracts.DTOs
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string OldPassword { get; set; }
        //public string Token { get; set; }
        public string NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }

}
