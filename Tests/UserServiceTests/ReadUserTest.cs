using Application.DTOs.User;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using Domain.Relationships;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Tests.Helpers;
using static Application.DTOs.DTOHelper;
using static Application.Errors.UserErrors;
using static Application.Helpers.ResultHelper;
using static Tests.Helpers.TestHelper;
using static Tests.UserServiceTests.UserServiceTestHelper;

namespace Tests.UserServiceTests
{
    public class ReadUserTest
    {
        [Theory]
        [InlineData("1")]
        [InlineData("756FB2B3-3617-40C5-8A87-6A5959F49769")]
        public async Task GetStudentById_InvalidId_ReturnsError(string id)
        {
            // Arrange.

            Role studentRole = new() { Id = Guid.NewGuid(), Name = Role.STUDENT };

            User user = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();

            UserRole userRole = new() { UserId = user.Id, RoleId = studentRole.Id };

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [user]);
            mockDbContext.CreateDbSetMock(x => x.Roles, [studentRole]);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, [userRole]);
            UserService userService = GetUserService(mockDbContext.Object);

            // Act.

            Result<UserResponseDto> result = await userService.GetStudentByIdAsync(id);

            // Assert.

            TestError<StudentDoesNotExistError>(result);
        }

        [Fact]
        public async Task GetStudentById_ValidIdButNotStudent_ReturnsError()
        {
            // Arrange.

            Role adminRole = new() { Id = Guid.NewGuid(), Name = Role.ADMIN };

            User user = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();

            UserRole userRole = new() { UserId = user.Id, RoleId = adminRole.Id };

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [user]);
            mockDbContext.CreateDbSetMock(x => x.Roles, [adminRole]);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, [userRole]);
            UserService userService = GetUserService(mockDbContext.Object);

            // Act.

            Result<UserResponseDto> result = await userService.GetStudentByIdAsync(user.Id.ToString());

            // Assert.

            TestError<StudentDoesNotExistError>(result);
        }

        [Fact]
        public async Task GetStudentById_ValidIdAndStudent_ReturnsStudent()
        {
            // Arrange.

            Role studentRole = new() { Id = Guid.NewGuid(), Name = Role.STUDENT };

            User user = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();

            UserRole userRole = new() { UserId = user.Id, RoleId = studentRole.Id };

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [user]);
            mockDbContext.CreateDbSetMock(x => x.Roles, [studentRole]);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, [userRole]);
            UserService userService = GetUserService(mockDbContext.Object);

            // Act.

            Result<UserResponseDto> result = await userService.GetStudentByIdAsync(user.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            UserResponseDto responseDto = result.Value!;
            responseDto.Should().BeEquivalentTo(user.ToUserResponseDto());
        }

        [Fact]
        public async Task GetAllStudents__ReturnsStudents()
        {
            // Arrange.

            Role adminRole = new() { Id = Guid.NewGuid(), Name = Role.ADMIN };
            Role staffRole = new() { Id = Guid.NewGuid(), Name = Role.STAFF };
            Role studentRole = new() { Id = Guid.NewGuid(), Name = Role.STUDENT };

            User user1 = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();
            User user2 = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();
            User user3 = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();
            User user4 = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();

            UserRole userRole1 = new() { UserId = user1.Id, RoleId = adminRole.Id };
            UserRole userRole2 = new() { UserId = user2.Id, RoleId = staffRole.Id };
            UserRole userRole3 = new() { UserId = user3.Id, RoleId = studentRole.Id };
            UserRole userRole4 = new() { UserId = user4.Id, RoleId = studentRole.Id };

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [user1, user2, user3, user4]);
            mockDbContext.CreateDbSetMock(x => x.Roles, [adminRole, staffRole, studentRole]);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, [userRole1, userRole2, userRole3, userRole4]);
            UserService userService = GetUserService(mockDbContext.Object);

            // Act.

            Result<List<UserResponseDto>> result = await userService.GetAllStudentsAsync();

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            List<UserResponseDto> userResponses = result.Value!;
            userResponses.Should().HaveCount(2);

            UserResponseDto? userResponse1 = userResponses.FirstOrDefault(x => x.Id == user3.Id.ToString());
            userResponse1.Should().NotBeNull();
            userResponse1.Should().BeEquivalentTo(user3.ToUserResponseDto());

            UserResponseDto? userResponse2 = userResponses.FirstOrDefault(x => x.Id == user4.Id.ToString());
            userResponse2.Should().NotBeNull();
            userResponse2.Should().BeEquivalentTo(user4.ToUserResponseDto());
        }
    }
}
