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
using static Tests.ClassServiceTests.ClassServiceTestHelper;
using Application.DTOs.Class;
using static Application.Errors.ClassErrors;
using Domain.Repositories;

namespace Tests.ClassServiceTests
{
    public class CreateClassTest
    {

        [Theory]
        [InlineData("", 1)]
        [InlineData("a", 2)]
        [InlineData("a!", 2)]
        public async Task CreateClass_InvalidName_ReturnsValidationError(string className, int caseNo)
        {
            // Arrange.

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Classes, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            IClassService classService = GetClassService(dbContext);

            CreateClassRequestDto request = new() { Name = className};

            // Act.

            Result result = await classService.CreateClassAsync(request);

            // Assert.

            TestError<BadCreateClassRequest>(result);

            BadCreateClassRequest badCreateClassRequest = (BadCreateClassRequest)result.ProblemDetails!;
            badCreateClassRequest.Errors.Should().NotBeNull();

            if(caseNo == 1)
            {
                badCreateClassRequest.Errors.Should().ContainKey(nameof(CreateClassRequestDto.Name));
                badCreateClassRequest.Errors[nameof(CreateClassRequestDto.Name)].Should().Contain(CreateClassRequestDto.NAME_REQ_ERR_MSG);
            }
            else if(caseNo == 2)
            {
                badCreateClassRequest.Errors.Should().ContainKey(nameof(CreateClassRequestDto.Name));
                badCreateClassRequest.Errors[nameof(CreateClassRequestDto.Name)].Should().Contain(CreateClassRequestDto.NAME_MINLEN_ERR_MSG);
            }
            else if (caseNo == 3)
            {
                badCreateClassRequest.Errors.Should().ContainKey(nameof(CreateClassRequestDto.Name));
                badCreateClassRequest.Errors[nameof(CreateClassRequestDto.Name)].Should().Contain(CreateClassRequestDto.NAME_ALPHNUM_ERR_MSG);
            }
        }

        [Fact]
        public async Task CreateClass_DuplicateName_ReturnsError()
        {
            // Arrange.

            Class clss = ClassFixture().With(x => x.Name, "abc").Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            IClassService classService = GetClassService(dbContext);

            CreateClassRequestDto request = new() { Name = clss.Name };

            // Act.

            Result result = await classService.CreateClassAsync(request);

            // Assert.

            TestError<ClassAlreadyExistsError>(result);
        }

        [Fact]
        public async Task CreateClass_ValidName_CreatesNewClass()
        {
            // Arrange.

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Classes, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            IClassRepository classRepository = new ClassRepository(dbContext);
            IClassService classService = GetClassService(dbContext);

            CreateClassRequestDto request = new() { Name = "abc" };

            // Act.

            Result result = await classService.CreateClassAsync(request);

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status201Created);

            Class? newClass = await classRepository.GetClassByNameAsync(request.Name);
            newClass.Should().NotBeNull();
        }
    }
}
