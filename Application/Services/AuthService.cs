using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using static Application.Errors.AuthErrors;
using static Application.Helpers.ResultHelper;
using static Application.Helpers.ValidationHelper;

namespace Application.Services
{
    public interface IAuthService
    {
        public Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        public Task<Result> VerifyEmailAsync(string verificationToken);
        public Task<Result<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
        public Task<Result> LogoutAsync(string email);
    }

    public class AuthService(IConfiguration configuration, ICryptoHasher cryptoHasher, ISecurityTokenProvider securityTokenProvider,
                             IUserRepository userRepository, IUnitOfWork unitOfWork, IEmailService emailService) : IAuthService
    {
        public const string VERIFY_EMAIL_ROUTE = "verify/email";

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            // Validate request.
            ValidationOutcome validationOutcome = Validate(request);
            if(!validationOutcome.IsValid)
            {
                return Result<LoginResponseDto>.Failure(new BadLoginRequest(validationOutcome.Errors));
            }

            // User should exist.
            User? user = await userRepository.GetUserByEmailWithRolesAsync(request.Email);

            if (user is null)
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
            string refreshToken = await CreateAndSaveRefreshTokenAsync(user);

            return Result<LoginResponseDto>.Success(StatusCodes.Status200OK, new LoginResponseDto { JwtToken = jwtToken, RefreshToken = refreshToken });

        }

        public async Task<Result> VerifyEmailAsync(string verificationToken)
        {
            // Token has to be matched with user.
            User? user = await userRepository.GetUserByEmailVerificationTokenAsync(verificationToken);
            if(user is null)
            {
                return Result.Failure(new InvalidEmailVerificationToken());
            }

            // If token is expired, generate and send new token.
            if(user.EmailVerificationTokenExpires is not null && user.EmailVerificationTokenExpires < DateTime.UtcNow)
            {
                string emailVerificationToken = securityTokenProvider.CreateEmailVerificationToken();
                int emailVerificationTokenExpiration = Int32.Parse(configuration["Email:VerificationTokenExpirationInMinutes"]!);

                user.EmailVerificationToken = emailVerificationToken;
                user.EmailVerificationTokenExpires = DateTime.UtcNow.AddMinutes(emailVerificationTokenExpiration);

                await unitOfWork.SaveChangesAsync();

                await emailService.SendEmailVerificationLinkAsync(user.Email, emailVerificationToken);

                return Result.Failure(new ExpiredEmailVerificationToken());
            }

            // Verify email.
            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpires = null;

            await unitOfWork.SaveChangesAsync();

            return Result.Success(StatusCodes.Status200OK);
        }

        public async Task<Result<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            // Validate jwt token ignoring expiration.
            ClaimsPrincipal? claimsPrincipal = securityTokenProvider.ValidateJwtToken(request.JwtToken, false);

            if (claimsPrincipal == null)
            {
                return Result<RefreshTokenResponseDto>.Failure(new InvalidLoginCredentialsError());
            }

            // Validate user.
            string email = securityTokenProvider.GetEmailFromClaims(claimsPrincipal);
            User? user = await userRepository.GetUserByEmailWithRolesAsync(email);

            if(user is null || user.RefreshTokenHash is null || user.RefreshTokenExpires is null)
            {
                return Result<RefreshTokenResponseDto>.Failure(new InvalidLoginCredentialsError());
            }

            // Validate refresh token.
            if(user.RefreshTokenExpires < DateTime.UtcNow || !cryptoHasher.Verify(request.RefreshToken, user.RefreshTokenHash))
            {
                return Result<RefreshTokenResponseDto>.Failure(new InvalidLoginCredentialsError());
            }

            // Generate new jwt and refresh token.
            string jwtToken = securityTokenProvider.CreateJwtToken(user);
            string refreshToken = await CreateAndSaveRefreshTokenAsync(user);

            return Result<RefreshTokenResponseDto>.Success(StatusCodes.Status200OK, new RefreshTokenResponseDto { JwtToken = jwtToken, RefreshToken = refreshToken});
        }

        public async Task<Result> LogoutAsync(string email)
        {
            // User should exist.
            User? user = await userRepository.GetUserByEmailAsync(email);
            if(user is null)
            {
                return Result.Failure(new InvalidLoginCredentialsError());
            }

            user.RefreshTokenExpires = DateTime.UtcNow;
            await unitOfWork.SaveChangesAsync();

            return Result.Success(StatusCodes.Status200OK);
        }

        private async Task<string> CreateAndSaveRefreshTokenAsync(User user)
        {
            string refreshToken = securityTokenProvider.CreateRefreshToken();
            user.RefreshTokenHash = cryptoHasher.SimpleHash(refreshToken);
            user.RefreshTokenExpires = DateTime.UtcNow.AddMinutes(Int32.Parse(configuration["RefTok:ExpirationInMinutes"]!));

            await unitOfWork.SaveChangesAsync();

            return refreshToken;
        }
    }
}
