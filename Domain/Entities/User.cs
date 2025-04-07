namespace Domain.Entities
{
    public class User
    {
        #region Constants
        public const int NAME_MIN_LENGTH = 5;
        public const string NAME_MIN_LENGTH_STR = "5";
        public const int NAME_MAX_LENGTH = 50;
        public const string NAME_MAX_LENGTH_STR = "50";
        public const int EMAIL_MAX_LENGTH = 50;
        public const string EMAIL_MAX_LENGTH_STR = "50";
        public const int HASH_MAX_LENGTH = 200;
        public const int PASSWORD_MIN_LENGTH = 5;
        public const string PASSWORD_MIN_LENGTH_STR = "5";
        public const int PASSWORD_MAX_LENGTH = 20;
        public const string PASSWORD_MAX_LENGTH_STR = "20";
        #endregion

        #region Properties
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public bool IsEmailVerified { get; set; }
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpires { get; set; }
        public string? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpires { get; set; }
        #endregion

        #region Navigation properties
        public List<Role> Roles { get; set; } = [];
        #endregion
    }
}
