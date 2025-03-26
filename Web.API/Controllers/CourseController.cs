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
    }
}
