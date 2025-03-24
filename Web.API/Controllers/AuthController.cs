using Application.DTOs.Auth;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using static Application.Helpers.ResultHelper;

namespace Web.API.Controllers
{
    public class AuthController(IAuthService authService) : BaseController
    {
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            Result<LoginResponseDto> response = await authService.LoginAsync(request);
            return ApiResult(response);
        }
    }
}
