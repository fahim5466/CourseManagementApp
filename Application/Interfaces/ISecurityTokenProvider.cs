using Domain.Entities.Users;

namespace Application.Interfaces
{
    public interface ISecurityTokenProvider
    {
        string CreateJwtToken(User user);

        string CreateRefreshToken();

        string CreateEmailVerificationToken();
    }
}
