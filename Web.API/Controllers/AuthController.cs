using Application.DTOs.Auth;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Web.API.Helpers;
using static Application.Helpers.ResultHelper;
using static Application.Services.AuthService;

namespace Web.API.Controllers
{
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : BaseController
    {
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(LoginRequestDto request)
        {
            Result<LoginResponseDto> result = await authService.LoginAsync(request);

            LogResult(result, logger);

            return ApiResult(result);
        }

        [HttpGet]
        [Route(VERIFY_EMAIL_ROUTE)]
        public async Task<IActionResult> VerifyEmailAsync(string verificationToken)
        {
            Result result = await authService.VerifyEmailAsync(verificationToken, HttpHelpers.GetHostPathPrefix(HttpContext));

            return ApiResult(result);
        }
    }
}
