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

            IQueryable<User> query = from user in dbContext.Users
                                     join userRole in dbContext.UserRoles
                                     on user.Id equals userRole.UserId
                                     join role in dbContext.Roles
                                     on userRole.RoleId equals role.Id
                                     where user.Id == guid &&
                                           role.Name == Role.STUDENT
                                     select user;

            return await query.FirstOrDefaultAsync();
        }
    }
}
