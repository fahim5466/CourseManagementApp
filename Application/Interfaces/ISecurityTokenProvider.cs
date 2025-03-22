using Domain.Entities.Users;

namespace Application.Interfaces
{
    public interface ISecurityTokenProvider
    {
        Task<string> CreateJwtTokenAsync(User user);

        string CreateRefreshToken();
    }
}
