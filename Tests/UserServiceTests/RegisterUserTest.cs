using Application.DTOs.User;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Tests.Helpers;
using static Application.Errors.UserErrors;
using static Application.Helpers.ResultHelper;
using static Tests.Helpers.TestHelper;
using static Tests.UserServiceTests.UserServiceTestHelper;

namespace Tests.UserServiceTests
{
    public class RegisterUserTest
    {
        [Fact]
        public async Task RegisterStudent_EmptyRequestValues_ReturnsValidationErrorAsync()
        {
            // Arrange.

            UserService userService = GetUserService(MockDependencyHelper.GetMockDbContext().Object);

            UserRequestDto request = new() { Email = string.Empty, Password = string.Empty, Name = string.Empty };

            // Act.

            Result result = await userService.RegisterStudentAsync(request, string.Empty);

            // Assert.

            TestError<BadRegisterOrUpdateUserRequest>(result);

            BadRegisterOrUpdateUserRequest badRequest = (BadRegisterOrUpdateUserRequest)result.ProblemDetails!;
            badRequest.Errors.Should().NotBeNull();

            badRequest.Errors.Should().ContainKey(nameof(UserRequestDto.Email));
            badRequest.Errors[nameof(UserRequestDto.Email)].Should().Contain(UserRequestDto.EMAIL_REQ_ERR_MSG);

            badRequest.Errors.Should().ContainKey(nameof(UserRequestDto.Password));
            badRequest.Errors[nameof(UserRequestDto.Password)].Should().Contain(UserRequestDto.PASSWORD_REQ_ERR_MSG);

            badRequest.Errors.Should().ContainKey(nameof(UserRequestDto.Name));
            badRequest.Errors[nameof(UserRequestDto.Name)].Should().Contain(UserRequestDto.NAME_REQ_ERR_MSG);
        }

        [Fact]
        public async Task RegisterStudent_InvalidRequestValues_ReturnsValidationErrorAsync()
        {
            // Arrange.

            UserService userService = GetUserService(MockDependencyHelper.GetMockDbContext().Object);

            UserRequestDto request = new() { Email = "a", Password = "1", Name = "b"};

            // Act.

            Result result = await userService.RegisterStudentAsync(request, string.Empty);

            // Assert.

            TestError<BadRegisterOrUpdateUserRequest>(result);

            BadRegisterOrUpdateUserRequest badRequest = (BadRegisterOrUpdateUserRequest)result.ProblemDetails!;
            badRequest.Errors.Should().NotBeNull();

            badRequest.Errors.Should().ContainKey(nameof(UserRequestDto.Email));
            badRequest.Errors[nameof(UserRequestDto.Email)].Should().Contain(UserRequestDto.EMAIL_FORMAT_ERR_MSG);

            badRequest.Errors.Should().ContainKey(nameof(UserRequestDto.Password));
            badRequest.Errors[nameof(UserRequestDto.Password)].Should().Contain(UserRequestDto.PASSWORD_MINLEN_ERR_MSG);

            badRequest.Errors.Should().ContainKey(nameof(UserRequestDto.Name));
            badRequest.Errors[nameof(UserRequestDto.Name)].Should().Contain(UserRequestDto.NAME_MINLEN_ERR_MSG);
        }

        [Fact]
        public async Task RegisterStudent_UserAlreadyExists_ReturnsError()
        {
            // Arrange.
            User existingUser = UserFixture().With(u => u.Email, "test@test.com").Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [existingUser]);

            UserService userService = GetUserService(mockDbContext.Object);

            UserRequestDto request = new() { Email = existingUser.Email, Name = "abcde", Password = "abcde" };

            // Act.

            Result result = await userService.RegisterStudentAsync(request, string.Empty);

            // Assert.

            TestError<UserAlreadyExistsError>(result);
        }

        [Fact]
        public async Task RegisterStudent_Success_ShouldCreateStudent()
        {
            // Arrange.

            Role studentRole = new() { Id = Guid.NewGuid(), Name = Role.STUDENT };

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, []);
            mockDbContext.CreateDbSetMock(x => x.Roles, [studentRole]);

            ApplicationDbContext dbContext = mockDbContext.Object;
            UserRepository userRepository = new(dbContext);
            UserService userService = GetUserService(dbContext);

            UserRequestDto request = new() { Email = "test@test.com", Name = "test_name", Password = "test_pass" };
            CryptoHasher cryptoHasher = new();

            // Act.

            Result result = await userService.RegisterStudentAsync(request, string.Empty);

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status201Created);

            User? newUser = dbContext.Users.FirstOrDefault(u => u.Email == request.Email);
            newUser.Should().NotBeNull();
            newUser.Name.Should().Be(request.Name);
            cryptoHasher.Verify(request.Password, newUser.PasswordHash).Should().BeTrue();
            newUser.IsEmailVerified.Should().BeFalse();
            newUser.EmailVerificationToken.Should().NotBeNull();
            newUser.EmailVerificationTokenExpires.Should().NotBeNull();
            newUser.RefreshTokenHash.Should().BeNull();
            newUser.RefreshTokenExpires.Should().BeNull();
            newUser.Roles.Should().HaveCount(1);
            newUser.Roles.Should().Contain(r => r.Name == Role.STUDENT);
        }
    }
}
