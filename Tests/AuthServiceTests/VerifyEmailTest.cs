using Application.Services;
using AutoFixture;
using Domain.Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using static Application.Errors.AuthErrors;
using static Application.Helpers.ResultHelper;
using static Tests.Helpers.MockDependencyHelper;
using static Tests.Helpers.TestHelper;

namespace Tests.AuthServiceTests
{
    public class VerifyEmailTest
    {
        [Fact]
        public async Task VerifyEmail_InvalidToken_ReturnsError()
        {
            // Arrange.

            DbContextMock<ApplicationDbContext> mockDbContext = GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, []);

            AuthService authService = AuthServiceTestHelper.GetAuthService(mockDbContext.Object);

            string verificationToken = "abc";

            // Act.

            Result result = await authService.VerifyEmailAsync(verificationToken, "random");

            // Assert.

            TestError<InvalidEmailVerificationToken>(result);
        }

        [Fact]
        public async Task VerifyEmail_ExpiredToken_GeneratesNewToken()
        {
            // Arrange.

            string verificationToken = "abc";
            DateTime verificationTokenExpires = DateTime.UtcNow.AddMinutes(-10);

            User user = UserFixture().With(u => u.EmailVerificationToken, verificationToken)
                                     .With(u => u.EmailVerificationTokenExpires, verificationTokenExpires)
                                     .Create();

            DbContextMock<ApplicationDbContext> mockDbContext = GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [user]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            AuthService authService = AuthServiceTestHelper.GetAuthService(dbContext);

            // Act.

            Result result = await authService.VerifyEmailAsync(verificationToken, "random");

            // Assert.

            TestError<ExpiredEmailVerificationToken>(result);

            User? modifiedUser = dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
            modifiedUser.Should().NotBeNull();
            modifiedUser.EmailVerificationToken.Should().NotBe(verificationToken);
            modifiedUser.EmailVerificationTokenExpires.Should().NotBe(verificationTokenExpires);
        }

        [Fact]
        public async Task VerifyEmail_ValidToken_VerifiesEmail()
        {
            // Arrange.

            string verificationToken = "abc";
            DateTime verificationTokenExpires = DateTime.UtcNow.AddMinutes(10);

            User user = UserFixture().With(u => u.EmailVerificationToken, verificationToken)
                                     .With(u => u.EmailVerificationTokenExpires, verificationTokenExpires)
                                     .Create();

            DbContextMock<ApplicationDbContext> mockDbContext = GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [user]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            AuthService authService = AuthServiceTestHelper.GetAuthService(dbContext);

            // Act.

            Result result = await authService.VerifyEmailAsync(verificationToken, "random");

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            User? modifiedUser = dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
            modifiedUser.Should().NotBeNull();
            modifiedUser.IsEmailVerified.Should().BeTrue();
            modifiedUser.EmailVerificationToken.Should().BeNull();
            modifiedUser.EmailVerificationTokenExpires.Should().BeNull();
        }
    }
}
