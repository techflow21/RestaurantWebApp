
namespace Contracts.DTOs
{
    public class RegisterResponseDto
    {
        public bool IsSuccessfulRegistration { get; set; }
        public string? Errors { get; set; }
    }
}
