using Application.DTOs.User;
using Application.Interfaces;
using Domain.Entities.Roles;
using Domain.Entities.Users;
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
                EmailVerificationTokenHash = cryptoHasher.SimpleHash(emailVerificationToken),
                EmailVerificationTokenHashExpires = DateTime.UtcNow.AddMinutes(emailVerificationTokenExpiration)
            }, [Role.STUDENT]);

            await emailService.SendEmailVerificationLinkAsync(request.Email, pathPrefix, emailVerificationToken);

            Result result = Result.Success(StatusCodes.Status201Created);

            return result;
        }
    }
}
