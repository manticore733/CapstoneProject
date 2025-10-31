using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.Employee
{
    public class UpdateEmployeeDto
    {

        public string? EmployeeName { get; set; }

        public string? Email { get; set; }

        public string? AccountNumber { get; set; }

        public string? BankName { get; set; }

        public string? IFSC { get; set; }

        public decimal? Salary { get; set; }
        public DateTime? DateOfJoining { get; set; }

       
    }
}
