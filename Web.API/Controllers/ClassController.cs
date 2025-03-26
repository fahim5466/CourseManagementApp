﻿using Application.DTOs.Class;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Helpers.ResultHelper;

namespace Web.API.Controllers
{
    public class ClassController(IClassService classService) : BaseController
    {
        [HttpGet]
        [Route("class")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> GetClassByIdAsync(string id)
        {
            Result<ClassResponseDto> result = await classService.GetClassByIdAsync(id);

            return ApiResult(result);
        }

        [HttpGet]
        [Route("classes")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> GetAllClassesAsync()
        {
            Result<List<ClassResponseDto>> result = await classService.GetAllClassesAsync();

            return ApiResult(result);
        }

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
