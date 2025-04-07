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
    public class UpdateClassTest
    {
        [Theory]
        [InlineData("", 1)]
        [InlineData("a", 2)]
        [InlineData("a!", 2)]
        public async Task UpdateClass_InvalidName_ReturnsValidationError(string className, int caseNo)
        {
            // Arrange.

            Class clss = ClassFixture().With(x => x.Id, Guid.NewGuid()).Create();

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Classes).ReturnsDbSet([clss]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            ClassRequestDto request = new() { Name = className };

            // Act.

            Result<ClassResponseDto> result = await classService.UpdateClassAsync(clss.Id.ToString(), request);

            // Assert.

            TestError<BadClassCreateOrUpdateRequest>(result);

            BadClassCreateOrUpdateRequest badCreateClassRequest = (BadClassCreateOrUpdateRequest)result.ProblemDetails!;
            badCreateClassRequest.Errors.Should().NotBeNull();

            if (caseNo == 1)
            {
                badCreateClassRequest.Errors.Should().ContainKey(nameof(ClassRequestDto.Name));
                badCreateClassRequest.Errors[nameof(ClassRequestDto.Name)].Should().Contain(ClassRequestDto.NAME_REQ_ERR_MSG);
            }
            else if (caseNo == 2)
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

        [Theory]
        [InlineData("1")]
        [InlineData("7E89EECB-7A95-48D6-A63B-FE6A4D7588F8")]
        public async Task UpdateClass_InvalidId_ReturnsErrorAsync(string id)
        {
            // Arrange.

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Classes).ReturnsDbSet([]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            ClassRequestDto request = new() { Name = "abc" };

            // Act.

            Result<ClassResponseDto> result = await classService.UpdateClassAsync(id, request);

            // Assert.

            TestError<ClassDoesNotExistError>(result);
        }

        [Fact]
        public async Task UpdateClass_DuplicateName_ReturnsError()
        {
            // Arrange.

            Class clss1 = ClassFixture().With(x => x.Id, Guid.NewGuid()).Create();
            Class clss2 = ClassFixture().With(x => x.Id, Guid.NewGuid()).With(x => x.Name, "abc").Create();

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Classes).ReturnsDbSet([clss1, clss2]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            ClassRequestDto request = new() { Name = clss2.Name };

            // Act.

            Result<ClassResponseDto> result = await classService.UpdateClassAsync(clss1.Id.ToString(), request);

            // Assert.

            TestError<ClassAlreadyExistsError>(result);
        }

        [Fact]
        public async Task UpdateClass_ValidRequest_UpdatesClass()
        {
            // Arrange.

            Class clss1 = ClassFixture().With(x => x.Id, Guid.NewGuid()).Create();
            Class clss2 = ClassFixture().With(x => x.Id, Guid.NewGuid()).Create();

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Classes).ReturnsDbSet([clss1, clss2]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassRepository classRepository = new(dbContext);
            ClassService classService = GetClassService(dbContext);

            ClassRequestDto request = new() { Name = "abcde" };

            // Act.

            Result<ClassResponseDto> result = await classService.UpdateClassAsync(clss1.Id.ToString(), request);

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            Class? updatedClass = await classRepository.GetClassByIdAsync(clss1.Id.ToString());
            updatedClass.Should().NotBeNull();
            updatedClass.Name.Should().Be("abcde");

            ClassResponseDto? response = result.Value;
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(updatedClass.ToClassResponseDto());
        }
    }
}
