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
    public class DeleteClassTest
    {
        [Theory]
        [InlineData("1")]
        [InlineData("7E89EECB-7A95-48D6-A63B-FE6A4D7588F8")]
        public async Task DeleteClass_InvalidId_ReturnsError(string id)
        {
            // Arrange.

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Classes).ReturnsDbSet([]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result result = await classService.DeleteClassAsync(id);

            // Assert.

            TestError<ClassDoesNotExistError>(result);
        }

        [Fact]
        public async Task DeleteClass_ValidId_DeletesClass()
        {
            // Arrange.

            Class clss1 = ClassFixture().With(x => x.Id, Guid.NewGuid()).Create();

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Classes).ReturnsDbSet([clss1]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassRepository classRepository = new(dbContext);
            ClassService classService = GetClassService(dbContext);

            // Act.

            Result result = await classService.DeleteClassAsync(clss1.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

            Class? deletedClass = await classRepository.GetClassByIdAsync(clss1.Id.ToString());
            deletedClass.Should().BeNull();
        }
    }
}
