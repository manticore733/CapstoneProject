using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.Model
{
    public enum DocProofType
    {
        AADHAAR_CARD,
        PAN_CARD,
        ELECTRICITY_BILL,
        PASSPORT,
        DRIVING_LICENSE,
        OTHER
    }
    public class ProofType
    {
        public int ProofTypeId { get; set; }

        [Required(ErrorMessage = "Type is Required!")]
        public DocProofType Type { get; set; }
    }
}
