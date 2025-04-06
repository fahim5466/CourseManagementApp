using Application.DTOs;
using Application.DTOs.Class;
using Application.DTOs.Course;
using Application.DTOs.Enrollment;
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
    public class ClassEnrollmentInfoTest
    {
        [Theory]
        [InlineData("1")]
        [InlineData("7E89EECB-7A95-48D6-A63B-FE6A4D7588F8")]
        public async Task GetEnrollmentInfoForStudent_InvalidClassId_ReturnsError(string classId)
        {
            // Arrange.

            User student = UserFixture().Create();
            Role studentRole = RoleFixture().With(x => x.Name, Role.STUDENT).Create();
            UserRole userRole = new() { RoleId = studentRole.Id, UserId = student.Id };

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [student]);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, [userRole]);
            mockDbContext.CreateDbSetMock(x => x.Roles, [studentRole]);
            mockDbContext.CreateDbSetMock(x => x.Classes, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<EnrollmentInfoResponseDto> result = await classService.GetClassEnrollmentInfoForStudentAsync(classId, student.Id.ToString());

            // Assert.

            TestError<ClassDoesNotExistError>(result);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("7E89EECB-7A95-48D6-A63B-FE6A4D7588F8")]
        public async Task GetEnrollmentInfoForStudent_InvalidStudentId_ReturnsError(string studentId)
        {
            // Arrange.

            User student = UserFixture().Create();
            Role studentRole = RoleFixture().With(x => x.Name, Role.STUDENT).Create();
            UserRole userRole = new() { RoleId = studentRole.Id, UserId = student.Id };

            Class clss = ClassFixture().Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [student]);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, [userRole]);
            mockDbContext.CreateDbSetMock(x => x.Roles, [studentRole]);
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<EnrollmentInfoResponseDto> result = await classService.GetClassEnrollmentInfoForStudentAsync(clss.Id.ToString(), studentId);

            // Assert.

            TestError<StudentDoesNotExistError>(result);
        }

        [Fact]
        public async Task GetEnrollmentInfoForStudent_StudentNotEnrolled_ReturnsEnrollmentInfo()
        {
            // Arrange.

            User student = UserFixture().Create();
            Role studentRole = RoleFixture().With(x => x.Name, Role.STUDENT).Create();
            UserRole userRole = new() { RoleId = studentRole.Id, UserId = student.Id };

            Class clss = ClassFixture().Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [student]);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, [userRole]);
            mockDbContext.CreateDbSetMock(x => x.Roles, [studentRole]);
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss]);
            mockDbContext.CreateDbSetMock(x => x.Courses, []);
            mockDbContext.CreateDbSetMock(x => x.CourseClasses, []);
            mockDbContext.CreateDbSetMock(x => x.ClassEnrollments, []);
            mockDbContext.CreateDbSetMock(x => x.CourseEnrollments, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<EnrollmentInfoResponseDto> result = await classService.GetClassEnrollmentInfoForStudentAsync(clss.Id.ToString(), student.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            EnrollmentInfoResponseDto? response = result.Value;
            response.Should().NotBeNull();

            response.IsEnrolled.Should().BeFalse();
            response.DirectEnrollment.Should().BeNull();
            response.IndirectEnrollmentsThroughCourses.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetEnrollmentInfoForStudent_StudentEnrolledDirectlyInClass_ReturnsEnrollmentInfo()
        {
            // Arrange.

            User student = UserFixture().Create();
            Role studentRole = RoleFixture().With(x => x.Name, Role.STUDENT).Create();
            UserRole userRole = new() { RoleId = studentRole.Id, UserId = student.Id };

            Class clss = ClassFixture().Create();

            ClassEnrollment classEnrollment = ClassEnrollmentFixture().With(x => x.ClassId, clss.Id)
                                                                      .With(x => x.StudentId, student.Id)
                                                                      .Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [student]);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, [userRole]);
            mockDbContext.CreateDbSetMock(x => x.Roles, [studentRole]);
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss]);
            mockDbContext.CreateDbSetMock(x => x.Courses, []);
            mockDbContext.CreateDbSetMock(x => x.CourseClasses, []);
            mockDbContext.CreateDbSetMock(x => x.ClassEnrollments, [classEnrollment]);
            mockDbContext.CreateDbSetMock(x => x.CourseEnrollments, []);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<EnrollmentInfoResponseDto> result = await classService.GetClassEnrollmentInfoForStudentAsync(clss.Id.ToString(), student.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            EnrollmentInfoResponseDto? response = result.Value;
            response.Should().NotBeNull();

            response.IsEnrolled.Should().BeTrue();

            response.DirectEnrollment.Should().NotBeNull();
            response.DirectEnrollment.CreatedBy.Should().Be(classEnrollment.CreatedBy);
            response.DirectEnrollment.CreatedOn.Should().Be(classEnrollment.CreatedOn);

            response.IndirectEnrollmentsThroughCourses.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetEnrollmentInfoForStudent_StudentEnrolledInDirectlyThroughCourses_ReturnsEnrollmentInfo()
        {
            // Arrange.

            User student = UserFixture().Create();
            Role studentRole = RoleFixture().With(x => x.Name, Role.STUDENT).Create();
            UserRole userRole = new() { RoleId = studentRole.Id, UserId = student.Id };

            Class clss = ClassFixture().Create();

            Course course1 = CourseFixture().Create();
            Course course2 = CourseFixture().Create();

            CourseClass courseClass1 = new() { CourseId = course1.Id, ClassId = clss.Id };
            CourseClass courseClass2 = new() { CourseId = course2.Id, ClassId = clss.Id };

            CourseEnrollment courseEnrollment1 = CourseEnrollmentFixture().With(x => x.CourseId, course1.Id)
                                                                          .With(x => x.StudentId, student.Id)
                                                                          .Create();

            CourseEnrollment courseEnrollment2 = CourseEnrollmentFixture().With(x => x.CourseId, course2.Id)
                                                                          .With(x => x.StudentId, student.Id)
                                                                          .Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [student]);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, [userRole]);
            mockDbContext.CreateDbSetMock(x => x.Roles, [studentRole]);
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss]);
            mockDbContext.CreateDbSetMock(x => x.Courses, [course1, course2]);
            mockDbContext.CreateDbSetMock(x => x.CourseClasses, [courseClass1, courseClass2]);
            mockDbContext.CreateDbSetMock(x => x.ClassEnrollments, []);
            mockDbContext.CreateDbSetMock(x => x.CourseEnrollments, [courseEnrollment1, courseEnrollment2]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<EnrollmentInfoResponseDto> result = await classService.GetClassEnrollmentInfoForStudentAsync(clss.Id.ToString(), student.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            EnrollmentInfoResponseDto? response = result.Value;
            response.Should().NotBeNull();

            response.IsEnrolled.Should().BeTrue();
            response.DirectEnrollment.Should().BeNull();
            response.IndirectEnrollmentsThroughCourses.Should().HaveCount(2);

            CourseEnrollmentInfoDto? info1 = response.IndirectEnrollmentsThroughCourses.FirstOrDefault(x => x.CourseId == course1.Id);
            info1.Should().NotBeNull();
            info1.CreatedBy.Should().Be(courseEnrollment1.CreatedBy);
            info1.CreatedOn.Should().Be(courseEnrollment1.CreatedOn);

            CourseEnrollmentInfoDto? info2 = response.IndirectEnrollmentsThroughCourses.FirstOrDefault(x => x.CourseId == course2.Id);
            info2.Should().NotBeNull();
            info2.CreatedBy.Should().Be(courseEnrollment2.CreatedBy);
            info2.CreatedOn.Should().Be(courseEnrollment2.CreatedOn);
        }

        [Fact]
        public async Task GetEnrollmentInfoForStudent_StudentEnrolledBothDirectlyAndIndirectly_ReturnsEnrollmentInfo()
        {
            // Arrange.

            User student = UserFixture().Create();
            Role studentRole = RoleFixture().With(x => x.Name, Role.STUDENT).Create();
            UserRole userRole = new() { RoleId = studentRole.Id, UserId = student.Id };

            Class clss = ClassFixture().Create();

            ClassEnrollment classEnrollment = ClassEnrollmentFixture().With(x => x.ClassId, clss.Id)
                                                                      .With(x => x.StudentId, student.Id)
                                                                      .Create();

            Course course1 = CourseFixture().Create();
            Course course2 = CourseFixture().Create();

            CourseClass courseClass1 = new() { CourseId = course1.Id, ClassId = clss.Id };
            CourseClass courseClass2 = new() { CourseId = course2.Id, ClassId = clss.Id };

            CourseEnrollment courseEnrollment1 = CourseEnrollmentFixture().With(x => x.CourseId, course1.Id)
                                                                          .With(x => x.StudentId, student.Id)
                                                                          .Create();

            CourseEnrollment courseEnrollment2 = CourseEnrollmentFixture().With(x => x.CourseId, course2.Id)
                                                                          .With(x => x.StudentId, student.Id)
                                                                          .Create();

            DbContextMock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.CreateDbSetMock(x => x.Users, [student]);
            mockDbContext.CreateDbSetMock(x => x.UserRoles, [userRole]);
            mockDbContext.CreateDbSetMock(x => x.Roles, [studentRole]);
            mockDbContext.CreateDbSetMock(x => x.Classes, [clss]);
            mockDbContext.CreateDbSetMock(x => x.Courses, [course1, course2]);
            mockDbContext.CreateDbSetMock(x => x.CourseClasses, [courseClass1, courseClass2]);
            mockDbContext.CreateDbSetMock(x => x.ClassEnrollments, [classEnrollment]);
            mockDbContext.CreateDbSetMock(x => x.CourseEnrollments, [courseEnrollment1, courseEnrollment2]);
            ApplicationDbContext dbContext = mockDbContext.Object;

            ClassService classService = GetClassService(dbContext);

            // Act.

            Result<EnrollmentInfoResponseDto> result = await classService.GetClassEnrollmentInfoForStudentAsync(clss.Id.ToString(), student.Id.ToString());

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            EnrollmentInfoResponseDto? response = result.Value;
            response.Should().NotBeNull();

            response.IsEnrolled.Should().BeTrue();
            
            response.DirectEnrollment.Should().NotBeNull();
            response.DirectEnrollment.CreatedBy.Should().Be(classEnrollment.CreatedBy);
            response.DirectEnrollment.CreatedOn.Should().Be(classEnrollment.CreatedOn);

            response.IndirectEnrollmentsThroughCourses.Should().HaveCount(2);

            CourseEnrollmentInfoDto? info1 = response.IndirectEnrollmentsThroughCourses.FirstOrDefault(x => x.CourseId == course1.Id);
            info1.Should().NotBeNull();
            info1.CreatedBy.Should().Be(courseEnrollment1.CreatedBy);
            info1.CreatedOn.Should().Be(courseEnrollment1.CreatedOn);

            CourseEnrollmentInfoDto? info2 = response.IndirectEnrollmentsThroughCourses.FirstOrDefault(x => x.CourseId == course2.Id);
            info2.Should().NotBeNull();
            info2.CreatedBy.Should().Be(courseEnrollment2.CreatedBy);
            info2.CreatedOn.Should().Be(courseEnrollment2.CreatedOn);
        }
    }
}
