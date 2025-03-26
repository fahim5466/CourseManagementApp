using Application.DTOs.Course;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using static Application.Errors.ClassErrors;
using static Application.Errors.CourseErrors;
using static Application.Helpers.ResultHelper;
using static Application.Helpers.ValidationHelper;

namespace Application.Services
{
    public interface ICourseService
    {
        public Task<Result<CourseResponseDto>> GetCourseByIdAsync(string id);
        public Task<Result<List<CourseResponseDto>>> GetAllCoursesAsync();
        public Task<Result> CreateCourseAsync(CourseRequestDto request);
        public Task<Result> UpdateCourseAsync(string id, CourseRequestDto request);
    }

    public class CourseService(ICourseRepository courseRepository, IClassRepository classRepository, IUnitOfWork unitOfWork) : ICourseService
    {

        public async Task<Result<CourseResponseDto>> GetCourseByIdAsync(string id)
        {
            Course? course = await courseRepository.GetCourseByIdWithClassesAsync(id);

            // Course does not exist.
            if(course is null)
            {
                return Result<CourseResponseDto>.Failure(new CourseDoesNotExistError());
            }

            return Result<CourseResponseDto>.Success(StatusCodes.Status200OK,
                            new() { Id = course.Id.ToString(), Name = course.Name, ClassNames = course.Classes.Select(c => c.Name).ToList() });
        }

        public async Task<Result<List<CourseResponseDto>>> GetAllCoursesAsync()
        {
            List<Course> courses = await courseRepository.GetAllCoursesAsync();

            return Result<List<CourseResponseDto>>.Success(StatusCodes.Status200OK,
                    courses.Select(course => 
                            new CourseResponseDto()
                            { 
                                Id = course.Id.ToString(),
                                Name = course.Name,
                                ClassNames = course.Classes.Select(c => c.Name).ToList() 
                            })
                         .ToList());
        }

        public async Task<Result> CreateCourseAsync(CourseRequestDto request)
        {
            // Validate request.
            ValidationOutcome validationOutcome = Validate(request);
            if (!validationOutcome.IsValid)
            {
                return Result.Failure(new BadCourseCreateOrUpdateRequest(validationOutcome.Errors));
            }

            // Course name should be unique.
            Course? existingCourse = await courseRepository.GetCourseByNameAsync(request.Name);
            if (existingCourse is not null)
            {
                return Result.Failure(new CourseAlreadyExistsError());
            }

            // Class ids should be valid.
            bool areClassIdsValid = await classRepository.AreClassIdsValidAsync(request.ClassIds);
            if (!areClassIdsValid)
            {
                return Result.Failure(new InvalidClassIdsError());
            }

            List<Class> classes = await classRepository.GetClassesByIdAsync(request.ClassIds);
            Course course = new() { Id = Guid.NewGuid(), Name = request.Name, Classes = classes };
            await courseRepository.CreateCourseAsync(course);

            return Result.Success(StatusCodes.Status201Created);
        }

        public async Task<Result> UpdateCourseAsync(string id, CourseRequestDto request)
        {
            // Validate request.
            ValidationOutcome validationOutcome = Validate(request);
            if (!validationOutcome.IsValid)
            {
                return Result.Failure(new BadCourseCreateOrUpdateRequest(validationOutcome.Errors));
            }

            // Course does not exist.
            Course? course = await courseRepository.GetCourseByIdWithClassesAsync(id);
            if (course is null)
            {
                return Result.Failure(new CourseDoesNotExistError());
            }

            // New name should be unique.
            Course? existingCourse = await courseRepository.GetCourseByNameAsync(request.Name, id);
            if (existingCourse is not null)
            {
                return Result.Failure(new CourseAlreadyExistsError());
            }

            // Class ids should be valid.
            bool areClassIdsValid = await classRepository.AreClassIdsValidAsync(request.ClassIds);
            if (!areClassIdsValid)
            {
                return Result.Failure(new InvalidClassIdsError());
            }

            List<Class> classes = await classRepository.GetClassesByIdAsync(request.ClassIds);
            course.Name = request.Name;
            course.Classes = classes;
            await unitOfWork.SaveChangesAsync();

            return Result.Success(StatusCodes.Status204NoContent);
        }
    }
}
