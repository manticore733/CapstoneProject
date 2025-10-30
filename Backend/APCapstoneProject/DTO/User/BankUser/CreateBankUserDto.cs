using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.User.BankUser
{
    public class CreateBankUserDto
    {
        [Required]
        public string UserFullName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{10}$")]
        public string UserPhone { get; set; }

        [Required]
        public int BankId { get; set; }
        [Required]
        public string Branch { get; set; }
    }
}
