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
        public Task<Result> RegisterStudentAsync(RegisterUserRequestDto request, string pathPreix);
        public Task<Result<UserResponseDto>> GetStudentByIdAsync(string id);
    }

    public class UserService(IUserRepository userRepository, ICryptoHasher cryptoHasher, ISecurityTokenProvider securityTokenProvider, IEmailService emailService, IConfiguration configuration) : IUserService
    {
        public async Task<Result> RegisterStudentAsync(RegisterUserRequestDto request, string pathPrefix)
        {
            // Validate request.
            ValidationOutcome validationOutcome = Validate(request);
            if(!validationOutcome.IsValid)
            {
                return Result.Failure(new BadRegisterUserRequest(validationOutcome.Errors));
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

            await emailService.SendEmailVerificationLinkAsync(request.Email, pathPrefix, emailVerificationToken);

            Result result = Result.Success(StatusCodes.Status201Created);

            return result;
        }

        public async Task<Result<UserResponseDto>> GetStudentByIdAsync(string id)
        {
            User? student = await userRepository.GetStudentByIdAsync(id);

            // Student does not exist.
            if (student is null)
            {
                return Result<UserResponseDto>.Failure(new StudentDoesNotExist());
            }

            return Result<UserResponseDto>.Success(StatusCodes.Status200OK,
                                                    new UserResponseDto()
                                                    {
                                                        Id = student.Id.ToString(),
                                                        Name = student.Name,
                                                        Email = student.Email,
                                                        IsEmailVerified = student.IsEmailVerified
                                                    });
        }
    }
}
