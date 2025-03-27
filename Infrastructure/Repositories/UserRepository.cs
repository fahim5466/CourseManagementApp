using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

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

        public async Task<User?> GetStudentByIdAsync(string id)
        {
            if(!Guid.TryParse(id, out Guid guid))
            {
                return null;
            }

            return await dbContext.Users
                                  .FirstOrDefaultAsync(u => u.Id == guid &&
                                                       u.Roles.Select(r => r.Name).Contains(Role.STUDENT));

        }
    }
}
