using Application.DTOs.Class;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Helpers.ResultHelper;

namespace Web.API.Controllers
{
    public class ClassController(IClassService classService) : BaseController
    {
        [HttpPost]
        [Route("class/create")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> CreateClassAsync(CreateClassRequestDto request)
        {
            Result result = await classService.CreateClassAsync(request);

            return ApiResult(result);
        }
    }
}
