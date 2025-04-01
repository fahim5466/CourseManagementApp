using Application.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Errors
{
    public class ClassErrors
    {
        private const string CLASS_ERROR_PATH = "class";

        public class BadClassCreateOrUpdateRequest : ValidationProblemDetails
        {
            public BadClassCreateOrUpdateRequest(Dictionary<string, List<string>> errors) : base(errors.ToModelStateDictionary())
            {
                Status = StatusCodes.Status400BadRequest;
                Type = $"{CLASS_ERROR_PATH}/bad-class-create-or-update-request";
                Title = "The class create/update request has one or more validation errors";
            }
        }

        public class ClassAlreadyExistsError : ProblemDetails
        {
            public ClassAlreadyExistsError()
            {
                Status = StatusCodes.Status409Conflict;
                Type = $"{CLASS_ERROR_PATH}/class-already-exists-error";
                Title = "Class with this name already exists";
            }
        }

        public class ClassDoesNotExistError : ProblemDetails
        {
            public ClassDoesNotExistError()
            {
                Status = StatusCodes.Status404NotFound;
                Type = $"{CLASS_ERROR_PATH}/class-does-not-exist-error";
                Title = "Class does not exist";
            }
        }

        public class StudentAlreadyEnrolledInClassError : ProblemDetails
        {
            public StudentAlreadyEnrolledInClassError()
            {
                Status = StatusCodes.Status409Conflict;
                Type = $"{CLASS_ERROR_PATH}/student-already-enrolled-in-class-error";
                Title = "Student is already enrolled in class";
            }
        }

        public class StudentNotEnrolledInClassError : ProblemDetails
        {
            public StudentNotEnrolledInClassError()
            {
                Status = StatusCodes.Status403Forbidden;
                Type = $"{CLASS_ERROR_PATH}/student-not-enrolled-in-class-error";
                Title = "Student is not enrolled in class";
            }
        }
    }
}
