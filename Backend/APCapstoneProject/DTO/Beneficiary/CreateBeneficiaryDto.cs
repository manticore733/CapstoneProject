using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.Beneficiary
{
    public class CreateBeneficiaryDto
    {
        [Required]
        public string BeneficiaryName { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string BankName { get; set; }
        [Required]
        public string IFSC { get; set; }
    }
}
