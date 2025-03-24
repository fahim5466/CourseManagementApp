using Application.DTOs.Auth;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using static Application.Helpers.ResultHelper;

namespace Web.API.Controllers
{
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : BaseController
    {
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            Result<LoginResponseDto> result = await authService.LoginAsync(request);

            LogResult(result, logger);

            return ApiResult(result);
        }
    }
}
