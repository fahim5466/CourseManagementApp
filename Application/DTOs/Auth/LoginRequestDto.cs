using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class LoginRequestDto : IPreprocessDto
    {
        public const string EMAIL_REQ_ERR_MSG = "User email is required";
        public const string PASSWORD_REQ_ERR_MSG = "User password is required";

        [Required(ErrorMessage = EMAIL_REQ_ERR_MSG)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = PASSWORD_REQ_ERR_MSG)]
        public string Password { get; set; } = string.Empty;

        public void Preprocess()
        {
            Email = Email.Trim().ToLower();
        }
    }
}
