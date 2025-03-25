using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class LoginRequestDto : IRequestDto
    {
        public const string EMAIL_REQ_ERR_MSG = "User email is required";
        public const string PASSWORD_REQ_ERR_MSG = "User password is required";

        [Required(ErrorMessage = EMAIL_REQ_ERR_MSG)]
        public required string Email { get; set; }

        [Required(ErrorMessage = PASSWORD_REQ_ERR_MSG)]
        public required string Password { get; set; }

        public void Preprocess()
        {
            Email = Email.Trim().ToLower();
        }
    }
}
