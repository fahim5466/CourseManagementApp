using Application.Services;
using AutoFixture;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Database;
using Moq;
using Moq.EntityFrameworkCore;
using Tests.Helpers;
using static Application.Helpers.ResultHelper;
using static Tests.AuthServiceTests.AuthServiceTestHelper;
using static Tests.Helpers.TestHelper;

namespace Tests.AuthServiceTests
{
    public class LogoutTest
    {
        [Fact]
        public async Task Logout_UserIsLoggedIn_ExpiresRefreshToken()
        {
            // Arrange.

            Fixture fixture = new Fixture();
            User user = UserFixture().With(u => u.RefreshTokenExpires, DateTime.UtcNow.AddMinutes(-10)).Create();

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([user]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            AuthService authService = GetAuthService(dbContext);

            // Act.

            Result result = await authService.LogoutAsync(user.Email);

            // Assert.

            TestSuccess(result);

            User? modifiedUser = dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
            modifiedUser.Should().NotBeNull();
            (modifiedUser.RefreshTokenExpires - user.RefreshTokenExpires).Should().BePositive();
        }
    }
}
