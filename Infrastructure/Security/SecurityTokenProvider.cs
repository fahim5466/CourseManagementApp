using Application;
using Application.Interfaces;
using Domain.Entities.Roles;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security
{
    public class SecurityTokenProvider(IConfiguration configuration) : ISecurityTokenProvider
    {
        private const int REFRESH_TOKEN_SIZE_IN_BYTES = 32;

        public string CreateJwtToken(User user)
        {
            string secretKey = configuration["Jwt:Secret"]!;
            int expirationInMinues = configuration.GetValue<int>("Jwt:ExpirationInMinutes");
            string issuer = configuration["Jwt:Issuer"]!;
            string audience = configuration["Jwt:Audience"]!;

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(secretKey));

            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            ];
            claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r.Name)));


            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinues),
                SigningCredentials = credentials,
                Issuer = issuer,
                Audience = audience
            };

            JsonWebTokenHandler handler = new();

            string token = handler.CreateToken(tokenDescriptor);

            return token;
        }

        public string CreateRefreshToken()
        {
            byte[] randomNumber = new byte[REFRESH_TOKEN_SIZE_IN_BYTES];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            string refreshToken = Convert.ToBase64String(randomNumber);

            return refreshToken;
        }
    }
}
