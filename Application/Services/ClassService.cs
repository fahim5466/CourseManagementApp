using Application.DTOs;
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
        public Task<Result> CreateClassAsync(ClassRequestDto request);
        public Task<Result> UpdateClassAsync(string id, ClassRequestDto request);
        public Task<Result> DeleteClassAsync(string id);
    }

    public class ClassService(IClassRepository classRepository, IUnitOfWork unitOfWork) : IClassService
    {
        public async Task<Result<ClassResponseDto>> GetClassByIdAsync(string id)
        {
            Class? clss = await classRepository.GetClassByIdAsync(id);

            // Class not found.
            if (clss is null)
            {
                return Result<ClassResponseDto>.Failure(new ClassDoesNotExistError());
            }

            return Result<ClassResponseDto>.Success(StatusCodes.Status200OK, clss.ToClassResponseDto());
        }

        public async Task<Result<List<ClassResponseDto>>> GetAllClassesAsync()
        {
            List<Class> classes = await classRepository.GetAllClassesAsync();

            return Result<List<ClassResponseDto>>.Success(StatusCodes.Status200OK,
                        classes.Select(c => c.ToClassResponseDto()).ToList());
        }

        public async Task<Result> CreateClassAsync(ClassRequestDto request)
        {
            // Validate request.
            ValidationOutcome validationOutcome = Validate(request);
            if(!validationOutcome.IsValid)
            {
                return Result.Failure(new BadClassCreateOrUpdateRequest(validationOutcome.Errors));
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

        public async Task<Result> UpdateClassAsync(string id, ClassRequestDto request)
        {
            // Validate request.
            ValidationOutcome validationOutcome = Validate(request);
            if (!validationOutcome.IsValid)
            {
                return Result.Failure(new BadClassCreateOrUpdateRequest(validationOutcome.Errors));
            }

            // Class not found.
            Class? clss = await classRepository.GetClassByIdAsync(id);
            if (clss is null)
            {
                return Result.Failure(new ClassDoesNotExistError());
            }

            // New name should be unique.
            Class? existingClass = await classRepository.GetClassByNameAsync(request.Name, id);
            if (existingClass is not null)
            {
                return Result.Failure(new ClassAlreadyExistsError());
            }

            clss.Name = request.Name;
            await unitOfWork.SaveChangesAsync();

            return Result.Success(StatusCodes.Status204NoContent);
        }

        public async Task<Result> DeleteClassAsync(string id)
        {
            // Class not found.
            Class? clss = await classRepository.GetClassByIdAsync(id);
            if (clss is null)
            {
                return Result<ClassResponseDto>.Failure(new ClassDoesNotExistError());
            }

            await classRepository.DeleteClassAsync(clss);

            return Result.Success(StatusCodes.Status204NoContent);
        }
    }
}
