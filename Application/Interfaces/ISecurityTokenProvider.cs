using Domain.Entities;
using System.Security.Claims;

namespace Application.Interfaces
{
    public interface ISecurityTokenProvider
    {
        string CreateJwtToken(User user);

        ClaimsPrincipal? ValidateJwtToken(string token, bool validateLifetime = true);

        string GetEmailFromClaims(ClaimsPrincipal claimsPrincipal);

        string CreateRefreshToken();

        string CreateEmailVerificationToken();
    }
}
