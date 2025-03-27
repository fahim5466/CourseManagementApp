using Application.Services;
using AutoFixture;
using Domain.Entities;
using Domain.Relationships;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Tests.Helpers;
using static Application.Errors.UserErrors;
using static Application.Helpers.ResultHelper;
using static Tests.Helpers.TestHelper;
using static Tests.UserServiceTests.UserServiceTestHelper;

namespace Tests.UserServiceTests
{
    public class DeleteStudentTest
    {
        [Fact]
        public async Task DeleteStudent_StudentDoesNotExist_ReturnError()
        {
            // Arrange.

            Role studentRole = new() { Id = Guid.NewGuid(), Name = Role.STUDENT };
            User user = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();
            UserRole userRole = new() { UserId = user.Id, RoleId = studentRole.Id };

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [user]);
            mockDbContext.CreateDbSetMock(x => x.Roles, [studentRole]);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, [userRole]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            UserService userService = GetUserService(dbContext);

            // Act.

            Result result = await userService.DeleteStudentAsync(Guid.NewGuid().ToString());

            // Assert.

            TestError<StudentDoesNotExist>(result);
        }

        [Fact]
        public async Task DeleteStudent_StudentExists_StudentIsDeleted()
        {
            // Arrange.

            Role studentRole = new() { Id = Guid.NewGuid(), Name = Role.STUDENT };
            User user = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();
            UserRole userRole = new() { UserId = user.Id, RoleId = studentRole.Id };

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [user]);
            mockDbContext.CreateDbSetMock(x => x.Roles, [studentRole]);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, [userRole]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            UserRepository userRepository = new(dbContext);
            UserService userService = GetUserService(dbContext);

            // Act.

            Result result = await userService.DeleteStudentAsync(user.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

            User? deletedUser = await userRepository.GetStudentByIdAsync(user.Id.ToString());
            deletedUser.Should().BeNull();
        }
    }
}
