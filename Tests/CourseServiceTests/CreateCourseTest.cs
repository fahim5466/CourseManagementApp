using Application.DTOs.Class;
using Application.DTOs.Course;
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
using static Application.Errors.CourseErrors;
using static Application.Helpers.ResultHelper;
using static Tests.CourseServiceTests.CourseServiceTestHelper;
using static Tests.Helpers.TestHelper;

namespace Tests.CourseServiceTests
{
    public class CreateCourseTest
    {
        [Theory]
        [InlineData("", 1)]
        [InlineData("a", 2)]
        [InlineData("a!", 3)]
        public async Task CreateCourse_InvalidName_ReturnsValidationError(string courseName, int caseNo)
        {
            // Arrange.

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            CourseRequestDto request = new() { Name = courseName, ClassIds = [] };

            // Act.

            Result result = await courseService.CreateCourseAsync(request);

            // Assert.

            TestError<BadCourseCreateOrUpdateRequest>(result);

            BadCourseCreateOrUpdateRequest badRequest = (BadCourseCreateOrUpdateRequest)result.ProblemDetails!;
            badRequest.Errors.Should().NotBeNull();

            if (caseNo == 1)
            {
                badRequest.Errors.Should().ContainKey(nameof(CourseRequestDto.Name));
                badRequest.Errors[nameof(CourseRequestDto.Name)].Should().Contain(CourseRequestDto.NAME_REQ_ERR_MSG);
            }
            else if (caseNo == 2)
            {
                badRequest.Errors.Should().ContainKey(nameof(CourseRequestDto.Name));
                badRequest.Errors[nameof(CourseRequestDto.Name)].Should().Contain(CourseRequestDto.NAME_MINLEN_ERR_MSG);
            }
            else if (caseNo == 3)
            {
                badRequest.Errors.Should().ContainKey(nameof(CourseRequestDto.Name));
                badRequest.Errors[nameof(CourseRequestDto.Name)].Should().Contain(CourseRequestDto.NAME_ALPHNUM_ERR_MSG);
            }
        }

        [Fact]
        public async Task CreateCourse_DuplicateName_ReturnsError()
        {
            // Arrange.

            Course course = CourseFixture().With(x => x.Name, "abc").Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, [course]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            CourseRequestDto request = new() { Name = course.Name, ClassIds = [] };

            // Act.

            Result result = await courseService.CreateCourseAsync(request);

            // Assert.

            TestError<CourseAlreadyExistsError>(result);
        }

        [Fact]
        public async Task CreateCourse_InvalidClassId_ReturnsError()
        {
            // Arrange.

            Class clss = ClassFixture().With(c => c.Id, Guid.NewGuid()).Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss]);
            mockDbContext.CreateDbSetMock(x => x.Courses, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            CourseRequestDto request = new() { Name = "abc", ClassIds = [Guid.NewGuid().ToString()] };

            // Act.

            Result result = await courseService.CreateCourseAsync(request);

            // Assert.

            TestError<InvalidClassIdsError>(result);
        }

        [Fact]
        public async Task CreateCourse_ValidRequest_CreatesCourse()
        {
            // Arrange.

            Class clss = ClassFixture().With(c => c.Id, Guid.NewGuid()).Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss]);
            mockDbContext.CreateDbSetMock(x => x.Courses, []);
            mockDbContext.CreateDbSetMock(x => x.CourseClasses, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseRepository courseRepository = new CourseRepository(dbContext);
            CourseService courseService = GetCourseService(dbContext);

            CourseRequestDto request = new() { Name = "abc", ClassIds = [clss.Id.ToString()] };

            // Act.

            Result result = await courseService.CreateCourseAsync(request);

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status201Created);

            Course? course = await courseRepository.GetCourseByNameAsync("abc");
            course.Should().NotBeNull();

            course = await courseRepository.GetCourseByIdWithClassesAsync(course.Id.ToString());
            course.Should().NotBeNull();
            course.Classes.Should().HaveCount(1);
            course.Classes[0].Id.Should().Be(clss.Id);
        }
    }
}
