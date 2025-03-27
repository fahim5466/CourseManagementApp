using Application.DTOs;
using Application.DTOs.Class;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using Domain.Repositories;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Tests.Helpers;
using static Application.Errors.ClassErrors;
using static Application.Helpers.ResultHelper;
using static Tests.ClassServiceTests.ClassServiceTestHelper;
using static Tests.Helpers.TestHelper;

namespace Tests.ClassServiceTests
{
    public class ReadClassTest
    {
        [Theory]
        [InlineData("1")]
        [InlineData("7E89EECB-7A95-48D6-A63B-FE6A4D7588F8")]
        public async Task GetClassById_InvalidId_ReturnsErrorAsync(string id)
        {
            // Arrange.

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Classes, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<ClassResponseDto> result = await classService.GetClassByIdAsync(id);

            // Assert.

            TestError<ClassDoesNotExistError>(result);
        }

        [Fact]
        public async Task GetClassById_ValidId_ReturnsClass()
        {
            // Arrange.

            Class clss = ClassFixture().With(x => x.Id, Guid.NewGuid()).Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<ClassResponseDto> result = await classService.GetClassByIdAsync(clss.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            ClassResponseDto classResponseDto = result.Value!;
            classResponseDto.Should().BeEquivalentTo(clss.ToClassResponseDto());
        }

        [Fact]
        public async Task GetAllClasses_ReturnsAllClasses()
        {
            // Arrange.

            Class clss1 = ClassFixture().With(x => x.Id, Guid.NewGuid()).Create();
            Class clss2 = ClassFixture().With(x => x.Id, Guid.NewGuid()).Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss1, clss2]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<List<ClassResponseDto>> result = await classService.GetAllClassesAsync();

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            List<ClassResponseDto> classResponses = result.Value!;
            classResponses.Should().HaveCount(2);

            ClassResponseDto? classResponse1 = classResponses.FirstOrDefault(x => x.Id == clss1.Id.ToString());
            classResponse1.Should().NotBeNull();
            classResponse1.Should().BeEquivalentTo(clss1.ToClassResponseDto());

            ClassResponseDto? classResponse2 = classResponses.FirstOrDefault(x => x.Id == clss2.Id.ToString());
            classResponse2.Should().NotBeNull();
            classResponse2.Should().BeEquivalentTo(clss2.ToClassResponseDto());
        }
    }
}
