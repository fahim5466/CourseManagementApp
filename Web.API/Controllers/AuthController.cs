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
    public class AuthController(IAuthService authService, ISecurityTokenProvider securityTokenProvider, IHttpHelper httpHelper, ILogger<AuthController> logger) : BaseController
    {
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(LoginRequestDto request)
        {
            Result<LoginResponseDto> result = await authService.LoginAsync(request);

            if(result.Value is not null)
            {
                httpHelper.SetAccessTokenCookie(result.Value.JwtToken);
                httpHelper.SetRefreshTokenCookie(result.Value.RefreshToken);
            }

            LogResult(result, logger);

            return ApiResult((Result)result);
        }

        [HttpGet]
        [Route(VERIFY_EMAIL_ROUTE)]
        public async Task<IActionResult> VerifyEmailAsync(string verificationToken)
        {
            Result result = await authService.VerifyEmailAsync(verificationToken);

            LogResult(result, logger);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            string jwtToken = HttpContext.Request.Cookies[HttpHelper.ACCESS_TOKEN_COOKIE_KEY] ?? string.Empty;
            string refreshToken = HttpContext.Request.Cookies[HttpHelper.REFRESH_TOKEN_COOKIE_KEY] ?? string.Empty;

            RefreshTokenRequestDto request = new() { JwtToken = jwtToken, RefreshToken = refreshToken };

            Result<RefreshTokenResponseDto> result = await authService.RefreshTokenAsync(request);

            if (result.Value is not null)
            {
                httpHelper.SetAccessTokenCookie(result.Value.JwtToken);
                httpHelper.SetRefreshTokenCookie(result.Value.RefreshToken);
            }

            LogResult(result, logger);

            return ApiResult((Result)result);
        }

        [HttpGet]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            string email = securityTokenProvider.GetEmailFromClaims(HttpContext.User);

            Result result = await authService.LogoutAsync(email);

            LogResult(result, logger);

            return ApiResult(result);
        }
    }
}
