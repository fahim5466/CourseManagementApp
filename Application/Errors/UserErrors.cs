using Application.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Errors
{
    public class UserErrors
    {
        private const string USER_ERROR_PATH = "user";

        public class BadRegisterOrUpdateUserRequest : ValidationProblemDetails
        {
            public BadRegisterOrUpdateUserRequest(Dictionary<string, List<string>> errors) : base(errors.ToModelStateDictionary())
            {
                Status = StatusCodes.Status400BadRequest;
                Type = $"{USER_ERROR_PATH}/bad-register-or-update-user-request";
                Title = "The register/update user request has one or more validation errors";
            }
        }

        public class UserAlreadyExistsError : ProblemDetails
        {
            public UserAlreadyExistsError()
            {
                Status = StatusCodes.Status409Conflict;
                Type = $"{USER_ERROR_PATH}/user-with-email-already-exists";
                Title = "User with email already exists";
            }
        }

        public class StudentDoesNotExist : ProblemDetails
        {
            public StudentDoesNotExist()
            {
                Status = StatusCodes.Status404NotFound;
                Type = $"{USER_ERROR_PATH}/student-does-not-exist";
                Title = "Student does not exist";
            }
        }
    }
}
