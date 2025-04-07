using Domain.Entities;
using System.Security.Claims;

namespace Application.Interfaces
{
    public interface ISecurityTokenProvider
    {
        public string CreateJwtToken(User user);

        public ClaimsPrincipal? ValidateJwtToken(string token, bool validateLifetime = true);

        public string GetEmailFromClaims(ClaimsPrincipal claimsPrincipal);

        public string CreateRefreshToken();

        public string CreateEmailVerificationToken();
    }
}
