using Application.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Errors
{
    public class UserErrors
    {
        private const string USER_ERROR_PATH = "user";

        public class BadRegisterUserRequest : ValidationProblemDetails
        {
            public BadRegisterUserRequest(Dictionary<string, List<string>> errors) : base(errors.ToModelStateDictionary())
            {
                Status = StatusCodes.Status400BadRequest;
                Type = $"{USER_ERROR_PATH}/bad-register-user-request";
                Title = "The register user request has one or more validation errors";
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
    }
}
