using Application.DTOs;
using Application.DTOs.User;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using Domain.Relationships;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.EntityFrameworkCore;
using Tests.Helpers;
using static Application.Errors.UserErrors;
using static Application.Helpers.ResultHelper;
using static Tests.Helpers.TestHelper;
using static Tests.UserServiceTests.UserServiceTestHelper;

namespace Tests.UserServiceTests
{
    public class UpdateStudentTest
    {
        [Fact]
        public async Task UpdateStudent_EmptyRequestValues_ReturnsValidationErrors()
        {
            // Arrange.

            User user = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([user]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            UserService userService = GetUserService(dbContext);

            UserRequestDto request = new() { Email = string.Empty, Password = string.Empty, Name = string.Empty };

            // Act.

            Result<UserResponseDto> result = await userService.UpdateStudentAsync(user.Id.ToString(), request);

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
        public async Task UpdateStudent_InvalidRequestValues_ReturnsValidationErrors()
        {
            // Arrange.

            User user = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([user]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            UserService userService = GetUserService(dbContext);

            UserRequestDto request = new() { Email = "abc", Password = "a", Name = "a" };

            // Act.

            Result<UserResponseDto> result = await userService.UpdateStudentAsync(user.Id.ToString(), request);

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
        public async Task UpdateStudent_UserDoesNotExist_ReturnsError()
        {
            // Arrange.

            Role studentRole = new() { Id = Guid.NewGuid(), Name = Role.STUDENT };
            User user = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();
            UserRole userRole = new() { UserId = user.Id, RoleId = studentRole.Id };

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([user]);
            mockDbContext.Setup(x => x.Roles).ReturnsDbSet([studentRole]);
            mockDbContext.Setup(x => x.UserRoles).ReturnsDbSet([userRole]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            UserService userService = GetUserService(dbContext);

            UserRequestDto request = new() { Email = "new@email.com", Password = "abc12345!!!", Name = "newName" };

            // Act.

            Result<UserResponseDto> result = await userService.UpdateStudentAsync(Guid.NewGuid().ToString(), request);

            // Assert.

            TestError<StudentDoesNotExistError>(result);
        }

        [Fact]
        public async Task UpdateStudent_UserExistsButIsNotStudent_ReturnsError()
        {
            // Arrange.

            Role adminRole = new() { Id = Guid.NewGuid(), Name = Role.ADMIN };
            User user = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();
            UserRole userRole = new() { UserId = user.Id, RoleId = adminRole.Id };

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([user]);
            mockDbContext.Setup(x => x.Roles).ReturnsDbSet([adminRole]);
            mockDbContext.Setup(x => x.UserRoles).ReturnsDbSet([userRole]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            UserService userService = GetUserService(dbContext);

            UserRequestDto request = new() { Email = "new@email.com", Password = "abc12345!!!", Name = "newName" };

            // Act.

            Result<UserResponseDto> result = await userService.UpdateStudentAsync(user.Id.ToString(), request);

            // Assert.

            TestError<StudentDoesNotExistError>(result);
        }

        [Fact]
        public async Task UpdateStudent_NewEmailIsNotUnique_ReturnsError()
        {
            // Arrange.

            Role studentRole = new() { Id = Guid.NewGuid(), Name = Role.STUDENT };
            User user1 = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();
            User user2 = UserFixture().With(x => x.Id, Guid.NewGuid()).With(x => x.Email, "some@email.com").Create();
            UserRole userRole1 = new() { UserId = user1.Id, RoleId = studentRole.Id };
            UserRole userRole2 = new() { UserId = user2.Id, RoleId = studentRole.Id };

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([user1, user2]);
            mockDbContext.Setup(x => x.Roles).ReturnsDbSet([studentRole]);
            mockDbContext.Setup(x => x.UserRoles).ReturnsDbSet([userRole1, userRole2]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            UserService userService = GetUserService(dbContext);

            UserRequestDto request = new() { Email = user2.Email, Password = "abc12345!!!", Name = "newName" };

            // Act.

            Result<UserResponseDto> result = await userService.UpdateStudentAsync(user1.Id.ToString(), request);

            // Assert.

            TestError<UserAlreadyExistsError>(result);
        }

        [Fact]
        public async Task UpdateStudent_ValidRequest_UpdatesStudent()
        {
            // Arrange.

            Role studentRole = new() { Id = Guid.NewGuid(), Name = Role.STUDENT };
            User user1 = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();
            User user2 = UserFixture().With(x => x.Id, Guid.NewGuid()).With(x => x.Email, "some@email.com").Create();
            UserRole userRole1 = new() { UserId = user1.Id, RoleId = studentRole.Id };
            UserRole userRole2 = new() { UserId = user2.Id, RoleId = studentRole.Id };

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([user1, user2]);
            mockDbContext.Setup(x => x.Roles).ReturnsDbSet([studentRole]);
            mockDbContext.Setup(x => x.UserRoles).ReturnsDbSet([userRole1, userRole2]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CryptoHasher cryptoHasher = new();
            UserRepository userRepository = new(dbContext);
            UserService userService = GetUserService(dbContext);

            UserRequestDto request = new() { Email = "new@email.com", Password = "abc12345!!!", Name = "newName" };

            // Act.

            Result<UserResponseDto> result = await userService.UpdateStudentAsync(user1.Id.ToString(), request);

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            User? updatedStudent = await userRepository.GetStudentByIdAsync(user1.Id.ToString());
            updatedStudent.Should().NotBeNull();
            updatedStudent.Name.Should().Be(request.Name);

            cryptoHasher.Verify(request.Password, updatedStudent.PasswordHash).Should().BeTrue();

            updatedStudent.Email.Should().Be(request.Email);
            updatedStudent.IsEmailVerified.Should().BeFalse();
            updatedStudent.EmailVerificationToken.Should().NotBeNull();
            updatedStudent.EmailVerificationTokenExpires.Should().NotBeNull();

            UserResponseDto? response = result.Value;
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(updatedStudent.ToUserResponseDto());
        }
    }
}
