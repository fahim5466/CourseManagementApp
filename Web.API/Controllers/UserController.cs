using Application.DTOs.User;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.Helpers;
using static Application.Helpers.ResultHelper;

namespace Web.API.Controllers
{
    public class UserController(IUserService userService) : BaseController
    {
        [HttpPost]
        [Route("register/student")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> RegisterStudentAsync(RegisterUserRequestDto request)
        {
            Result result = await userService.RegisterStudentAsync(request, HttpHelpers.GetHostPathPrefix(HttpContext));

            return ApiResult(result);
        }
    }
}
