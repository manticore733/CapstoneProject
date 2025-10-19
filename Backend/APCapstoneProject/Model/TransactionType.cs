using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.Model
{
    public enum TxnType { CREDIT, DEBIT }
    public class TransactionType
    {
        public int TransactionTypeId { get; set; }

        [Required(ErrorMessage = "Type is Required!")]
        public TxnType Type { get; set; }
    }
}
