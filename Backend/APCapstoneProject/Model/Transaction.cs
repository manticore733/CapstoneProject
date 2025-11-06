using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APCapstoneProject.Model
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        [Required(ErrorMessage = "Amount is Required!")]
        [DataType(DataType.Currency)]
        [Precision(18, 2)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Transaction Type is Required!")]
        public int TransactionTypeId { get; set; }
        public virtual TransactionType? TransactionType { get; set; }

        //nav property
        [Required(ErrorMessage = "Transaction Status is Required!")]
        public int StatusId { get; set; }
        public virtual Status? TransactionStatus { get; set; }

        [MaxLength(200)]
        public string? Remarks { get; set; } // this is a description of the transaction (electricity bill, etc)

        public string? BankRemark { get; set; } //given as reasoning for accepting or rejecting a txn

        //nav property
        public int AccountId { get; set; }
        public virtual Account? Account { get; set; }


        


        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime ProcessedAt { get; set; }
    }
}


