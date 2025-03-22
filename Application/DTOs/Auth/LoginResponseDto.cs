namespace Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public required string JwtToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
