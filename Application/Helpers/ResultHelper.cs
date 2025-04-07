using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Helpers
{
    public static class ResultHelper
    {
        private const string APP_ERROR_PATH = "coursemanage/errors";

        public class Result
        {
            public bool IsSuccessful { get; }
            public int StatusCode { get; }
            public ProblemDetails? ProblemDetails { get; }

            protected Result(bool isSuccessful, int statusCode, ProblemDetails? problemDetails)
            {
                IsSuccessful = isSuccessful;
                StatusCode = statusCode;
                ProblemDetails = problemDetails;
            }

            public static Result Success(int statusCode)
            {
                return new Result(true, statusCode, null);
            }

            public static Result Failure(ProblemDetails problemDetails)
            {
                problemDetails.Type = $"{APP_ERROR_PATH}/{problemDetails.Type}";
                int statusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
                return new Result(false, statusCode, problemDetails);
            }
        }

        public class Result<T> : Result
        {
            public T? Value { get; }

            private Result(bool isSuccessful, int statusCode, ProblemDetails? problemDetails, T? value) :
                    base(isSuccessful, statusCode, problemDetails)
            {
                Value = value;
            }

            public static Result<T> Success(int statusCode, T? value)
            {
                return new Result<T>(true, statusCode, null, value);
            }

            public static new Result<T> Failure(ProblemDetails problemDetails)
            {
                problemDetails.Type = $"{APP_ERROR_PATH}/{problemDetails.Type}";
                int statusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
                return new Result<T>(false, statusCode, problemDetails, default);
            }
        }
    }
}
