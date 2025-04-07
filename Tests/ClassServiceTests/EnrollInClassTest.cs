using Application.DTOs.Class;
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
using static Application.Errors.ClassErrors;
using static Application.Errors.UserErrors;
using static Application.Helpers.ResultHelper;
using static Tests.ClassServiceTests.ClassServiceTestHelper;
using static Tests.Helpers.TestHelper;

namespace Tests.ClassServiceTests
{
    public class EnrollInClassTest
    {
        [Theory]
        [InlineData("1", "885A60D5-A62E-46C4-95D6-E4684B84EF6B", 1)]
        [InlineData("885A60D5-A62E-46C4-95D6-E4684B84EF6B", "885A60D5-A62E-46C4-95D6-E4684B84EF6B", 2)]
        [InlineData("A27F997A-AC4B-4F2B-AB1E-72DE073DAA52", "1", 3)]
        [InlineData("A27F997A-AC4B-4F2B-AB1E-72DE073DAA52", "A27F997A-AC4B-4F2B-AB1E-72DE073DAA52", 4)]
        public async Task EnrollStudentInClass_InvalidIds_ReturnsError(string classId, string studentId, int caseNo)
        {
            // Arrange.

            Class clss = ClassFixture().With(x => x.Id, Guid.Parse("A27F997A-AC4B-4F2B-AB1E-72DE073DAA52")).Create();
            User student = UserFixture().With(x => x.Id, Guid.Parse("885A60D5-A62E-46C4-95D6-E4684B84EF6B")).Create();
            Role studentRole = RoleFixture().With(x => x.Id, Guid.NewGuid())
                                            .With(x => x.Name, Role.STUDENT)
                                            .Create();
            UserRole userRole = new() { UserId = student.Id, RoleId = studentRole.Id };

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Classes).ReturnsDbSet([clss]);
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([student]);
            mockDbContext.Setup(x => x.Roles).ReturnsDbSet( [studentRole]);
            mockDbContext.Setup(x => x.UserRoles).ReturnsDbSet([userRole]);
            mockDbContext.Setup(x => x.ClassEnrollments).ReturnsDbSet([]);

            ApplicationDbContext dbContext = mockDbContext.Object;
            ClassService classService = GetClassService(dbContext);

            ClassEnrollmentRequestDto request = new() { ClassId = classId, StudentId = studentId };

            // Act.

            Result result = await classService.EnrollStudentInClassAsync(request);

            // Assert.

            if(caseNo == 1 || caseNo == 2)
            {
                TestError<ClassDoesNotExistError>(result);
            }
            else
            {
                TestError<StudentDoesNotExistError>(result);
            }
        }

        [Fact]
        public async Task EnrollStudentInClass_AlreadyEnrolled_ReturnsError()
        {
            // Arrange.

            Class clss = ClassFixture().With(x => x.Id, Guid.NewGuid()).Create();
            User student = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();
            Role studentRole = RoleFixture().With(x => x.Id, Guid.NewGuid())
                                            .With(x => x.Name, Role.STUDENT)
                                            .Create();
            UserRole userRole = new() { UserId = student.Id, RoleId = studentRole.Id };
            ClassEnrollment classEnrollment = new() { ClassId = clss.Id, StudentId = student.Id };

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Classes).ReturnsDbSet([clss]);
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([student]);
            mockDbContext.Setup(x => x.Roles).ReturnsDbSet([studentRole]);
            mockDbContext.Setup(x => x.UserRoles).ReturnsDbSet([userRole]);
            mockDbContext.Setup(x => x.ClassEnrollments).ReturnsDbSet([classEnrollment]);

            ApplicationDbContext dbContext = mockDbContext.Object;
            ClassService classService = GetClassService(dbContext);

            ClassEnrollmentRequestDto request = new() { ClassId = clss.Id.ToString(), StudentId = student.Id.ToString() };

            // Act.

            Result result = await classService.EnrollStudentInClassAsync(request);

            // Assert.

            TestError<StudentAlreadyEnrolledInClassError>(result);

        }

        [Fact]
        public async Task EnrollStudentInClass_ValidRequest_CreatesEnrollment()
        {
            // Arrange.

            Class clss = ClassFixture().With(x => x.Id, Guid.NewGuid()).Create();
            User student = UserFixture().With(x => x.Id, Guid.NewGuid()).Create();
            Role studentRole = RoleFixture().With(x => x.Id, Guid.NewGuid())
                                            .With(x => x.Name, Role.STUDENT)
                                            .Create();
            UserRole userRole = new() { UserId = student.Id, RoleId = studentRole.Id};

            Mock<ApplicationDbContext> mockDbContext = MockDependencyHelper.GetMockDbContext();
            mockDbContext.Setup(x => x.Classes).ReturnsDbSet([clss]);
            mockDbContext.Setup(x => x.Users).ReturnsDbSet([student]);
            mockDbContext.Setup(x => x.Roles).ReturnsDbSet([studentRole]);
            mockDbContext.Setup(x => x.UserRoles).ReturnsDbSet([userRole]);
            mockDbContext.Setup(x => x.ClassEnrollments).ReturnsDbSet([]);

            ApplicationDbContext dbContext = mockDbContext.Object;
            ClassRepository classRepository = new(dbContext);
            ClassService classService = GetClassService(dbContext);

            ClassEnrollmentRequestDto request = new() { ClassId = clss.Id.ToString(), StudentId = student.Id.ToString() };

            // Act.

            Result result = await classService.EnrollStudentInClassAsync(request);

            // Assert.

            TestSuccess(result);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            ClassEnrollment? classEnrollment = await classRepository.GetClassEnrollmentAsync(clss.Id, student.Id);
            classEnrollment.Should().NotBeNull();
            classEnrollment.ClassId.Should().Be(clss.Id);
            classEnrollment.StudentId.Should().Be(student.Id);
        }
    }
}
