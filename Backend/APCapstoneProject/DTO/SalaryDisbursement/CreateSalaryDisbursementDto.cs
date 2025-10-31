using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.SalaryDisbursement
{
    public class CreateSalaryDisbursementDto
    {
        [Required(ErrorMessage = "EmployeeId is required.")]
        public int EmployeeId { get; set; }

        [MaxLength(200)]
        public string? Remarks { get; set; }
    }
}
