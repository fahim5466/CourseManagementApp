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
        [Route("student")]
        public async Task<IActionResult> RegisterStudentAsync(UserRequestDto request)
        {
            Result<UserResponseDto> result = await userService.RegisterStudentAsync(request);

            return ApiResult(result);
        }

        [HttpPut]
        [Route("student/{id}")]
        public async Task<IActionResult> UpdateStudentAsync(string id, UserRequestDto request)
        {
            Result<UserResponseDto> result = await userService.UpdateStudentAsync(id, request);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("student/{id}")]
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

        [HttpDelete]
        [Route("student/{id}")]
        public async Task<IActionResult> DeleteStudentAsync(string id)
        {
            Result result = await userService.DeleteStudentAsync(id);

            return ApiResult(result);
        }
    }
}
