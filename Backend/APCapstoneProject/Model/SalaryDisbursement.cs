using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APCapstoneProject.Model
{
    //type of transaction which sends money to all employees from the client so it contains a list of employees and how much salary each employee has been sent
    public class SalaryDisbursement : Transaction
    {
        [Required]
        public int ClientUserId { get; set; }
        public virtual ClientUser? ClientUser { get; set; }

        public bool AllEmployees { get; set; }

        [DataType(DataType.Currency)]
        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }

        public bool? IsPartialSuccess { get; set; } = null; // true if some succeed, false if all succeed
        public int TotalEmployees { get; set; }
        public int SuccessfulCount { get; set; }
        public int FailedCount { get; set; }





        [DataType(DataType.DateTime)]
        public DateTime DisbursementDate { get; set; } = DateTime.UtcNow;


        public virtual ICollection<SalaryDisbursementDetail>? Details { get; set; } = new List<SalaryDisbursementDetail>();
    }
}