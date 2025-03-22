using Domain.Entities.Roles;

namespace Domain.Entities.Users
{
    public class User
    {
        #region Constants
        public const int NAME_MAX_LENGTH = 50;
        public const int EMAIL_MAX_LENGTH = 50;
        public const int HASH_MAX_LENGTH = 200;
        #endregion

        #region Properties
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public bool IsEmailVerified { get; set; }
        public string? EmailVerificationTokenHash { get; set; }
        public DateTime? EmailVerificationTokenHashExpires { get; set; }
        public string? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpires { get; set; }
        #endregion
    }
}
