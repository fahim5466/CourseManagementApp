using Application.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Errors
{
    public static class AuthErrors
    {
        private const string AUTH_ERROR_PATH = "auth";

        public class BadLoginRequest : ValidationProblemDetails
        {
            public BadLoginRequest(Dictionary<string, List<string>> errors) : base(errors.ToModelStateDictionary())
            {
                Status = StatusCodes.Status400BadRequest;
                Type = $"{AUTH_ERROR_PATH}/bad-login-request";
                Title = "The login request has one or more validation errors";
            }
        }

        public class InvalidLoginCredentialsError : ProblemDetails
        {
            public InvalidLoginCredentialsError()
            {
                Status = StatusCodes.Status401Unauthorized;
                Type = $"{AUTH_ERROR_PATH}/invalid-login-credentials";
                Title = "Invalid login credentials";
            }
        }

        public class EmailIsNotVerifiedError : ProblemDetails
        {
            public EmailIsNotVerifiedError()
            {
                Status = StatusCodes.Status401Unauthorized;
                Type = $"{AUTH_ERROR_PATH}/email-is-not-verified";
                Title = "User email has not been verified";
            }
        }
    }
}
