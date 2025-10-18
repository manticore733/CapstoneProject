using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.Model
{
    public class Beneficiary
    {
        public int BeneficiaryId { get; set; }

        [Required(ErrorMessage = "clientId is Required!")]
        public int ClientId { get; set; }


        [Required(ErrorMessage = "Beneficiary name is Required!")]
        public string BeneficiaryName { get; set; }


        [Required(ErrorMessage = "AccountNumber is Required!")]
        public string AccountNumber { get; set; }


        [Required(ErrorMessage = "Bank name is Required!")]
        public string BankName { get; set; }


        [Required(ErrorMessage = "IFSC Code is Required!")]
        public string IFSC { get; set; }

        public bool IsVerified { get; set; } = false;


        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
