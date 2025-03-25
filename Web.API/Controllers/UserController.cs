﻿using Application.DTOs.User;
using Application.Services;
using Domain.Entities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Helpers.ResultHelper;

namespace Web.API.Controllers
{
    public class UserController(IUserService userService) : BaseController
    {
        [HttpPost]
        [Route("register/student")]
        [Authorize(Roles = $"{Role.ADMIN}, {Role.STAFF}")]
        public async Task<IActionResult> RegisterStudentAsync(RegisterUserRequestDto request)
        {
            string host = HttpContext.Request.Host.Value!;
            string scheme = HttpContext.Request.Scheme;
            string pathPrefix = $"{scheme}://{host}";

            Result result = await userService.RegisterStudentAsync(request, pathPrefix);

            return ApiResult(result);
        }
    }
}
