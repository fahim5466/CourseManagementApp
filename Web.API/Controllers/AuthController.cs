using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.Helpers;
using static Application.Helpers.ResultHelper;
using static Application.Services.AuthService;

namespace Web.API.Controllers
{
    public class AuthController(IAuthService authService, ISecurityTokenProvider securityTokenProvider, ILogger<AuthController> logger) : BaseController
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

        [HttpPost]
        [Route("refreshtoken")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            Result<RefreshTokenResponseDto> result = await authService.RefreshTokenAsync(request);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            string email = securityTokenProvider.GetEmailFromClaims(HttpContext.User);

            Result result = await authService.LogoutAsync(email);

            return ApiResult(result);
        }
    }
}
