using Application.DTOs.Course;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Helpers.ResultHelper;

namespace Web.API.Controllers
{
    public class CourseController(ICourseService courseService) : BaseController
    {
        [HttpPost]
        [Route("course/create")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> CreateCourseAsync(CourseRequestDto request)
        {
            Result result = await courseService.CreateCourseAsync(request);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("course")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> GetCourseByIdAsync(string id)
        {
            Result<CourseResponseDto> result = await courseService.GetCourseByIdAsync(id);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("courses")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> GetAllCoursesAsync()
        {
            Result<List<CourseResponseDto>> result = await courseService.GetAllCoursesAsync();

            return ApiResult(result);
        }

        [HttpPost]
        [Route("course/update")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> UpdateCourseAsync(string id, [FromBody] CourseRequestDto request)
        {
            Result result = await courseService.UpdateCourseAsync(id, request);

            return ApiResult(result);
        }
    }
}
