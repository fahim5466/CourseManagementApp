using Application.DTOs;
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
using static Application.Errors.CourseErrors;
using static Application.Helpers.ResultHelper;
using static Tests.CourseServiceTests.CourseServiceTestHelper;
using static Tests.Helpers.TestHelper;

namespace Tests.CourseServiceTests
{
    public class UpdateCourseTest
    {
        [Theory]
        [InlineData("", 1)]
        [InlineData("a", 2)]
        [InlineData("a!", 3)]
        public async Task UpdateCourse_InvalidName_ReturnsValidationError(string courseName, int caseNo)
        {
            // Arrange.

            Course course = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, [course]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            CourseRequestDto request = new() { Name = courseName, ClassIds = [] };

            // Act.

            Result<CourseResponseDtoWithClasses> result = await courseService.UpdateCourseAsync(course.Id.ToString(), request);

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
        public async Task UpdateCourse_DuplicateName_ReturnsError()
        {
            // Arrange.

            Course course1 = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();
            Course course2 = CourseFixture()
                             .With(x => x.Id, Guid.NewGuid())
                             .With(x => x.Name, "abc")
                             .Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, [course1, course2]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            CourseRequestDto request = new() { Name = course2.Name, ClassIds = [] };

            // Act.

            Result<CourseResponseDtoWithClasses> result = await courseService.UpdateCourseAsync(course1.Id.ToString(), request);

            // Assert.

            TestError<CourseAlreadyExistsError>(result);
        }

        [Fact]
        public async Task UpdateCourse_CourseDoesNotExist_ReturnsError()
        {
            // Arrange.

            Course course = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, [course]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            CourseRequestDto request = new() { Name = "abc", ClassIds = [] };

            // Act.

            Result<CourseResponseDtoWithClasses> result = await courseService.UpdateCourseAsync(Guid.NewGuid().ToString(), request);

            // Assert.

            TestError<CourseDoesNotExistError>(result);
        }

        [Fact]
        public async Task UpdateCourse_InvalidClassId_ReturnsError()
        {
            // Arrange.

            Course course = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();
            Class clss = ClassFixture().With(c => c.Id, Guid.NewGuid()).Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss]);
            mockDbContext.CreateDbSetMock(x => x.Courses, [course]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            CourseRequestDto request = new() { Name = "abc", ClassIds = [Guid.NewGuid().ToString()] };

            // Act.

            Result<CourseResponseDtoWithClasses> result = await courseService.UpdateCourseAsync(course.Id.ToString(), request);

            // Assert.

            TestError<InvalidClassIdsError>(result);
        }

        [Fact]
        public async Task UpdateCourse_ValidRequest_UpdatesCourseWithClasses()
        {
            // Arrange.

            Course course = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();
            Class clss1 = ClassFixture().With(c => c.Id, Guid.NewGuid()).Create();
            Class clss2 = ClassFixture().With(c => c.Id, Guid.NewGuid()).Create();

            course.Classes.Add(clss1);

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss1, clss2]);
            mockDbContext.CreateDbSetMock(x => x.Courses, [course]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseRepository courseRepository = new CourseRepository(dbContext);
            CourseService courseService = GetCourseService(dbContext);

            CourseRequestDto request = new() { Name = "abc", ClassIds = [clss2.Id.ToString()] };

            // Act.

            Result<CourseResponseDtoWithClasses> result = await courseService.UpdateCourseAsync(course.Id.ToString(), request);

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            Course? updatedCourse = await courseRepository.GetCourseByIdWithClassesAsync(course.Id.ToString());
            updatedCourse.Should().NotBeNull();
            updatedCourse.Name.Should().Be(request.Name);
            updatedCourse.Classes.Should().HaveCount(1);
            updatedCourse.Classes.Should().NotContain(x => x.Id == clss1.Id);
            updatedCourse.Classes.Should().Contain(x => x.Id == clss2.Id);

            CourseResponseDtoWithClasses? response = result.Value;
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(updatedCourse.ToCourseResponseDtoWithClasses());
        }
    }
}
