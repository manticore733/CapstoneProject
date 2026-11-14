using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APCapstoneProject.Model
{
    //list of how much money sent to each employeee
    public class SalaryDisbursementDetail
    {
        public int SalaryDisbursementDetailId { get; set; }

        [Required]//batch ID in which this salary transaction exists
        public int SalaryDisbursementId { get; set; }
        public virtual SalaryDisbursement? SalaryDisbursement { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }



        public bool? IsSuccessful { get; set; } = null;

        public string? Remark { get; set; }


        public string BankName { get; set; } = string.Empty;
        public string IFSC { get; set; } = string.Empty;
        public string DestinationAccountNumber { get; set; }


        [DataType(DataType.Currency)]
        [Precision(18, 2)]
        public decimal Amount { get; set; }



        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }


}
