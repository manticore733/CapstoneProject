using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APCapstoneProject.Model
{
    public class Account
    {
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Account Number is Required!")]
        [RegularExpression(@"^BPA\d{8}[A-Z0-9]{6}$", ErrorMessage = "Account Number is Not Valid")]
        public string AccountNumber { get; set; }


        //nav property
        [Required]
        public int? ClientUserId { get; set; }
        public virtual ClientUser? ClientUser { get; set; }


        //nav property
        public int BankId { get; set; }
        public virtual Bank? Bank { get; set; }


        [Required(ErrorMessage = "Balance in Required!")]
        [DataType(DataType.Currency)]
        [Precision(18, 2)]
        public decimal Balance { get; set; } = 0;


        [Required(ErrorMessage = "Account Type is Required!")]
        public int AccountTypeId { get; set; }
        public virtual AccountType? AccountType { get; set; }

        // onlyt exists because an account can be banned
        //nav property
        [Required(ErrorMessage = "Account Status is Required!")]
        public int StatusId { get; set; }
        public virtual Status? AccountStatus { get; set; }


        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DataType(DataType.Date)]
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Transaction>? Transactions { get; set; } 
    }
}

