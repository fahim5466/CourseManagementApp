using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static Application.Errors.AuthErrors;
using static Application.Helpers.ResultHelper;
using static Application.Helpers.ValidationHelper;

namespace Application.Services
{
    public interface IAuthService
    {
        public Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    }

    public class AuthService(IConfiguration configuration, ICryptoHasher cryptoHasher, ISecurityTokenProvider securityTokenProvider,
                       IUserRepository userRepository, IUnitOfWork unitOfWork) : IAuthService
    {
        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            // Validate request.
            ValidationOutcome validationOutcome = Validate(request);
            if(!validationOutcome.IsValid)
            {
                return Result<LoginResponseDto>.Failure(new BadLoginRequest(validationOutcome.Errors));
            }

            // Find user by email.
            User? user = await userRepository.GetUserByEmailWithRolesAsync(request.Email);

            if (user == null)
            {
                return Result<LoginResponseDto>.Failure(new InvalidLoginCredentialsError());
            }

            // Check password.
            if (!cryptoHasher.Verify(request.Password, user.PasswordHash))
            {
                return Result<LoginResponseDto>.Failure(new InvalidLoginCredentialsError());
            }

            // Check if email is verified.
            if (!user.IsEmailVerified)
            {
                return Result<LoginResponseDto>.Failure(new EmailIsNotVerifiedError());
            }

            // Create jwt token.
            string jwtToken = securityTokenProvider.CreateJwtToken(user);

            // Create and save refresh token.
            string refreshToken = securityTokenProvider.CreateRefreshToken();
            user.RefreshTokenHash = cryptoHasher.SimpleHash(refreshToken);
            user.RefreshTokenExpires = DateTime.UtcNow.AddMinutes(Int32.Parse(configuration["RefTok:ExpirationInMinutes"]!));
            await unitOfWork.SaveChangesAsync();

            return Result<LoginResponseDto>.Success(StatusCodes.Status200OK, new LoginResponseDto { JwtToken = jwtToken, RefreshToken = refreshToken });

        }
    }
}
