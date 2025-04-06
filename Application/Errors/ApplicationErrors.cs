using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Errors
{
    public class UnexpectedError : ProblemDetails
    {
        private const string APP_ERROR_PATH = "app";

        public UnexpectedError()
        {
            Status = StatusCodes.Status500InternalServerError;
            Type = $"{APP_ERROR_PATH}/unexpected-error";
            Title = "Unexpected error occured";
        }
    }
}
