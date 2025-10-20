using APCapstoneProject.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APCapstoneProject.Model
{           
    public class Payment: Transaction
    {
        //nav property
        [Required]
        public int SenderClientId { get; set; }
        public virtual ClientUser? SenderClient { get; set; }

        //nav property
        [Required]
        public int BeneficiaryId { get; set; }
        public virtual Beneficiary? Beneficiary { get; set; }

        //parent class already has a processedAt field but still time of approval and processing can differ.
        public DateTime? ApprovedAt { get; set; }
    }
}