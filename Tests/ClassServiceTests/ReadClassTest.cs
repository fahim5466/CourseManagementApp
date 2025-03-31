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
using static Application.Errors.ClassErrors;
using static Application.Errors.UserErrors;
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

        [Theory]
        [InlineData("1")]
        [InlineData("7E89EECB-7A95-48D6-A63B-FE6A4D7588F8")]
        public async Task GetCoursesOfClass_InvalidId_ReturnsError(string id)
        {
            // Arrange.

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Classes, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<List<CourseResponseDto>> result = await classService.GetCoursesOfClassAsync(id);

            // Assert.

            TestError<ClassDoesNotExistError>(result);
        }

        [Fact]
        public async Task GetCoursesOfClass_ReturnsCourses()
        {
            // Arrange.

            Class clss = ClassFixture().With(x => x.Id, Guid.NewGuid()).Create();

            Course course1 = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();
            Course course2 = CourseFixture().With(x => x.Id, Guid.NewGuid()).Create();

            clss.Courses = [course1, course2];

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Courses, [course1, course2]);
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<List<CourseResponseDto>> result = await classService.GetCoursesOfClassAsync(clss.Id.ToString());

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

        [Theory]
        [InlineData("1")]
        [InlineData("7E89EECB-7A95-48D6-A63B-FE6A4D7588F8")]
        public async Task GetStudentsOfClass_InvalidId_ReturnsError(string id)
        {
            // Arrange.

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Classes, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<List<UserResponseDto>> result = await classService.GetStudentsOfClassAsync(id);

            // Assert.

            TestError<ClassDoesNotExistError>(result);
        }

        [Fact]
        public async Task GetStudentsOfClass_ReturnsStudents()
        {
            // Arrange.

            User student1 = UserFixture().Create();
            User student2 = UserFixture().Create();
            User student3 = UserFixture().Create();
            User student4 = UserFixture().Create();

            Class clss = ClassFixture().Create();

            Course course1 = CourseFixture().Create();
            Course course2 = CourseFixture().Create();

            CourseClass courseClass = new() { CourseId = course1.Id, ClassId = clss.Id };

            ClassEnrollment classEnrollment1 = new() { ClassId = clss.Id, StudentId = student1.Id };
            ClassEnrollment classEnrollment2 = new() { ClassId = clss.Id, StudentId = student2.Id };

            CourseEnrollment courseEnrollment1 = new() { CourseId = course1.Id, StudentId = student2.Id };
            CourseEnrollment courseEnrollment2 = new() { CourseId = course1.Id, StudentId = student3.Id };

            CourseEnrollment courseEnrollment3 = new() { CourseId = course2.Id, StudentId = student3.Id };
            CourseEnrollment courseEnrollment4 = new() { CourseId = course2.Id, StudentId = student4.Id };

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [student1, student2, student3, student4]);
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss]);
            mockDbContext.CreateDbSetMock(x => x.Courses, [course1, course2]);
            mockDbContext.CreateDbSetMock(x => x.CourseClasses, [courseClass]);
            mockDbContext.CreateDbSetMock(x => x.ClassEnrollments, [classEnrollment1, classEnrollment2]);
            mockDbContext.CreateDbSetMock(x => x.CourseEnrollments, [courseEnrollment1, courseEnrollment2, courseEnrollment3, courseEnrollment4]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<List<UserResponseDto>> result = await classService.GetStudentsOfClassAsync(clss.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            List<UserResponseDto> responses = result.Value!;
            responses.Should().HaveCount(3);

            UserResponseDto? studentResponse1 = responses.FirstOrDefault(x => x.Id == student1.Id.ToString());
            studentResponse1.Should().NotBeNull();
            studentResponse1.Should().BeEquivalentTo(student1.ToUserResponseDto());

            UserResponseDto? studentResponse2 = responses.FirstOrDefault(x => x.Id == student2.Id.ToString());
            studentResponse2.Should().NotBeNull();
            studentResponse2.Should().BeEquivalentTo(student2.ToUserResponseDto());

            UserResponseDto? studentResponse3 = responses.FirstOrDefault(x => x.Id == student3.Id.ToString());
            studentResponse3.Should().NotBeNull();
            studentResponse3.Should().BeEquivalentTo(student3.ToUserResponseDto());
        }

        [Theory]
        [InlineData("1")]
        [InlineData("7E89EECB-7A95-48D6-A63B-FE6A4D7588F8")]
        public async Task GetClassesOfStudent_InvalidId_ReturnsError(string id)
        {
            // Arrange.

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, []);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, []);
            mockDbContext.CreateDbSetMock(x => x.Roles, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<List<ClassResponseDto>> result = await classService.GetClassesOfStudentAsync(id);

            // Assert.

            TestError<StudentDoesNotExistError>(result);
        }

        [Fact]
        public async Task GetClassesOfStudent_ReturnsClasses()
        {
            // Arrange.

            User student = UserFixture().Create();
            Role studentRole = RoleFixture().With(x => x.Name, Role.STUDENT).Create();
            UserRole userRole = new() { RoleId = studentRole.Id, UserId = student.Id };

            Class clss1 = ClassFixture().Create();
            Class clss2 = ClassFixture().Create();
            Class clss3 = ClassFixture().Create();
            Class clss4 = ClassFixture().Create();

            Course course1 = CourseFixture().Create();
            Course course2 = CourseFixture().Create();

            CourseClass courseClass1 = new() { CourseId = course1.Id, ClassId = clss2.Id };
            CourseClass courseClass2 = new() { CourseId = course1.Id, ClassId = clss3.Id };

            CourseClass courseClass3 = new() { CourseId = course2.Id, ClassId = clss3.Id };
            CourseClass courseClass4 = new() { CourseId = course2.Id, ClassId = clss4.Id };

            ClassEnrollment classEnrollment1 = new() { ClassId = clss1.Id, StudentId = student.Id };
            ClassEnrollment classEnrollment2 = new() { ClassId = clss2.Id, StudentId = student.Id };

            CourseEnrollment courseEnrollment = new() { CourseId = course1.Id, StudentId = student.Id };

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [student]);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, [userRole]);
            mockDbContext.CreateDbSetMock(x => x.Roles, [studentRole]);
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss1, clss2, clss3, clss4]);
            mockDbContext.CreateDbSetMock(x => x.Courses, [course1, course2]);
            mockDbContext.CreateDbSetMock(x => x.CourseClasses, [courseClass1, courseClass2, courseClass3, courseClass4]);
            mockDbContext.CreateDbSetMock(x => x.ClassEnrollments, [classEnrollment1, classEnrollment2]);
            mockDbContext.CreateDbSetMock(x => x.CourseEnrollments, [courseEnrollment]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<List<ClassResponseDto>> result = await classService.GetClassesOfStudentAsync(student.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            List<ClassResponseDto> responses = result.Value!;
            responses.Should().HaveCount(3);

            ClassResponseDto? classResponse1 = responses.FirstOrDefault(x => x.Id == clss1.Id.ToString());
            classResponse1.Should().NotBeNull();
            classResponse1.Should().BeEquivalentTo(clss1.ToClassResponseDto());

            ClassResponseDto? classResponse2 = responses.FirstOrDefault(x => x.Id == clss2.Id.ToString());
            classResponse2.Should().NotBeNull();
            classResponse2.Should().BeEquivalentTo(clss2.ToClassResponseDto());

            ClassResponseDto? classResponse3 = responses.FirstOrDefault(x => x.Id == clss3.Id.ToString());
            classResponse3.Should().NotBeNull();
            classResponse3.Should().BeEquivalentTo(clss3.ToClassResponseDto());
        }
    }
}
