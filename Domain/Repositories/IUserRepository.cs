using Domain.Entities.Roles;
using Domain.Entities.Users;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailWithRolesAsync(string email);
    }
}
