using System.ComponentModel.DataAnnotations;
using static Domain.Entities.User;

namespace Application.DTOs.User
{
    public class UserRequestDto : IPreprocessDto
    {
        public const string EMAIL_REQ_ERR_MSG = "Email is required";
        public const string EMAIL_MAXLENGTH_ERR_MSG = $"Email cannot be more than {EMAIL_MAX_LENGTH_STR} characters";
        public const string EMAIL_FORMAT_ERR_MSG = "Email format is not correct";
        public const string PASSWORD_REQ_ERR_MSG = "Password is required";
        public const string PASSWORD_MINLEN_ERR_MSG = $"Password cannot be less than {PASSWORD_MIN_LENGTH_STR} characters";
        public const string PASSWORD_MAXLEN_ERR_MSG = $"Password cannot be more than {PASSWORD_MAX_LENGTH_STR} characters";
        public const string NAME_REQ_ERR_MSG = "Name is required";
        public const string NAME_MINLEN_ERR_MSG = $"Name cannot be less than {NAME_MIN_LENGTH_STR} characters";
        public const string NAME_MAXLEN_ERR_MSG = $"Name cannot be more than {NAME_MAX_LENGTH_STR} characters";

        [Required(ErrorMessage = EMAIL_REQ_ERR_MSG)]
        [MaxLength(EMAIL_MAX_LENGTH, ErrorMessage = EMAIL_MAXLENGTH_ERR_MSG)]
        [EmailAddress(ErrorMessage = EMAIL_FORMAT_ERR_MSG)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = PASSWORD_REQ_ERR_MSG)]
        [MinLength(PASSWORD_MIN_LENGTH, ErrorMessage = PASSWORD_MINLEN_ERR_MSG)]
        [MaxLength(PASSWORD_MAX_LENGTH, ErrorMessage = PASSWORD_MAXLEN_ERR_MSG)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = NAME_REQ_ERR_MSG)]
        [MinLength(NAME_MIN_LENGTH, ErrorMessage = NAME_MINLEN_ERR_MSG)]
        [MaxLength(NAME_MAX_LENGTH, ErrorMessage = NAME_MAXLEN_ERR_MSG)]
        public string Name { get; set; } = string.Empty;

        public void Preprocess()
        {
            Email = Email.Trim().ToLower();
            Name = Name.Trim();
        }
    }
}
