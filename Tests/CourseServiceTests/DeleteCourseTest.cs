using Application.DTOs.Course;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Tests.Helpers;
using static Application.Errors.ClassErrors;
using static Application.Errors.CourseErrors;
using static Application.Helpers.ResultHelper;
using static Tests.CourseServiceTests.CourseServiceTestHelper;
using static Tests.Helpers.TestHelper;

namespace Tests.CourseServiceTests
{
    public class DeleteCourseTest
    {
        [Theory]
        [InlineData("1")]
        [InlineData("8389B6C8-2693-491B-B02C-8FDE3EF0A6B4")]
        public async Task DeleteCourse_InvalidId_ReturnsError(string id)
        {
            // Arrange.

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            // Act.

            Result result = await courseService.DeleteCourseAsync(id);

            // Assert.

            TestError<CourseDoesNotExistError>(result);
        }

        [Fact]
        public async Task DeleteCourse_ValidId_DeletesCourse()
        {
            // Arrange.

            Course course = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, [course]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseRepository courseRepository = new(dbContext);
            CourseService courseService = GetCourseService(dbContext);

            // Act.

            Result result = await courseService.DeleteCourseAsync(course.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

            Course? deletedCourse = await courseRepository.GetCourseByIdWithClassesAsync(course.Id.ToString());
            deletedCourse.Should().BeNull();
        }
    }
}
