using Application.DTOs.Auth;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.EntityFrameworkCore;
using System.Security.Claims;
using Tests.Helpers;
using static Application.Errors.AuthErrors;
using static Application.Helpers.ResultHelper;
using static Tests.AuthServiceTests.AuthServiceTestHelper;
using static Tests.Helpers.TestHelper;

namespace Tests.AuthServiceTests
{
    public class LoginTest
    {
        [Fact]
        public async Task Login_EmailAndPasswordIsEmpty_ReturnsValidationError()
        {
            // Arrange

            AuthService authService = GetAuthService(MockDependencyHelper.GetMockDbContext().Object);

            LoginRequestDto request = new() { Email = string.Empty, Password = string.Empty};

            // Act

            Result<LoginResponseDto> result = await authService.LoginAsync(request);

            // Assert

            TestError<BadLoginRequest>(result);

            BadLoginRequest badLoginRequest = (BadLoginRequest)result.ProblemDetails!;
            badLoginRequest.Errors.Should().NotBeNull();

            badLoginRequest.Errors.Should().ContainKey(nameof(LoginRequestDto.Email));
            badLoginRequest.Errors[nameof(LoginRequestDto.Email)].Should().Contain(LoginRequestDto.EMAIL_REQ_ERR_MSG);

            badLoginRequest.Errors.Should().ContainKey(nameof(LoginRequestDto.Password));
            badLoginRequest.Errors[nameof(LoginRequestDto.Password)].Should().Contain(LoginRequestDto.PASSWORD_REQ_ERR_MSG);
        }

        [Fact]
        public async Task Login_UserWithEmailDoesNotExist_ReturnsError()
        {
            // Arrange

            Fixture fixture = new();

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([]);

            AuthService authService = GetAuthService(mockDbContext.Object);

            LoginRequestDto request = fixture.Create<LoginRequestDto>();

            // Act

            Result<LoginResponseDto> result = await authService.LoginAsync(request);

            // Assert

            TestError<InvalidLoginCredentialsError>(result);
        }

        [Fact]
        public async Task Login_PasswordDoesNotMatch_ReturnsError()
        {
            // Arrange

            Fixture fixture = new();
            CryptoHasher cryptoHasher = new();

            User user = UserFixture().With(x => x.Email, "test@email.com")
                                     .With(x => x.PasswordHash, cryptoHasher.EnhancedHash("testpass"))
                                     .Create();

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([user]);

            AuthService authService = GetAuthService(mockDbContext.Object);

            LoginRequestDto request = fixture.Build<LoginRequestDto>()
                                             .With(x => x.Email, user.Email)
                                             .With(x => x.Password, "testpassnotmatch")
                                             .Create();

            // Act

            Result<LoginResponseDto> result = await authService.LoginAsync(request);

            // Assert

            TestError<InvalidLoginCredentialsError>(result);
        }

        [Fact]
        public async Task Login_EmailNotVerified_ReturnsError()
        {
            // Arrange

            Fixture fixture = new();
            CryptoHasher cryptoHasher = new();

            User user = UserFixture().With(x => x.Email, "test@email.com")
                                     .With(x => x.PasswordHash, cryptoHasher.EnhancedHash("testpass"))
                                     .With(x => x.IsEmailVerified, false)
                                     .Create();

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([user]);

            AuthService authService = GetAuthService(mockDbContext.Object);

            LoginRequestDto request = fixture.Build<LoginRequestDto>()
                                             .With(x => x.Email, user.Email)
                                             .With(x => x.Password, "testpass")
                                             .Create();

            // Act

            Result<LoginResponseDto> result = await authService.LoginAsync(request);

            // Assert

            TestError<EmailIsNotVerifiedError>(result);
        }

        [Fact]
        public async Task Login_LoginSuccess_ReturnsJwtAndRefreshToken()
        {
            // Arrange

            Fixture fixture = new();
            IConfiguration configuration = MockDependencyHelper.GetMockConfiguration();
            CryptoHasher cryptoHasher = new();
            SecurityTokenProvider securityTokenProvider = new(configuration);

            Role adminRole = RoleFixture().With(x => x.Name, Role.ADMIN).Create();

            User user = fixture.Build<User>()
                               .With(x => x.Email, "test@email.com")
                               .With(x => x.PasswordHash, cryptoHasher.EnhancedHash("testpass"))
                               .With(x => x.IsEmailVerified, true)
                               .Without(x => x.RefreshTokenHash)
                               .Without(x => x.RefreshTokenExpires)
                               .With(x => x.Roles, [adminRole])
                               .Create();

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([user]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            AuthService authService = GetAuthService(dbContext);

            LoginRequestDto request = fixture.Build<LoginRequestDto>()
                                             .With(x => x.Email, user.Email)
                                             .With(x => x.Password, "testpass")
                                             .Create();

            // Act

            Result<LoginResponseDto> result = await authService.LoginAsync(request);

            // Assert

            TestSuccess(result);
            result.Value.Should().NotBeNull();

            string jwtToken = result.Value.JwtToken;
            string refreshToken = result.Value.RefreshToken;

            // Jwt token should be okay.

            jwtToken.Should().NotBeNull();
            
            ClaimsPrincipal? claimsPrincipal = securityTokenProvider.ValidateJwtToken(jwtToken);
            claimsPrincipal.Should().NotBeNull();
            
            List<Claim> claims = claimsPrincipal.Claims.ToList();
            claims.Should().Contain(c => c.Value == user.Id.ToString());
            claims.Should().Contain(c => c.Value == user.Email);
            claims.Should().Contain(c => c.Value == adminRole.Name);

            // Refresh token should be okay.

            refreshToken.Should().NotBeNull();

            User? updatedUser = dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser.RefreshTokenHash.Should().NotBeNull();
            updatedUser.RefreshTokenExpires.Should().NotBeNull();

            cryptoHasher.Verify(refreshToken, updatedUser.RefreshTokenHash).Should().BeTrue();
        }
    }
}
