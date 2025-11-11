using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.User
{
    public class UserCreateDto
    {
        [Required]
        public string UserFullName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public int UserRoleId { get; set; }

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{10}$")]
        public string UserPhone { get; set; }

        public int? BankId { get; set; }
    }
}
