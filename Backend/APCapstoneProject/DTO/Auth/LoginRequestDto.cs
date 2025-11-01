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
    }
}
