using Application.DTOs.User;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.Helpers;
using static Application.Helpers.ResultHelper;

namespace Web.API.Controllers
{
    [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
    public class UserController(IUserService userService) : BaseController
    {
        [HttpPost]
        [Route("register/student")]
        public async Task<IActionResult> RegisterStudentAsync(UserRequestDto request)
        {
            Result result = await userService.RegisterStudentAsync(request, HttpHelpers.GetHostPathPrefix(HttpContext));

            return ApiResult(result);
        }

        [HttpPost]
        [Route("student/update")]
        public async Task<IActionResult> UpdateStudentAsync(string id, UserRequestDto request)
        {
            Result result = await userService.UpdateStudentAsync(id, request, HttpHelpers.GetHostPathPrefix(HttpContext));

            return ApiResult(result);
        }

        [HttpGet]
        [Route("student")]
        public async Task<IActionResult> GetStudentByIdAsync(string id)
        {
            Result<UserResponseDto> result = await userService.GetStudentByIdAsync(id);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("students")]
        public async Task<IActionResult> GetAllStudentsAsync()
        {
            Result<List<UserResponseDto>> result = await userService.GetAllStudentsAsync();

            return ApiResult(result);
        }

        [HttpGet]
        [Route("student/delete")]
        public async Task<IActionResult> DeleteStudentAsync(string id)
        {
            Result result = await userService.DeleteStudentAsync(id);

            return ApiResult(result);
        }
    }
}
