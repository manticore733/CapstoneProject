using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.Employee
{
    public class CreateEmployeeDto
    {
        [Required]
        public string EmployeeName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string BankName { get; set; }
        [Required]
        public string IFSC { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Salary { get; set; }
        [Required]
        public DateTime DateOfJoining { get; set; }
    }
}
