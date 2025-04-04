﻿using Application.DTOs;
using Application.DTOs.Class;
using Application.DTOs.Course;
using Domain.Entities;
using Domain.Relationships;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using static Application.Errors.ClassErrors;
using static Application.Errors.CourseErrors;
using static Application.Errors.UserErrors;
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
        public Task<Result> DeleteCourseAsync(string id);
        public Task<Result> EnrollStudentInCourseAsync(CourseEnrollmentRequestDto request);
    }

    public class CourseService(ICourseRepository courseRepository, IClassRepository classRepository, IUserRepository userRepository, IUnitOfWork unitOfWork) : ICourseService
    {

        public async Task<Result<CourseResponseDto>> GetCourseByIdAsync(string id)
        {
            Course? course = await courseRepository.GetCourseByIdWithClassesAsync(id);

            // Course does not exist.
            if(course is null)
            {
                return Result<CourseResponseDto>.Failure(new CourseDoesNotExistError());
            }

            return Result<CourseResponseDto>.Success(StatusCodes.Status200OK, course.ToCourseResponseDto());
        }

        public async Task<Result<List<CourseResponseDto>>> GetAllCoursesAsync()
        {
            List<Course> courses = await courseRepository.GetAllCoursesAsync();

            return Result<List<CourseResponseDto>>.Success(StatusCodes.Status200OK,
                    courses.Select(course => course.ToCourseResponseDto()).ToList());
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

        public async Task<Result> DeleteCourseAsync(string id)
        {
            Course? course = await courseRepository.GetCourseByIdWithClassesAsync(id);

            // Course does not exist.
            if(course is null)
            {
                return Result.Failure(new CourseDoesNotExistError());
            }

            await courseRepository.DeleteCourseAsync(course);

            return Result.Success(StatusCodes.Status204NoContent);
        }

        public async Task<Result> EnrollStudentInCourseAsync(CourseEnrollmentRequestDto request)
        {
            // Course should exist.
            Course? course = await courseRepository.GetCourseByIdAsync(request.CourseId);
            if (course is null)
            {
                return Result.Failure(new CourseDoesNotExistError());
            }

            // Student should exist.
            User? student = await userRepository.GetStudentByIdAsync(request.StudentId);
            if (student is null)
            {
                return Result.Failure(new StudentDoesNotExistError());
            }

            Guid courseGuid = Guid.Empty;
            Guid.TryParse(request.CourseId, out courseGuid);

            Guid studentGuid = Guid.Empty;
            Guid.TryParse(request.StudentId, out studentGuid);

            // Student should not be enrolled already.
            CourseEnrollment? courseEnrollment = await courseRepository.GetCourseEnrollmentAsync(courseGuid, studentGuid);
            if (courseEnrollment is not null)
            {
                return Result.Failure(new StudentAlreadyEnrolledInCourseError());
            }

            await courseRepository.CreateCourseEnrollmentAsync(new()
            {
                CourseId = courseGuid,
                StudentId = studentGuid
            });

            return Result.Success(StatusCodes.Status200OK);
        }
    }
}
