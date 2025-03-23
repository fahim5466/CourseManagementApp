using Microsoft.AspNetCore.Mvc;
using static Application.Helpers.ResultHelper;

namespace Web.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public IActionResult ApiResult(Result result)
        {
            return result.IsSuccessful ? StatusCode(result.StatusCode)
                                       : StatusCode(result.StatusCode, result.ProblemDetails);
        }

        public IActionResult ApiResult<T>(Result<T> result)
        {
            return result.IsSuccessful ? StatusCode(result.StatusCode, result.Value)
                                       : StatusCode(result.StatusCode, result.ProblemDetails);
        }
    }
}
