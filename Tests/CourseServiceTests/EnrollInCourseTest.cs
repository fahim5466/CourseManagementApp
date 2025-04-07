using Application.DTOs.Course;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using Domain.Relationships;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.EntityFrameworkCore;
using Tests.Helpers;
using static Application.Errors.CourseErrors;
using static Application.Errors.UserErrors;
using static Application.Helpers.ResultHelper;
using static Tests.CourseServiceTests.CourseServiceTestHelper;
using static Tests.Helpers.TestHelper;

namespace Tests.CourseServiceTests
{
    public class EnrollInCourseTest
    {
        [Theory]
        [InlineData("1", "885A60D5-A62E-46C4-95D6-E4684B84EF6B", 1)]
        [InlineData("885A60D5-A62E-46C4-95D6-E4684B84EF6B", "885A60D5-A62E-46C4-95D6-E4684B84EF6B", 2)]
        [InlineData("A27F997A-AC4B-4F2B-AB1E-72DE073DAA52", "1", 3)]
        [InlineData("A27F997A-AC4B-4F2B-AB1E-72DE073DAA52", "A27F997A-AC4B-4F2B-AB1E-72DE073DAA52", 4)]
        public async Task EnrollStudentInCourse_InvalidIds_ReturnsError(string courseId, string studentId, int caseNo)
        {
            // Arrange.

            Course course = CourseFixture().With(x => x.Id, Guid.Parse("A27F997A-AC4B-4F2B-AB1E-72DE073DAA52")).Create();
            User student = UserFixture().With(x => x.Id, Guid.Parse("885A60D5-A62E-46C4-95D6-E4684B84EF6B")).Create();
            Role studentRole = RoleFixture().With(x => x.Id, Guid.NewGuid())
                                            .With(x => x.Name, Role.STUDENT)
                                            .Create();
            UserRole userRole = new() { UserId = student.Id, RoleId = studentRole.Id };

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Courses).ReturnsDbSet([course]);
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([student]);
            mockDbContext.Setup(x => x.Roles).ReturnsDbSet([studentRole]);
            mockDbContext.Setup(x => x.UserRoles).ReturnsDbSet([userRole]);
            mockDbContext.Setup(x => x.CourseEnrollments).ReturnsDbSet([]);

            ApplicationDbContext dbContext = mockDbContext.Object;
            CourseService courseService = GetCourseService(dbContext);

            CourseEnrollmentRequestDto request = new() { CourseId = courseId, StudentId = studentId };

            // Act.

            Result result = await courseService.EnrollStudentInCourseAsync(request);

            // Assert.

            if (caseNo == 1 || caseNo == 2)
            {
                TestError<CourseDoesNotExistError>(result);
            }
            else
            {
                TestError<StudentDoesNotExistError>(result);
            }
        }

        [Fact]
        public async Task EnrollStudentInCourse_AlreadyEnrolled_ReturnsError()
        {
            // Arrange.

            Course course = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();
            User student = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();
            Role studentRole = RoleFixture().With(x => x.Id, Guid.NewGuid())
                                            .With(x => x.Name, Role.STUDENT)
                                            .Create();
            UserRole userRole = new() { UserId = student.Id, RoleId = studentRole.Id };
            CourseEnrollment courseEnrollment = new() { CourseId = course.Id, StudentId = student.Id };

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Courses).ReturnsDbSet([course]);
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([student]);
            mockDbContext.Setup(x => x.Roles).ReturnsDbSet([studentRole]);
            mockDbContext.Setup(x => x.UserRoles).ReturnsDbSet([userRole]);
            mockDbContext.Setup(x => x.CourseEnrollments).ReturnsDbSet([courseEnrollment]);

            ApplicationDbContext dbContext = mockDbContext.Object;
            CourseService courseService = GetCourseService(dbContext);

            CourseEnrollmentRequestDto request = new() { CourseId = course.Id.ToString(), StudentId = student.Id.ToString() };

            // Act.

            Result result = await courseService.EnrollStudentInCourseAsync(request);

            // Assert.

            TestError<StudentAlreadyEnrolledInCourseError>(result);

        }

        [Fact]
        public async Task EnrollStudentInCourse_ValidRequest_CreatesEnrollment()
        {
            // Arrange.

            Course course = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();
            User student = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();
            Role studentRole = RoleFixture().With(x => x.Id, Guid.NewGuid())
                                            .With(x => x.Name, Role.STUDENT)
                                            .Create();
            UserRole userRole = new() { UserId = student.Id, RoleId = studentRole.Id };

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Courses).ReturnsDbSet([course]);
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([student]);
            mockDbContext.Setup(x => x.Roles).ReturnsDbSet([studentRole]);
            mockDbContext.Setup(x => x.UserRoles).ReturnsDbSet([userRole]);
            mockDbContext.Setup(x => x.CourseEnrollments).ReturnsDbSet([]);

            ApplicationDbContext dbContext = mockDbContext.Object;
            CourseRepository courseRepository = new(dbContext);
            CourseService courseService = GetCourseService(dbContext);

            CourseEnrollmentRequestDto request = new() { CourseId = course.Id.ToString(), StudentId = student.Id.ToString() };

            // Act.

            Result result = await courseService.EnrollStudentInCourseAsync(request);

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            CourseEnrollment? courseEnrollment = await courseRepository.GetCourseEnrollmentAsync(course.Id, student.Id);
            courseEnrollment.Should().NotBeNull();
            courseEnrollment.CourseId.Should().Be(course.Id);
            courseEnrollment.StudentId.Should().Be(student.Id);
        }
    }
}
