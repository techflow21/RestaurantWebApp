namespace Contracts.DTOs
{
    public class AuthenticationResponseDto
    {
        /*public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public bool IsEmailConfirmed { get; set; }
*/
        public string? ErrorMessage { get; set; }
        public string AccessToken { get; set; }
        public bool IsAuthSuccessful { get; set; }
    }
}
