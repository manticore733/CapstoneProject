using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.Beneficiary
{
    public class UpdateBeneficiaryDto
    {
        public string? BeneficiaryName { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? IFSC { get; set; }
    }
}
