using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.Account
{
    public class CreateAccountDto
    {
        [Required]
        public decimal Balance { get; set; }

        [Required]
        public int AccountTypeId { get; set; }

        [Required]
        public int BankId { get; set; }
    }
}
