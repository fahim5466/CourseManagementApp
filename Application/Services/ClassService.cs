using Application.DTOs;
using Application.DTOs.Class;
using Application.DTOs.Course;
using Application.DTOs.Enrollment;
using Application.DTOs.User;
using Domain.Entities;
using Domain.Relationships;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using static Application.Errors.ClassErrors;
using static Application.Errors.UserErrors;
using static Application.Helpers.ResultHelper;
using static Application.Helpers.ValidationHelper;

namespace Application.Services
{
    public interface IClassService
    {
        public Task<Result<ClassResponseDto>> GetClassByIdAsync(string id);
        public Task<Result<List<ClassResponseDto>>> GetAllClassesAsync();
        public Task<Result<ClassResponseDto>> CreateClassAsync(ClassRequestDto request);
        public Task<Result<ClassResponseDto>> UpdateClassAsync(string id, ClassRequestDto request);
        public Task<Result> DeleteClassAsync(string id);
        public Task<Result> EnrollStudentInClassAsync(ClassEnrollmentRequestDto request);
        public Task<Result<List<CourseResponseDto>>> GetCoursesOfClassAsync(string id);
        public Task<Result<List<UserResponseDto>>> GetStudentsOfClassAsync(string id);
        public Task<Result<List<string>>> GetOtherStudentNamesOfClassAsync(string studentId, string classId);
        public Task<Result<List<ClassResponseDto>>> GetClassesOfStudentAsync(string id);
        public Task<Result<EnrollmentInfoResponseDto>> GetClassEnrollmentInfoForStudentAsync(string classId, string studentId);
    }

    public class ClassService(IClassRepository classRepository, ICourseRepository courseRepository, IUserRepository userRepository, IUnitOfWork unitOfWork) : IClassService
    {
        public async Task<Result<ClassResponseDto>> GetClassByIdAsync(string id)
        {
            Class? clss = await classRepository.GetClassByIdAsync(id);

            // Class not found.
            if (clss is null)
            {
                return Result<ClassResponseDto>.Failure(new ClassDoesNotExistError());
            }

            return Result<ClassResponseDto>.Success(StatusCodes.Status200OK, clss.ToClassResponseDto());
        }

        public async Task<Result<List<ClassResponseDto>>> GetAllClassesAsync()
        {
            List<Class> classes = await classRepository.GetAllClassesAsync();

            return Result<List<ClassResponseDto>>.Success(StatusCodes.Status200OK,
                        classes.Select(c => c.ToClassResponseDto()).ToList());
        }

        public async Task<Result<ClassResponseDto>> CreateClassAsync(ClassRequestDto request)
        {
            // Validate request.
            ValidationOutcome validationOutcome = Validate(request);
            if(!validationOutcome.IsValid)
            {
                return Result<ClassResponseDto>.Failure(new BadClassCreateOrUpdateRequest(validationOutcome.Errors));
            }

            // Class name should be unique.
            Class? existingClass = await classRepository.GetClassByNameAsync(request.Name);
            if(existingClass is not null)
            {
                return Result<ClassResponseDto>.Failure(new ClassAlreadyExistsError());
            }

            Class newClass = new() { Id = Guid.NewGuid(), Name = request.Name };
            await classRepository.CreateClassAsync(newClass);

            return Result<ClassResponseDto>.Success(StatusCodes.Status201Created, newClass.ToClassResponseDto());
        }

        public async Task<Result<ClassResponseDto>> UpdateClassAsync(string id, ClassRequestDto request)
        {
            // Validate request.
            ValidationOutcome validationOutcome = Validate(request);
            if (!validationOutcome.IsValid)
            {
                return Result<ClassResponseDto>.Failure(new BadClassCreateOrUpdateRequest(validationOutcome.Errors));
            }

            // Class not found.
            Class? clss = await classRepository.GetClassByIdAsync(id);
            if (clss is null)
            {
                return Result<ClassResponseDto>.Failure(new ClassDoesNotExistError());
            }

            // New name should be unique.
            Class? existingClassWithSameName = await classRepository.GetClassByNameAsync(request.Name, id);
            if (existingClassWithSameName is not null)
            {
                return Result<ClassResponseDto>.Failure(new ClassAlreadyExistsError());
            }

            clss.Name = request.Name;
            await unitOfWork.SaveChangesAsync();

            return Result<ClassResponseDto>.Success(StatusCodes.Status200OK, clss.ToClassResponseDto());
        }

        public async Task<Result> DeleteClassAsync(string id)
        {
            // Class not found.
            Class? clss = await classRepository.GetClassByIdAsync(id);
            if (clss is null)
            {
                return Result<ClassResponseDto>.Failure(new ClassDoesNotExistError());
            }

            await classRepository.DeleteClassAsync(clss);

            return Result.Success(StatusCodes.Status204NoContent);
        }

        public async Task<Result> EnrollStudentInClassAsync(ClassEnrollmentRequestDto request)
        {
            // Class should exist.
            Class? clss = await classRepository.GetClassByIdAsync(request.ClassId);
            if(clss is null)
            {
                return Result.Failure(new ClassDoesNotExistError());
            }

            // Student should exist.
            User? student = await userRepository.GetStudentByIdAsync(request.StudentId);
            if (student is null)
            {
                return Result.Failure(new StudentDoesNotExistError());
            }

            Guid classGuid = Guid.Empty;
            Guid.TryParse(request.ClassId, out classGuid);

            Guid studentGuid = Guid.Empty;
            Guid.TryParse(request.StudentId, out studentGuid);

            // Student should not be enrolled already.
            ClassEnrollment? classEnrollment = await classRepository.GetClassEnrollmentAsync(classGuid, studentGuid);
            if(classEnrollment is not null)
            {
                return Result.Failure(new StudentAlreadyEnrolledInClassError());
            }

            await classRepository.CreateClassEnrollmentAsync(new()
            {
                ClassId = classGuid,
                StudentId = studentGuid
            });

            return Result.Success(StatusCodes.Status200OK);
        }

        public async Task<Result<List<CourseResponseDto>>> GetCoursesOfClassAsync(string id)
        {
            Class? clss = await classRepository.GetClassByIdWithCoursesAsync(id);

            // Class should exist.
            if(clss is null)
            {
                return Result<List<CourseResponseDto>>.Failure(new ClassDoesNotExistError());
            }

            return Result<List<CourseResponseDto>>.Success(StatusCodes.Status200OK, clss.Courses.Select(c => c.ToCourseResponseDto()).ToList());
        }

        public async Task<Result<List<UserResponseDto>>> GetStudentsOfClassAsync(string id)
        {
            Class? clss = await classRepository.GetClassByIdAsync(id);

            // Class should exist.
            if (clss is null)
            {
                return Result<List<UserResponseDto>>.Failure(new ClassDoesNotExistError());
            }

            List<User> students = await classRepository.GetStudentsOfClassAsync(id);

            return Result<List<UserResponseDto>>.Success(StatusCodes.Status200OK, students.Select(s => s.ToUserResponseDto()).ToList());
        }

        public async Task<Result<List<string>>> GetOtherStudentNamesOfClassAsync(string studentId, string classId)
        {
            // Student should exist.
            User? student = await userRepository.GetStudentByIdAsync(studentId);
            if (student is null)
            {
                return Result<List<string>>.Failure(new StudentDoesNotExistError());
            }

            // Class should exist.
            Class? clss = await classRepository.GetClassByIdAsync(classId);
            
            if (clss is null)
            {
                return Result<List<string>>.Failure(new ClassDoesNotExistError());
            }

            List<User> students = await classRepository.GetStudentsOfClassAsync(classId);
                
            // Student should be enrolled in the class.
            if(!students.Any(s => s.Id.ToString() == studentId))
            {
                return Result<List<string>>.Failure(new StudentNotEnrolledInClassError());
            }

            return Result<List<string>>.Success(StatusCodes.Status200OK,
                                                students.Where(s => s.Id.ToString() != studentId)
                                                        .Select(s => s.Name)
                                                        .ToList());
        }

        public async Task<Result<List<ClassResponseDto>>> GetClassesOfStudentAsync(string id)
        {
            User? student = await userRepository.GetStudentByIdAsync(id);

            // Student should exist.
            if(student is null)
            {
                return Result<List<ClassResponseDto>>.Failure(new StudentDoesNotExistError());
            }

            List<Class> classes = await classRepository.GetClassesOfStudentAsync(id);

            return Result<List<ClassResponseDto>>.Success(StatusCodes.Status200OK, classes.Select(c => c.ToClassResponseDto()).ToList());
        }

        public async Task<Result<EnrollmentInfoResponseDto>> GetClassEnrollmentInfoForStudentAsync(string classId, string studentId)
        {
            // Class should exist.
            Class? clss = await classRepository.GetClassByIdAsync(classId);
            if(clss is null)
            {
                return Result<EnrollmentInfoResponseDto>.Failure(new ClassDoesNotExistError());
            }

            // Student should exist.
            User? student = await userRepository.GetStudentByIdAsync(studentId);
            if(student is null)
            {
                return Result<EnrollmentInfoResponseDto>.Failure(new StudentDoesNotExistError());
            }

            Guid classGuid = Guid.Parse(classId);
            Guid studentGuid = Guid.Parse(studentId);

            // Student may be enrolled directly in this class.
            ClassEnrollment? classEnrollment = await classRepository.GetClassEnrollmentAsync(classGuid, studentGuid);

            // Student may be enrolled indirectly in this class through a course.
            List<CourseEnrollment> courseEnrollments = await courseRepository.GetCourseEnrollmentsByClassAndStudentAsync(classGuid, studentGuid);

            bool isEnrolled = classEnrollment != null || courseEnrollments.Count > 0;

            ClassEnrollmentInfoDto? classEnrollmentInfoDto = classEnrollment?.ToClassEnrollmentInfoDto();

            List<CourseEnrollmentInfoDto> courseEnrollmentInfoDtos = courseEnrollments.Select(ce => ce.ToCourseEnrollmentInfoDto()).ToList();

            return Result<EnrollmentInfoResponseDto>.Success(StatusCodes.Status200OK,
                            new EnrollmentInfoResponseDto()
                            {
                                IsEnrolled = isEnrolled,
                                DirectEnrollment = classEnrollmentInfoDto,
                                IndirectEnrollmentsThroughCourses = courseEnrollmentInfoDtos
                            });
        }
    }
}
