using Application.Interfaces;
using Domain.Entities.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace Infrastructure.Security
{
    public class SecurityTokenProvider(IConfiguration configuration) : ISecurityTokenProvider
    {
        private const int REFRESH_TOKEN_SIZE_IN_BYTES = 32;
        private const int EMAIL_VERIFICATION_TOKEN_SIZE_IN_BYTES = 32;

        public string CreateJwtToken(User user)
        {
            string secretKey = configuration["Jwt:Secret"]!;
            int expirationInMinues = Int32.Parse(configuration["Jwt:ExpirationInMinutes"]!);
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

            JwtSecurityToken token = new(
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expirationInMinues),
                    signingCredentials: credentials,
                    issuer: issuer,
                    audience: audience
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateJwtToken(string token)
        {
            string secretKey = configuration["Jwt:Secret"]!;
            string issuer = configuration["Jwt:Issuer"]!;
            string audience = configuration["Jwt:Audience"]!;
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(secretKey));

            TokenValidationParameters validationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                IssuerSigningKey = securityKey,
                ValidIssuer = issuer,
                ValidAudience = audience,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                JwtSecurityTokenHandler tokenHandler = new();
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception)
            {
                return null; // Token is invalid or expired
            }
        }

        public string CreateRefreshToken()
        {
            return Convert.ToBase64String(CreateToken(REFRESH_TOKEN_SIZE_IN_BYTES));
        }

        public string CreateEmailVerificationToken()
        {
            return Convert.ToHexString(CreateToken(EMAIL_VERIFICATION_TOKEN_SIZE_IN_BYTES));
        }

        public byte[] CreateToken(int tokenSizeInBytes)
        {
            byte[] randomNumber = new byte[tokenSizeInBytes];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return randomNumber;
        }
    }
}
