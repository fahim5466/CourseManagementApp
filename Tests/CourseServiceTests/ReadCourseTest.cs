using Application.DTOs;
using Application.DTOs.Class;
using Application.DTOs.Course;
using Application.DTOs.User;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using Domain.Relationships;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Tests.Helpers;
using static Application.Errors.CourseErrors;
using static Application.Errors.UserErrors;
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
        public async Task GetCourseById_InvalidId_ReturnsError(string id)
        {
            // Arrange.

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            // Act.

            Result<CourseResponseDtoWithClasses> result = await courseService.GetCourseByIdAsync(id);

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

            Result<CourseResponseDtoWithClasses> result = await courseService.GetCourseByIdAsync(course.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            CourseResponseDtoWithClasses courseResponseDto = result.Value!;
            courseResponseDto.Should().BeEquivalentTo(course.ToCourseResponseDtoWithClasses());
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

            Result<List<CourseResponseDtoWithClasses>> result = await courseService.GetAllCoursesAsync();

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            List<CourseResponseDtoWithClasses> courseResponses = result.Value!;
            courseResponses.Should().HaveCount(2);

            CourseResponseDtoWithClasses? courseResponse1 = courseResponses.FirstOrDefault(x => x.Id == course1.Id.ToString());
            courseResponse1.Should().NotBeNull();
            courseResponse1.Should().BeEquivalentTo(course1.ToCourseResponseDtoWithClasses());

            CourseResponseDtoWithClasses? courseResponse2 = courseResponses.FirstOrDefault(x => x.Id == course2.Id.ToString());
            courseResponse2.Should().NotBeNull();
            courseResponse2.Should().BeEquivalentTo(course2.ToCourseResponseDtoWithClasses());
        }

        [Theory]
        [InlineData("1")]
        [InlineData("7E89EECB-7A95-48D6-A63B-FE6A4D7588F8")]
        public async Task GetClassesOfCourse_InvalidId_ReturnsError(string id)
        {
            // Arrange.

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            // Act.

            Result<List<ClassResponseDto>> result = await courseService.GetClassesOfCourseAsync(id);

            // Assert.

            TestError<CourseDoesNotExistError>(result);
        }

        [Fact]
        public async Task GetClassesOfCourse_ReturnsCourseWithClasses()
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
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss1, clss2, clss3]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            // Act.

            Result<List<ClassResponseDto>> result = await courseService.GetClassesOfCourseAsync(course1.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            List<ClassResponseDto> responses = result.Value!;
            responses.Should().HaveCount(2);

            ClassResponseDto? classResponse1 = responses.FirstOrDefault(x => x.Id == clss1.Id.ToString());
            classResponse1.Should().NotBeNull();
            classResponse1.Should().BeEquivalentTo(clss1.ToClassResponseDto());

            ClassResponseDto? classResponse2 = responses.FirstOrDefault(x => x.Id == clss2.Id.ToString());
            classResponse2.Should().NotBeNull();
            classResponse2.Should().BeEquivalentTo(clss2.ToClassResponseDto());
        }

        [Theory]
        [InlineData("1")]
        [InlineData("7E89EECB-7A95-48D6-A63B-FE6A4D7588F8")]
        public async Task GetStudentsOfCourse_InvalidId_ReturnsError(string id)
        {
            // Arrange.

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            // Act.

            Result<List<UserResponseDto>> result = await courseService.GetStudentsOfCourseAsync(id);

            // Assert.

            TestError<CourseDoesNotExistError>(result);
        }

        [Fact]
        public async Task GetStudentsOfCourse_ReturnsStudents()
        {
            // Arrange.

            User student1 = UserFixture().Create();
            User student2 = UserFixture().Create();
            User student3 = UserFixture().Create();

            Course course1 = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();
            Course course2 = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();

            CourseEnrollment courseEnrollment1 = new() { CourseId = course1.Id, StudentId = student1.Id };
            CourseEnrollment courseEnrollment2 = new() { CourseId = course1.Id, StudentId = student2.Id };
            CourseEnrollment courseEnrollment3 = new() { CourseId = course2.Id, StudentId = student2.Id };
            CourseEnrollment courseEnrollment4 = new() { CourseId = course2.Id, StudentId = student3.Id };

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, [course1, course2]);
            mockDbContext.CreateDbSetMock(x => x.Users, [student1, student2, student3]);
            mockDbContext.CreateDbSetMock(x => x.CourseEnrollments, [courseEnrollment1, courseEnrollment2, courseEnrollment3, courseEnrollment4]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            // Act.

            Result<List<UserResponseDto>> result = await courseService.GetStudentsOfCourseAsync(course1.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            List<UserResponseDto> responses = result.Value!;
            responses.Should().HaveCount(2);

            UserResponseDto? studentResponse1 = responses.FirstOrDefault(x => x.Id == student1.Id.ToString());
            studentResponse1.Should().NotBeNull();
            studentResponse1.Should().BeEquivalentTo(student1.ToUserResponseDto());

            UserResponseDto? studentResponse2 = responses.FirstOrDefault(x => x.Id == student2.Id.ToString());
            studentResponse2.Should().NotBeNull();
            studentResponse2.Should().BeEquivalentTo(student2.ToUserResponseDto());
        }

        [Theory]
        [InlineData("1")]
        [InlineData("7E89EECB-7A95-48D6-A63B-FE6A4D7588F8")]
        public async Task GetCoursesOfStudent_InvalidId_ReturnsError(string id)
        {
            // Arrange.

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, []);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, []);
            mockDbContext.CreateDbSetMock(x => x.Roles, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            // Act.

            Result<List<CourseResponseDto>> result = await courseService.GetCoursesOfStudentAsync(id);

            // Assert.

            TestError<StudentDoesNotExistError>(result);
        }

        [Fact]
        public async Task GetCoursesOfStudent_ReturnsCourses()
        {
            // Arrange.

            User student = UserFixture().Create();
            Role studentRole = RoleFixture().With(x => x.Name, Role.STUDENT).Create();
            UserRole userRole = new() { RoleId = studentRole.Id, UserId = student.Id };

            Course course1 = CourseFixture().Create();
            Course course2 = CourseFixture().Create();
            Course course3 = CourseFixture().Create();

            CourseEnrollment courseEnrollment1 = new() { CourseId = course1.Id, StudentId = student.Id };
            CourseEnrollment courseEnrollment2 = new() { CourseId = course2.Id, StudentId = student.Id };

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [student]);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, [userRole]);
            mockDbContext.CreateDbSetMock(x => x.Roles, [studentRole]);
            mockDbContext.CreateDbSetMock(x => x.Courses, [course1, course2, course3]);
            mockDbContext.CreateDbSetMock(x => x.CourseEnrollments, [courseEnrollment1, courseEnrollment2]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            CourseService courseService = GetCourseService(dbContext);

            // Act.

            Result<List<CourseResponseDto>> result = await courseService.GetCoursesOfStudentAsync(student.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            List<CourseResponseDto> responses = result.Value!;
            responses.Should().HaveCount(2);

            CourseResponseDto? courseResponse1 = responses.FirstOrDefault(x => x.Id == course1.Id.ToString());
            courseResponse1.Should().NotBeNull();
            courseResponse1.Should().BeEquivalentTo(course1.ToCourseResponseDto());

            CourseResponseDto? courseResponse2 = responses.FirstOrDefault(x => x.Id == course2.Id.ToString());
            courseResponse2.Should().NotBeNull();
            courseResponse2.Should().BeEquivalentTo(course2.ToCourseResponseDto());
        }
    }
}
