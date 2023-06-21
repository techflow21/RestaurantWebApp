
using System.ComponentModel.DataAnnotations;

namespace Contracts.DTOs
{
    public class AuthenticationRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }

        //public bool? IsActive { get; set; }
        /*public string UserName { get; set; }
        public string Password { get; set; }
        
        public bool IsAuthSuccessful { get; set; }*/
    }
}
