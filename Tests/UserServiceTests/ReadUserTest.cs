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
    public class ReadUserTest
    {
        [Theory]
        [InlineData("1")]
        [InlineData("756FB2B3-3617-40C5-8A87-6A5959F49769")]
        public async Task GetStudentById_InvalidId_ReturnsError(string id)
        {
            // Arrange.

            User user = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [user]);
            UserService userService = GetUserService(mockDbContext.Object);

            // Act.

            Result<UserResponseDto> result = await userService.GetStudentByIdAsync(id);

            // Assert.

            TestError<StudentDoesNotExist>(result);
        }

        [Fact]
        public async Task GetStudentById_ValidIdButNotStudent_ReturnsErrorA()
        {
            // Arrange.

            Role adminRole = new() { Id = Guid.NewGuid(), Name = Role.ADMIN };
            User user = UserFixture().With(x => x.Id, Guid.NewGuid())
                                     .With(x => x.Roles, [adminRole])
                                     .Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [user]);
            UserService userService = GetUserService(mockDbContext.Object);

            // Act.

            Result<UserResponseDto> result = await userService.GetStudentByIdAsync(user.Id.ToString());

            // Assert.

            TestError<StudentDoesNotExist>(result);
        }

        [Fact]
        public async Task GetStudentById_ValidIdAndStudent_ReturnsErrorA()
        {
            // Arrange.

            Role studentRole = new() { Id = Guid.NewGuid(), Name = Role.STUDENT };
            User user = UserFixture().With(x => x.Id, Guid.NewGuid())
                                     .With(x => x.Roles, [studentRole])
                                     .Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [user]);
            UserService userService = GetUserService(mockDbContext.Object);

            // Act.

            Result<UserResponseDto> result = await userService.GetStudentByIdAsync(user.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            UserResponseDto responseDto = result.Value!;
            responseDto.Id.Should().Be(user.Id.ToString());
            responseDto.Name.Should().Be(user.Name);
            responseDto.Email.Should().Be(user.Email);
            responseDto.IsEmailVerified.Should().Be(user.IsEmailVerified);
        }
    }
}
