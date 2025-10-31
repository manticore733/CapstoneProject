using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.Payment
{
    public class CreatePaymentDto
    {
        [Required]
        public int BeneficiaryId { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }

        public string? Remarks { get; set; }
    }
}
