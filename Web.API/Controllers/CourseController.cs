using Application.DTOs.Class;
using Application.DTOs.Course;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Helpers.ResultHelper;

namespace Web.API.Controllers
{
    public class CourseController(ICourseService courseService, IHttpHelper httpHelper) : BaseController
    {
        [HttpPost]
        [Route("course")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> CreateCourseAsync(CourseRequestDto request)
        {
            Result<CourseResponseDtoWithClasses> result = await courseService.CreateCourseAsync(request);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("course/{id}")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> GetCourseByIdAsync(string id)
        {
            Result<CourseResponseDtoWithClasses> result = await courseService.GetCourseByIdAsync(id);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("courses")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> GetAllCoursesAsync()
        {
            Result<List<CourseResponseDtoWithClasses>> result = await courseService.GetAllCoursesAsync();

            return ApiResult(result);
        }

        [HttpPut]
        [Route("course/{id}")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> UpdateCourseAsync(string id, [FromBody] CourseRequestDto request)
        {
            Result<CourseResponseDtoWithClasses> result = await courseService.UpdateCourseAsync(id, request);

            return ApiResult(result);
        }

        [HttpDelete]
        [Route("course/{id}")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> DeleteCourseAsync(string id)
        {
            Result result = await courseService.DeleteCourseAsync(id);

            return ApiResult(result);
        }

        [HttpPost]
        [Route("course/enroll")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> EnrollStudentInCourseAsync(CourseEnrollmentRequestDto request)
        {
            Result result = await courseService.EnrollStudentInCourseAsync(request);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("course/{id}/classes")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> GetClassesOfCourseAsync(string id)
        {
            Result<List<ClassResponseDto>> result = await courseService.GetClassesOfCourseAsync(id);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("course/{id}/students")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> GetStudentsOfCourseAsync(string id)
        {
            Result<List<UserResponseDto>> result = await courseService.GetStudentsOfCourseAsync(id);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("my-courses")]
        [Authorize(Roles = $"{Role.STUDENT}")]
        public async Task<IActionResult> GetCoursesOfStudentAsync()
        {
            string currentUserId = httpHelper.GetCurrentUserId().ToString();

            Result<List<CourseResponseDto>> result = await courseService.GetCoursesOfStudentAsync(currentUserId);

            return ApiResult(result);
        }
    }
}
