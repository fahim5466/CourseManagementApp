using Application.DTOs;
using Application.DTOs.User;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static Application.Errors.UserErrors;
using static Application.Helpers.ResultHelper;
using static Application.Helpers.ValidationHelper;

namespace Application.Services
{
    public interface IUserService
    {
        public Task<Result> RegisterStudentAsync(UserRequestDto request);
        public Task<Result> UpdateStudentAsync(string id, UserRequestDto request);
        public Task<Result<UserResponseDto>> GetStudentByIdAsync(string id);
        public Task<Result<List<UserResponseDto>>> GetAllStudentsAsync();
        public Task<Result> DeleteStudentAsync(string id);
    }

    public class UserService(IUserRepository userRepository, ICryptoHasher cryptoHasher, ISecurityTokenProvider securityTokenProvider, IEmailService emailService, IHttpHelper httpHelper, IConfiguration configuration, IUnitOfWork unitOfWork) : IUserService
    {
        public async Task<Result> RegisterStudentAsync(UserRequestDto request)
        {
            // Validate request.
            ValidationOutcome validationOutcome = Validate(request);
            if(!validationOutcome.IsValid)
            {
                return Result.Failure(new BadRegisterOrUpdateUserRequest(validationOutcome.Errors));
            }

            // Email has to be unique.
            User? existingUser = await userRepository.GetUserByEmailAsync(request.Email);
            if(existingUser != null)
            {
                return Result.Failure(new UserAlreadyExistsError());
            }

            string emailVerificationToken = securityTokenProvider.CreateEmailVerificationToken();
            int emailVerificationTokenExpiration = Int32.Parse(configuration["Email:VerificationTokenExpirationInMinutes"]!);

            // Create user.
            await userRepository.CreateUserWithRolesAsync(new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PasswordHash = cryptoHasher.EnhancedHash(request.Password),
                IsEmailVerified = false,
                EmailVerificationToken = emailVerificationToken,
                EmailVerificationTokenExpires = DateTime.UtcNow.AddMinutes(emailVerificationTokenExpiration)
            }, [Role.STUDENT]);

            await emailService.SendEmailVerificationLinkAsync(request.Email, httpHelper.GetHostPathPrefix(), emailVerificationToken);

            Result result = Result.Success(StatusCodes.Status201Created);

            return result;
        }

        public async Task<Result> UpdateStudentAsync(string id, UserRequestDto request)
        {
            // Validate request.
            ValidationOutcome validationOutcome = Validate(request);
            if (!validationOutcome.IsValid)
            {
                return Result.Failure(new BadRegisterOrUpdateUserRequest(validationOutcome.Errors));
            }

            // Student does not exist.
            User? student = await userRepository.GetStudentByIdAsync(id);
            if (student is null)
            {
                return Result.Failure(new StudentDoesNotExistError());
            }

            bool emailChanged = student.Email != request.Email;
            if(emailChanged)
            {
                // Email has to be unique.
                User? existingUser = await userRepository.GetUserByEmailAsync(request.Email);
                if (existingUser is not null)
                {
                    return Result.Failure(new UserAlreadyExistsError());
                }
            }

            student.Name = request.Name;
            student.Email = request.Email;
            student.PasswordHash = cryptoHasher.EnhancedHash(request.Password);

            if(emailChanged)
            {
                string emailVerificationToken = securityTokenProvider.CreateEmailVerificationToken();
                int emailVerificationTokenExpiration = Int32.Parse(configuration["Email:VerificationTokenExpirationInMinutes"]!);

                student.IsEmailVerified = false;
                student.EmailVerificationToken = emailVerificationToken;
                student.EmailVerificationTokenExpires = DateTime.UtcNow.AddMinutes(emailVerificationTokenExpiration);
            }

            await unitOfWork.SaveChangesAsync();

            if(emailChanged)
            {
                await emailService.SendEmailVerificationLinkAsync(student.Email, httpHelper.GetHostPathPrefix(), student.EmailVerificationToken!);
            }

            return Result.Success(StatusCodes.Status204NoContent);
        }

        public async Task<Result<UserResponseDto>> GetStudentByIdAsync(string id)
        {
            User? student = await userRepository.GetStudentByIdAsync(id);

            // Student does not exist.
            if (student is null)
            {
                return Result<UserResponseDto>.Failure(new StudentDoesNotExistError());
            }

            return Result<UserResponseDto>.Success(StatusCodes.Status200OK, student.ToUserResponseDto());
        }

        public async Task<Result<List<UserResponseDto>>> GetAllStudentsAsync()
        {
            List<User> users = await userRepository.GetAllStudentsAsync();

            return Result<List<UserResponseDto>>.Success(StatusCodes.Status200OK, users.Select(u => u.ToUserResponseDto()).ToList());
        }

        public async Task<Result> DeleteStudentAsync(string id)
        {
            User? student = await userRepository.GetStudentByIdAsync(id);

            if (student is null)
            {
                return Result.Failure(new StudentDoesNotExistError());
            }

            await userRepository.DeleteUserAsync(student);

            return Result.Success(StatusCodes.Status204NoContent);
        }
    }
}
