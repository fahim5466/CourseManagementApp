using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public interface IAuthService
    {
        public Task<LoginResponseDto> Login(LoginRequestDto request);
    }

    public class AuthService(IConfiguration configuration, ICryptoHasher cryptoHasher, ISecurityTokenProvider securityTokenProvider,
                       IUserRepository userRepository, IUnitOfWork unitOfWork) : IAuthService
    {
        public async Task<LoginResponseDto> Login(LoginRequestDto request)
        {
            // Find user by email.
            User? user = await userRepository.GetUserByEmailWithRolesAsync(request.Email);

            if (user == null)
            {
                throw new Exception("Invalid login credentials");
            }

            // Check password.
            if (cryptoHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Invalid login credentials");
            }

            // Check if email is verified.
            if (!user.IsEmailVerified)
            {
                throw new Exception("Email is not verified");
            }

            // Create jwt token.
            string jwtToken = securityTokenProvider.CreateJwtToken(user);

            // Create and save refresh token.
            string refreshToken = securityTokenProvider.CreateRefreshToken();
            user.RefreshTokenHash = cryptoHasher.SimpleHash(refreshToken);
            user.RefreshTokenExpires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("RefTok:ExpirationInMinutes"));
            await unitOfWork.SaveChangesAsync();

            return new LoginResponseDto { JwtToken = jwtToken, RefreshToken = refreshToken };

        }
    }
}
