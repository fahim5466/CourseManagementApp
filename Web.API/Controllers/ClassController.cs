using Application.DTOs.Class;
using Application.DTOs.Course;
using Application.DTOs.Enrollment;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Helpers.ResultHelper;

namespace Web.API.Controllers
{
    public class ClassController(IClassService classService, IHttpHelper httpHelper, ILogger<ClassController> logger) : BaseController
    {
        [HttpPost]
        [Route("class")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> CreateClassAsync(ClassRequestDto request)
        {
            Result<ClassResponseDto> result = await classService.CreateClassAsync(request);

            LogResult(result, logger);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("class/{id}")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> GetClassByIdAsync(string id)
        {
            Result<ClassResponseDto> result = await classService.GetClassByIdAsync(id);

            LogResult(result, logger);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("classes")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> GetAllClassesAsync()
        {
            Result<List<ClassResponseDto>> result = await classService.GetAllClassesAsync();

            LogResult(result, logger);

            return ApiResult(result);
        }

        [HttpPut]
        [Route("class/{id}")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> UpdateClassAsync(string id, [FromBody] ClassRequestDto request)
        {
            Result<ClassResponseDto> result = await classService.UpdateClassAsync(id, request);

            LogResult(result, logger);

            return ApiResult(result);
        }

        [HttpDelete]
        [Route("class/{id}")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> DeleteClassAsync(string id)
        {
            Result result = await classService.DeleteClassAsync(id);

            LogResult(result, logger);

            return ApiResult(result);
        }

        [HttpPost]
        [Route("class/enroll")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> EnrollStudentInClassAsync(ClassEnrollmentRequestDto request)
        {
            Result result = await classService.EnrollStudentInClassAsync(request);

            LogResult(result, logger);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("class/{id}/courses")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> GetCoursesOfClassAsync(string id)
        {
            Result<List<CourseResponseDto>> result = await classService.GetCoursesOfClassAsync(id);

            LogResult(result, logger);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("class/{id}/students")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> GetStudentsOfClassAsync(string id)
        {
            Result<List<UserResponseDto>> result = await classService.GetStudentsOfClassAsync(id);

            LogResult(result, logger);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("class/{classId}/other-students")]
        [Authorize(Roles = $"{Role.STUDENT}")]
        public async Task<IActionResult> GetOtherStudentNamesOfClassAsync(string classId)
        {
            string currentUserId = httpHelper.GetCurrentUserId().ToString();

            Result<List<string>> result = await classService.GetOtherStudentNamesOfClassAsync(currentUserId, classId);

            LogResult(result, logger);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("my-classes")]
        [Authorize(Roles = $"{Role.STUDENT}")]
        public async Task<IActionResult> GetClassesOfStudentAsync()
        {
            string currentUserId = httpHelper.GetCurrentUserId().ToString();

            Result<List<ClassResponseDto>> result = await classService.GetClassesOfStudentAsync(currentUserId);

            LogResult(result, logger);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("enrollment-info")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> GetClassEnrollmentInfoForStudentAsync(string classId, string studentId)
        {
            Result<EnrollmentInfoResponseDto> result = await classService.GetClassEnrollmentInfoForStudentAsync(classId, studentId);

            LogResult(result, logger);

            return ApiResult(result);
        }
    }
}
