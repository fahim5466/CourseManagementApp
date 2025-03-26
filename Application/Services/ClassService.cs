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
        public Task<Result<ClassResponseDto>> GetClassByIdAsync(string id);
        public Task<Result<List<ClassResponseDto>>> GetAllClassesAsync();
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

        public async Task<Result<ClassResponseDto>> GetClassByIdAsync(string id)
        {
            Class? clss = await classRepository.GetClassByIdAsync(id);
            
            // Class not found.
            if(clss is null)
            {
                return Result<ClassResponseDto>.Failure(new ClassDoesNotExistError());
            }

            return Result<ClassResponseDto>.Success(StatusCodes.Status200OK, new ClassResponseDto { Id = clss.Id.ToString(), Name = clss.Name });
        }

        public async Task<Result<List<ClassResponseDto>>> GetAllClassesAsync()
        {
            List<Class> classes = await classRepository.GetAllClassesAsync();

            return Result<List<ClassResponseDto>>.Success(StatusCodes.Status200OK,
                        classes.Select(c => new ClassResponseDto { Id = c.Id.ToString(), Name = c.Name }).ToList());
        }
    }
}
