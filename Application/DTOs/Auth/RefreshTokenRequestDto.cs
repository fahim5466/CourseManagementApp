using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        public required string JwtToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
