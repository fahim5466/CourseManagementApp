namespace Application.DTOs.Auth
{
    public class RefreshTokenResponseDto
    {
        public required string JwtToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
