using Application.Interfaces;
using Application.Services;
using Domain.Repositories;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Tests.Helpers;
using static Tests.Helpers.MockDependencyHelper;

namespace Tests.UserServiceTests
{
    public static class UserServiceTestHelper
    {
        public static UserService GetUserService(ApplicationDbContext dbContext)
        {
            IConfiguration configuration = GetMockConfiguration();
            ICryptoHasher cryptoHasher = new CryptoHasher();
            ISecurityTokenProvider securityTokenProvider = new SecurityTokenProvider(configuration);
            IUserRepository userRepository = new UserRepository(dbContext);

            return new(userRepository, cryptoHasher, securityTokenProvider, GetMockEmailService(), GetMockHttpHelper(), configuration, dbContext);
        }
    }
}
