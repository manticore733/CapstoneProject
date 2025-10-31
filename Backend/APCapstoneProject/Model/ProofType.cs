using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.Model
{
    public enum DocProofType
    {
        // Example: Certificate of Incorporation
        BUSINESS_REGISTRATION,

        // Example: Company PAN Card, GST Certificate
        TAX_ID_PROOF,

        // Example: Company utility bill, bank statement
        PROOF_OF_ADDRESS,

        OTHER
    }

    public class ProofType
    {
        public int ProofTypeId { get; set; }

        [Required(ErrorMessage = "Type is Required!")]
        public DocProofType Type { get; set; }
    }
}