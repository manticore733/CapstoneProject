using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.Auth
{
    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Captcha token is required.")]
        public string CaptchaToken { get; set; }
    }
}
