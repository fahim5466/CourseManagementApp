using Application.DTOs.User;
using Application.Services;
using Tests.Helpers;
using static Tests.Helpers.TestHelper;
using static Application.Helpers.ResultHelper;
using static Tests.UserServiceTests.UserServiceTestHelper;
using static Application.Errors.UserErrors;
using FluentAssertions;
using EntityFrameworkCoreMock;
using Infrastructure.Database;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Infrastructure.Security;
using Infrastructure.Repositories;
using Domain.Entities;

namespace Tests.UserServiceTests
{
    public class RegisterTest
    {
        [Fact]
        public async Task RegisterStudent_EmptyRequestValues_ReturnsValidationErrorAsync()
        {
            // Arrange.

            UserService userService = GetUserService(MockDependencyHelper.GetMockDbContext().Object);

            RegisterUserRequestDto request = new() { Email = string.Empty, Password = string.Empty, Name = string.Empty };

            // Act.

            Result result = await userService.RegisterStudentAsync(request, string.Empty);

            // Assert.

            TestError<BadRegisterUserRequest>(result);

            BadRegisterUserRequest badRequest = (BadRegisterUserRequest)result.ProblemDetails!;
            badRequest.Errors.Should().NotBeNull();

            badRequest.Errors.Should().ContainKey(nameof(RegisterUserRequestDto.Email));
            badRequest.Errors[nameof(RegisterUserRequestDto.Email)].Should().Contain(RegisterUserRequestDto.EMAIL_REQ_ERR_MSG);

            badRequest.Errors.Should().ContainKey(nameof(RegisterUserRequestDto.Password));
            badRequest.Errors[nameof(RegisterUserRequestDto.Password)].Should().Contain(RegisterUserRequestDto.PASSWORD_REQ_ERR_MSG);

            badRequest.Errors.Should().ContainKey(nameof(RegisterUserRequestDto.Name));
            badRequest.Errors[nameof(RegisterUserRequestDto.Name)].Should().Contain(RegisterUserRequestDto.NAME_REQ_ERR_MSG);
        }

        [Fact]
        public async Task RegisterStudent_InvalidRequestValues_ReturnsValidationErrorAsync()
        {
            // Arrange.

            UserService userService = GetUserService(MockDependencyHelper.GetMockDbContext().Object);

            RegisterUserRequestDto request = new() { Email = "a", Password = "1", Name = "b"};

            // Act.

            Result result = await userService.RegisterStudentAsync(request, string.Empty);

            // Assert.

            TestError<BadRegisterUserRequest>(result);

            BadRegisterUserRequest badRequest = (BadRegisterUserRequest)result.ProblemDetails!;
            badRequest.Errors.Should().NotBeNull();

            badRequest.Errors.Should().ContainKey(nameof(RegisterUserRequestDto.Email));
            badRequest.Errors[nameof(RegisterUserRequestDto.Email)].Should().Contain(RegisterUserRequestDto.EMAIL_FORMAT_ERR_MSG);

            badRequest.Errors.Should().ContainKey(nameof(RegisterUserRequestDto.Password));
            badRequest.Errors[nameof(RegisterUserRequestDto.Password)].Should().Contain(RegisterUserRequestDto.PASSWORD_MINLEN_ERR_MSG);

            badRequest.Errors.Should().ContainKey(nameof(RegisterUserRequestDto.Name));
            badRequest.Errors[nameof(RegisterUserRequestDto.Name)].Should().Contain(RegisterUserRequestDto.NAME_MINLEN_ERR_MSG);
        }

        [Fact]
        public async Task RegisterStudent_UserAlreadyExists_ReturnsError()
        {
            // Arrange.
            Fixture fixture = new();
            User existingUser = fixture.Build<User>()
                                       .With(u => u.Email, "test@test.com")
                                       .Without(u => u.Roles)
                                       .Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [existingUser]);

            UserService userService = GetUserService(mockDbContext.Object);

            RegisterUserRequestDto request = new() { Email = existingUser.Email, Name = "abcde", Password = "abcde" };

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

            RegisterUserRequestDto request = new() { Email = "test@test.com", Name = "test_name", Password = "test_pass" };
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
