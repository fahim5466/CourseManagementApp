using Application.DTOs.Class;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using static Application.Errors.ClassErrors;
using static Application.Helpers.ResultHelper;
using static Application.Helpers.ValidationHelper;

namespace Application.Services
{
    public interface IClassService
    {
        public Task<Result> CreateClassAsync(CreateClassRequestDto request);
    }

    public class ClassService(IClassRepository classRepository) : IClassService
    {
        public async Task<Result> CreateClassAsync(CreateClassRequestDto request)
        {
            // Validate request.
            ValidationOutcome validationOutcome = Validate(request);
            if(!validationOutcome.IsValid)
            {
                return Result.Failure(new BadCreateClassRequest(validationOutcome.Errors));
            }

            // Class name should be unique.
            Class? existingClass = await classRepository.GetClassByNameAsync(request.Name);
            if(existingClass is not null)
            {
                return Result.Failure(new ClassAlreadyExistsError());
            }

            await classRepository.CreateClassAsync(new() { Id = Guid.NewGuid(), Name = request.Name});

            return Result.Success(StatusCodes.Status201Created);
        }
    }
}
