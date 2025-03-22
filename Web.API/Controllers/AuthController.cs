using Application.DTOs.Auth;
using Application.Services;
using Domain.Entities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{
    public class AuthController(IAuthService authService) : BaseController
    {
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            LoginResponseDto response = await authService.Login(request);
            return Ok(response);
        }
    }
}
