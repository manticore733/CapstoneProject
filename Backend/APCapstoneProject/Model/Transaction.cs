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
        public string? Remarks { get; set; }

        //nav property
        public int AccountId { get; set; }
        public virtual Account? Account { get; set; }


        public string BankName { get; set; } = string.Empty;
        public string IFSC { get; set; } = string.Empty;
        public string DestinationAccountNumber { get; set; }


        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime ProcessedAt { get; set; }
    }
}


