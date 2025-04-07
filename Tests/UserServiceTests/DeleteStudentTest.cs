using Application.Services;
using AutoFixture;
using Domain.Entities;
using Domain.Relationships;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.Repositories;
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
    public class DeleteStudentTest
    {
        [Fact]
        public async Task DeleteStudent_StudentDoesNotExist_ReturnError()
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

            // Act.

            Result result = await userService.DeleteStudentAsync(Guid.NewGuid().ToString());

            // Assert.

            TestError<StudentDoesNotExistError>(result);
        }

        [Fact]
        public async Task DeleteStudent_StudentExists_StudentIsDeleted()
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
