using Application.Interfaces;
using Application.Services;
using Domain.Repositories;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Tests.Helpers;
using static Tests.Helpers.MockDependencyHelper;

namespace Tests.AuthServiceTests
{
    public static class AuthServiceTestHelper
    {
        public static AuthService GetAuthService(ApplicationDbContext dbContext)
        {
            IConfiguration configuration = MockDependencyHelper.GetMockConfiguration();
            ICryptoHasher cryptoHasher = new CryptoHasher();
            ISecurityTokenProvider securityTokenProvider = new SecurityTokenProvider(configuration);
            IUserRepository userRepository = new UserRepository(dbContext);

            return new(configuration, cryptoHasher, securityTokenProvider, userRepository, dbContext, GetMockEmailService(), GetMockHttpHelper());
        }
    }
}
