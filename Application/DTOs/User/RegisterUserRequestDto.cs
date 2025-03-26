using System.ComponentModel.DataAnnotations;
using static Domain.Entities.User;

namespace Application.DTOs.User
{
    public class RegisterUserRequestDto : IRequestDto
    {
        public const string EMAIL_REQ_ERR_MSG = "Email is required";
        public const string EMAIL_MAXLENGTH_ERR_MSG = $"Email is too long";
        public const string EMAIL_FORMAT_ERR_MSG = "Email format is not correct";
        public const string PASSWORD_REQ_ERR_MSG = "Password is required";
        public const string PASSWORD_MINLEN_ERR_MSG = "Password is too short";
        public const string PASSWORD_MAXLEN_ERR_MSG = "Password is too long";
        public const string NAME_REQ_ERR_MSG = "Name is required";
        public const string NAME_MINLEN_ERR_MSG = "Name is too short";
        public const string NAME_MAXLEN_ERR_MSG = "Name is too long";

        [Required(ErrorMessage = EMAIL_REQ_ERR_MSG)]
        [MaxLength(EMAIL_MAX_LENGTH, ErrorMessage = EMAIL_MAXLENGTH_ERR_MSG)]
        [EmailAddress(ErrorMessage = EMAIL_FORMAT_ERR_MSG)]
        public required string Email { get; set; }

        [Required(ErrorMessage = PASSWORD_REQ_ERR_MSG)]
        [MinLength(PASSWORD_MIN_LENGTH, ErrorMessage = PASSWORD_MINLEN_ERR_MSG)]
        [MaxLength(PASSWORD_MAX_LENGTH, ErrorMessage = PASSWORD_MAXLEN_ERR_MSG)]
        public required string Password { get; set; }

        [Required(ErrorMessage = NAME_REQ_ERR_MSG)]
        [MinLength(NAME_MIN_LENGTH, ErrorMessage = NAME_MINLEN_ERR_MSG)]
        [MaxLength(NAME_MAX_LENGTH, ErrorMessage = NAME_MAXLEN_ERR_MSG)]
        public required string Name { get; set; }

        public void Preprocess()
        {
            Email = Email.Trim().ToLower();
            Name = Name.Trim();
        }
    }
}
