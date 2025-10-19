using APCapstoneProject.Model;
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

        public bool AllEmployees { get; set; } = true;

        [DataType(DataType.DateTime)]
        public DateTime DisbursementDate { get; set; } = DateTime.UtcNow;


        // for review
        public virtual ICollection<SalaryDisbursementDetail>? Details { get; set; } = new List<SalaryDisbursementDetail>();
    }
}


public virtual ICollection<Employee>? Employees { get; set; } = new List<Employee>();