using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.Account
{
    public class AccountBalanceDto
    {
        public string AccountNumber { get; set; }

        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }
    }
}
