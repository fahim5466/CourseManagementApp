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
using Domain.Relationships;
using static Application.DTOs.DTOHelper;

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

            TestError<StudentDoesNotExist>(result);
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

            TestError<StudentDoesNotExist>(result);
        }

        [Fact]
        public async Task GetStudentById_ValidIdAndStudent_ReturnsStudent()
        {
            // Arrange.

            Role studentRole = new() { Id = Guid.NewGuid(), Name = Role.STUDENT };
            User user = UserFixture().With(x => x.Id, Guid.NewGuid())
                                     .With(x => x.Roles, [studentRole])
                                     .Create();

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
    }
}
