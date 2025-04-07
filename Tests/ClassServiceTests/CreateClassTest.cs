using Application.DTOs;
using Application.DTOs.Class;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.EntityFrameworkCore;
using Tests.Helpers;
using static Application.Errors.ClassErrors;
using static Application.Helpers.ResultHelper;
using static Tests.ClassServiceTests.ClassServiceTestHelper;
using static Tests.Helpers.TestHelper;

namespace Tests.ClassServiceTests
{
    public class CreateClassTest
    {

        [Theory]
        [InlineData("", 1)]
        [InlineData("a", 2)]
        [InlineData("a!", 3)]
        public async Task CreateClass_InvalidName_ReturnsValidationError(string className, int caseNo)
        {
            // Arrange.

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Classes).ReturnsDbSet([]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            ClassRequestDto request = new() { Name = className};

            // Act.

            Result<ClassResponseDto> result = await classService.CreateClassAsync(request);

            // Assert.

            TestError<BadClassCreateOrUpdateRequest>(result);

            BadClassCreateOrUpdateRequest badCreateClassRequest = (BadClassCreateOrUpdateRequest)result.ProblemDetails!;
            badCreateClassRequest.Errors.Should().NotBeNull();

            if(caseNo == 1)
            {
                badCreateClassRequest.Errors.Should().ContainKey(nameof(ClassRequestDto.Name));
                badCreateClassRequest.Errors[nameof(ClassRequestDto.Name)].Should().Contain(ClassRequestDto.NAME_REQ_ERR_MSG);
            }
            else if(caseNo == 2)
            {
                badCreateClassRequest.Errors.Should().ContainKey(nameof(ClassRequestDto.Name));
                badCreateClassRequest.Errors[nameof(ClassRequestDto.Name)].Should().Contain(ClassRequestDto.NAME_MINLEN_ERR_MSG);
            }
            else if (caseNo == 3)
            {
                badCreateClassRequest.Errors.Should().ContainKey(nameof(ClassRequestDto.Name));
                badCreateClassRequest.Errors[nameof(ClassRequestDto.Name)].Should().Contain(ClassRequestDto.NAME_ALPHNUM_ERR_MSG);
            }
        }

        [Fact]
        public async Task CreateClass_DuplicateName_ReturnsError()
        {
            // Arrange.

            Class clss = ClassFixture().With(x => x.Name, "abc").Create();

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Classes).ReturnsDbSet([clss]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            ClassRequestDto request = new() { Name = clss.Name };

            // Act.

            Result<ClassResponseDto> result = await classService.CreateClassAsync(request);

            // Assert.

            TestError<ClassAlreadyExistsError>(result);
        }

        [Fact]
        public async Task CreateClass_ValidName_CreatesNewClass()
        {
            // Arrange.

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Classes).ReturnsDbSet([]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassRepository classRepository = new ClassRepository(dbContext);
            ClassService classService = GetClassService(dbContext);

            ClassRequestDto request = new() { Name = "abc" };

            // Act.

            Result<ClassResponseDto> result = await classService.CreateClassAsync(request);

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status201Created);

            Class? newClass = await classRepository.GetClassByNameAsync(request.Name);
            newClass.Should().NotBeNull();

            ClassResponseDto? response = result.Value;
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(newClass.ToClassResponseDto());
        }
    }
}
