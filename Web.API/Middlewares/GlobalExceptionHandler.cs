using Application.Errors;
using static Application.Helpers.ResultHelper;

namespace Web.API.Middlewares
{
    public class GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch(Exception exception)
            {
                logger.LogError(exception, "Exception ocurred: {Message}", exception.Message);

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                Result result = Result.Failure(new UnexpectedError());

                await httpContext.Response.WriteAsJsonAsync(result.ProblemDetails);
            }
        }
    }
}
