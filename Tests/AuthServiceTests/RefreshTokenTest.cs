using Application.DTOs.Auth;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Tests.Helpers;
using static Application.Errors.AuthErrors;
using static Application.Helpers.ResultHelper;
using static Tests.AuthServiceTests.AuthServiceTestHelper;
using static Tests.Helpers.TestHelper;

namespace Tests.AuthServiceTests
{
    public class RefreshTokenTest
    {
        [Fact]
        public async Task RefreshToken_InvalidJwtToken_ReturnsError()
        {
            // Arrange.
            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            AuthService authService = GetAuthService(mockDbContext.Object);

            RefreshTokenRequestDto request = new() { JwtToken = "abc", RefreshToken = "abc" };

            // Act.
            
            Result<RefreshTokenResponseDto> result = await authService.RefreshTokenAsync(request);

            // Assert.

            TestError<InvalidLoginCredentialsError>(result);
        }

        [Fact]
        public async Task RefreshToken_InvalidRefreshToken_ReturnsError()
        {
            // Arrange.
            IConfiguration configuration = MockDependencyHelper.GetMockConfiguration();
            CryptoHasher cryptoHasher = new();
            SecurityTokenProvider securityTokenProvider = new(configuration);

            Fixture fixture = new();
            User user = fixture.Build<User>()
                               .With(u => u.RefreshTokenHash, cryptoHasher.SimpleHash("abcd"))
                               .With(u => u.RefreshTokenExpires, DateTime.UtcNow.AddMinutes(10))
                               .With(u => u.Roles, [])
                               .Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [user]);

            AuthService authService = GetAuthService(mockDbContext.Object);

            string jwtToken = securityTokenProvider.CreateJwtToken(user);

            RefreshTokenRequestDto request = new() { JwtToken = jwtToken, RefreshToken = "abc" };

            // Act.

            Result<RefreshTokenResponseDto> result = await authService.RefreshTokenAsync(request);

            // Assert.

            TestError<InvalidLoginCredentialsError>(result);
        }

        [Fact]
        public async Task RefreshToken_ExpiredRefreshToken_ReturnsError()
        {
            // Arrange.
            IConfiguration configuration = MockDependencyHelper.GetMockConfiguration();
            SecurityTokenProvider securityTokenProvider = new(configuration);
            CryptoHasher cryptoHasher = new();

            string refreshToken = securityTokenProvider.CreateRefreshToken();
            string refreshTokenHash = cryptoHasher.SimpleHash(refreshToken);

            Fixture fixture = new();
            User user = fixture.Build<User>()
                               .With(u => u.RefreshTokenHash, refreshTokenHash)
                               .With(u => u.RefreshTokenExpires, DateTime.UtcNow.AddMinutes(-10))
                               .With(u => u.Roles, [])
                               .Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [user]);

            AuthService authService = GetAuthService(mockDbContext.Object);

            string jwtToken = securityTokenProvider.CreateJwtToken(user);

            RefreshTokenRequestDto request = new() { JwtToken = jwtToken, RefreshToken = refreshToken };

            // Act.

            Result<RefreshTokenResponseDto> result = await authService.RefreshTokenAsync(request);

            // Assert.

            TestError<InvalidLoginCredentialsError>(result);
        }

        [Fact]
        public async Task RefreshToken_ValidTokens_ReturnsNewTokens()
        {
            // Arrange.
            IConfiguration configuration = MockDependencyHelper.GetMockConfiguration();
            SecurityTokenProvider securityTokenProvider = new(configuration);
            CryptoHasher cryptoHasher = new();

            string refreshToken = securityTokenProvider.CreateRefreshToken();
            string refreshTokenHash = cryptoHasher.SimpleHash(refreshToken);

            Fixture fixture = new();
            User user = fixture.Build<User>()
                               .With(u => u.RefreshTokenHash, refreshTokenHash)
                               .With(u => u.RefreshTokenExpires, DateTime.UtcNow.AddMinutes(10))
                               .With(u => u.Roles, [])
                               .Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [user]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            AuthService authService = GetAuthService(dbContext);

            string jwtToken = securityTokenProvider.CreateJwtToken(user);

            RefreshTokenRequestDto request = new() { JwtToken = jwtToken, RefreshToken = refreshToken };

            // Act.

            Result<RefreshTokenResponseDto> result = await authService.RefreshTokenAsync(request);

            // Assert.

            TestSuccess(result);

            RefreshTokenResponseDto response = result.Value!;
            
            response.JwtToken.Should().NotBeNull();
            response.RefreshToken.Should().NotBeNull();

            User? modifiedUser = dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
            modifiedUser.Should().NotBeNull();
            modifiedUser.RefreshTokenHash.Should().NotBeNull();
            modifiedUser.RefreshTokenExpires.Should().NotBeNull();
            modifiedUser.RefreshTokenHash.Should().NotBeEquivalentTo(refreshTokenHash);

            securityTokenProvider.ValidateJwtToken(response.JwtToken).Should().NotBeNull();
            cryptoHasher.Verify(response.RefreshToken, modifiedUser.RefreshTokenHash).Should().BeTrue();
        }
    }
}
