using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
    {
        public async Task<User?> GetUserByEmailWithRolesAsync(string email)
        {
            return await dbContext.Users
                            .Include(u => u.Roles)
                            .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await dbContext.Users
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
