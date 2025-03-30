using Application.DTOs.Class;
using Application.DTOs.Course;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Helpers.ResultHelper;

namespace Web.API.Controllers
{
    [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
    public class ClassController(IClassService classService) : BaseController
    {
        [HttpGet]
        [Route("class")]
        public async Task<IActionResult> GetClassByIdAsync(string id)
        {
            Result<ClassResponseDto> result = await classService.GetClassByIdAsync(id);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("classes")]
        public async Task<IActionResult> GetAllClassesAsync()
        {
            Result<List<ClassResponseDto>> result = await classService.GetAllClassesAsync();

            return ApiResult(result);
        }

        [HttpPost]
        [Route("class/create")]
        public async Task<IActionResult> CreateClassAsync(ClassRequestDto request)
        {
            Result result = await classService.CreateClassAsync(request);

            return ApiResult(result);
        }

        [HttpPost]
        [Route("class/update")]
        public async Task<IActionResult> UpdateClassAsync(string id, [FromBody] ClassRequestDto request)
        {
            Result result = await classService.UpdateClassAsync(id, request);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("class/delete")]
        public async Task<IActionResult> DeleteClassAsync(string id)
        {
            Result result = await classService.DeleteClassAsync(id);

            return ApiResult(result);
        }

        [HttpPost]
        [Route("class/enroll")]
        public async Task<IActionResult> EnrollStudentInClassAsync(ClassEnrollmentRequestDto request)
        {
            Result result = await classService.EnrollStudentInClassAsync(request);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("class/{id}/courses")]
        public async Task<IActionResult> GetCoursesOfClassAsync(string id)
        {
            Result<List<CourseResponseDto>> result = await classService.GetCoursesOfClassAsync(id);

            return ApiResult(result);
        }
    }
}
