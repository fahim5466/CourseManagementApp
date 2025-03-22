using Domain.Entities.Roles;
using Domain.Entities.Users;
using Domain.Repositories;
using Infrastructure.Database;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
    {
        public Task<User?> GetUserByEmailWithRolesAsync(string email)
        {
            return dbContext.Users
                            .Include(u => u.Roles)
                            .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
