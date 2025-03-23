using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Helpers
{
    public static class ResultHelper
    {
        private const string APP_ERROR_PATH = "coursemanage/errors";

        public class Result
        {
            public bool IsSuccessful { get; set; }
            public int StatusCode { get; set; }
            public ProblemDetails? ProblemDetails { get; set; }

            protected Result() { }

            public static Result Success(int statusCode)
            {
                return new Result
                {
                    IsSuccessful = true,
                    StatusCode = statusCode
                };
            }

            public static Result Failure(ProblemDetails problemDetails)
            {
                problemDetails.Type = $"{APP_ERROR_PATH}/{problemDetails.Type}";
                return new Result
                {
                    IsSuccessful = false,
                    StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError,
                    ProblemDetails = problemDetails
                };
            }
        }

        public class Result<T> : Result
        {
            public T? Value { get; set; }

            private Result() {}

            public static Result<T> Success(int statusCode, T? value)
            {
                return new Result<T>
                {
                    IsSuccessful = true,
                    StatusCode = statusCode,
                    Value = value
                };
            }

            public static new Result<T> Failure(ProblemDetails problemDetails)
            {
                problemDetails.Type = $"{APP_ERROR_PATH}/{problemDetails.Type}";
                return new Result<T>
                {
                    IsSuccessful = false,
                    StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError,
                    ProblemDetails = problemDetails
                };
            }
        }
    }
}
