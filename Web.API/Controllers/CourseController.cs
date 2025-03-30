using Application.DTOs.Class;
using Application.DTOs.Course;
using Application.DTOs.Course;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Helpers.ResultHelper;

namespace Web.API.Controllers
{
    [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
    public class CourseController(ICourseService courseService) : BaseController
    {
        [HttpPost]
        [Route("course/create")]
        public async Task<IActionResult> CreateCourseAsync(CourseRequestDto request)
        {
            Result result = await courseService.CreateCourseAsync(request);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("course")]
        public async Task<IActionResult> GetCourseByIdAsync(string id)
        {
            Result<CourseResponseDtoWithClasses> result = await courseService.GetCourseByIdAsync(id);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("courses")]
        public async Task<IActionResult> GetAllCoursesAsync()
        {
            Result<List<CourseResponseDtoWithClasses>> result = await courseService.GetAllCoursesAsync();

            return ApiResult(result);
        }

        [HttpPost]
        [Route("course/update")]
        public async Task<IActionResult> UpdateCourseAsync(string id, [FromBody] CourseRequestDto request)
        {
            Result result = await courseService.UpdateCourseAsync(id, request);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("course/delete")]
        public async Task<IActionResult> DeleteCourseAsync(string id)
        {
            Result result = await courseService.DeleteCourseAsync(id);

            return ApiResult(result);
        }

        [HttpPost]
        [Route("course/enroll")]
        public async Task<IActionResult> EnrollStudentInCourseAsync(CourseEnrollmentRequestDto request)
        {
            Result result = await courseService.EnrollStudentInCourseAsync(request);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("course/{id}/classes")]
        public async Task<IActionResult> GetClassesOfCourseAsync(string id)
        {
            Result<List<ClassResponseDto>> result = await courseService.GetClassesOfCourseAsync(id);

            return ApiResult(result);
        }
    }
}
