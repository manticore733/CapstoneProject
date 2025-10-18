using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APCapstoneProject.Model
{
    public class Employee
    {

        public int EmployeeId { get; set; }

        //nav property
        public int ClientUserId { get; set; }
        public virtual ClientUser? ClientUser { get; set; }


        [Required(ErrorMessage = "Employee Name is Required!")]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }


        [Required(ErrorMessage = "Account Number is Required!")]
        public string AccountNumber { get; set; }


        [Required(ErrorMessage = "Bank Name is Required!")]
        public string BankName { get; set; }


        [Required(ErrorMessage = "IFSC Code is Required!")]
        public string IFSC { get; set; }

        [Required(ErrorMessage = "Salary amount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        [DataType(DataType.Currency)]
        public int Salary { get; set; }


        [DataType(DataType.Date)]
        public DateTime DateOfJoining { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfLeaving { get; set; }



        public virtual ICollection<SalaryDisbursementList>? SalaryDisbursementList { get; set; }


        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
