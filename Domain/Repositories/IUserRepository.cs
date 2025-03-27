using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailWithRolesAsync(string email);

        public Task<User?> GetUserByEmailAsync(string email);

        public Task CreateUserWithRolesAsync(User user, List<string> roleNames);

        public Task<User?> GetUserByEmailVerificationTokenAsync(string token);

        public Task<User?> GetStudentByIdAsync(string id);

        public Task<List<User>> GetAllStudentsAsync();
    }
}
