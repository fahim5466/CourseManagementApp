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
        public Task<Result> CreateCourseAsync(CourseRequestDto request);
    }

    public class CourseService(ICourseRepository courseRepository, IClassRepository classRepository) : ICourseService
    {
        public async Task<Result> CreateCourseAsync(CourseRequestDto request)
        {
            // Validate request.
            ValidationOutcome validationOutcome = Validate(request);
            if(!validationOutcome.IsValid)
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
    }
}
