using Domain.Entities.Roles;
using Domain.Entities.Users;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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

        public Task<User?> GetUserByEmailAsync(string email)
        {
            return dbContext.Users
                            .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task CreateUserWithRolesAsync(User user, List<string> roleNames)
        {
            List<Role> roles = dbContext.Roles.Where(r => roleNames.Contains(r.Name)).ToList();

            user.Roles = roles;

            dbContext.Users.Add(user);

            await dbContext.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmailVerificationTokenAsync(string token)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
        }
    }
}
