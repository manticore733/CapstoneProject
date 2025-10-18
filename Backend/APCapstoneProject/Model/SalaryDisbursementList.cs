using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace APCapstoneProject.Model
{
    public class SalaryDisbursementList
    {
        public int SalaryDisbursementListId { get; set; }

        //nav property
        public int SalaryDisbursementId { get; set; }
        public virtual SalaryDisbursement SalaryDisbursement { get; set; }

        //nav property
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        //nav property
        public int? TransactionId { get; set; }
        public virtual Transaction? Transaction { get; set; }


        public bool? Success { get; set; } = null;

        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;


    }
}
