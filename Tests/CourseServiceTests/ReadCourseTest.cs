using Application.DTOs;
using Application.DTOs.Course;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Tests.Helpers;
using static Application.Errors.CourseErrors;
using static Application.Helpers.ResultHelper;
using static Tests.CourseServiceTests.CourseServiceTestHelper;
using static Tests.Helpers.TestHelper;

namespace Tests.CourseServiceTests
{
    public class ReadCourseTest
    {
        [Theory]
        [InlineData("1")]
        [InlineData("7E89EECB-7A95-48D6-A63B-FE6A4D7588F8")]
        public async Task GetCourseById_InvalidId_ReturnsErrorAsync(string id)
        {
            // Arrange.

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            // Act.

            Result<CourseResponseDto> result = await courseService.GetCourseByIdAsync(id);

            // Assert.

            TestError<CourseDoesNotExistError>(result);
        }

        [Fact]
        public async Task GetCourseById_ValidId_ReturnsCourseWithClasses()
        {
            // Arrange.
            Class clss1 = ClassFixture().Create();
            Class clss2 = ClassFixture().Create();
            Course course = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();
            course.Classes = [clss1, clss2];

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, [course]);
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss1, clss2]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            // Act.

            Result<CourseResponseDto> result = await courseService.GetCourseByIdAsync(course.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            CourseResponseDto courseResponseDto = result.Value!;
            courseResponseDto.Should().BeEquivalentTo(course.ToCourseResponseDto());
        }

        [Fact]
        public async Task GetAllCourses_ReturnsCoursesWithClasses()
        {
            // Arrange.
            Class clss1 = ClassFixture().Create();
            Class clss2 = ClassFixture().Create();
            Class clss3 = ClassFixture().Create();

            Course course1 = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();
            course1.Classes = [clss1, clss2];

            Course course2 = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();
            course2.Classes = [clss1, clss3];

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, [course1, course2]);
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss1, clss2]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            // Act.

            Result<List<CourseResponseDto>> result = await courseService.GetAllCoursesAsync();

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            List<CourseResponseDto> responseDto = result.Value!;
            responseDto.Should().HaveCount(2);

            responseDto[0].Should().BeEquivalentTo(course1.ToCourseResponseDto());
            responseDto[1].Should().BeEquivalentTo(course2.ToCourseResponseDto());
        }
    }
}
