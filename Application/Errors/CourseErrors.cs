using Application.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Errors
{
    public class CourseErrors
    {
        private const string COURSE_ERROR_PATH = "course";

        public class BadCourseCreateOrUpdateRequest : ValidationProblemDetails
        {
            public BadCourseCreateOrUpdateRequest(Dictionary<string, List<string>> errors) : base(errors.ToModelStateDictionary())
            {
                Status = StatusCodes.Status400BadRequest;
                Type = $"{COURSE_ERROR_PATH}/bad-course-create-or-update-request";
                Title = "The course create/update request has one or more validation errors";
            }
        }

        public class InvalidClassIdsError : ProblemDetails
        {
            public InvalidClassIdsError()
            {
                Status = StatusCodes.Status400BadRequest;
                Type = $"{COURSE_ERROR_PATH}/invalid-class-ids-error";
                Title = "One or more class ids are not valid";
            }
        }

        public class CourseAlreadyExistsError : ProblemDetails
        {
            public CourseAlreadyExistsError()
            {
                Status = StatusCodes.Status409Conflict;
                Type = $"{COURSE_ERROR_PATH}/course-already-exists-error";
                Title = "Course with this name already exists";
            }
        }

        public class CourseDoesNotExistError : ProblemDetails
        {
            public CourseDoesNotExistError()
            {
                Status = StatusCodes.Status404NotFound;
                Type = $"{COURSE_ERROR_PATH}/course-does-not-exist-error";
                Title = "Course does not exist";
            }
        }

        public class StudentAlreadyEnrolledInCourseError : ProblemDetails
        {
            public StudentAlreadyEnrolledInCourseError()
            {
                Status = StatusCodes.Status409Conflict;
                Type = $"{COURSE_ERROR_PATH}/student-already-enrolled-in-course-error";
                Title = "Student is already enrolled in course";
            }
        }
    }
}
