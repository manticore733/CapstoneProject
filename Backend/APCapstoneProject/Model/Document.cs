using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APCapstoneProject.Model
{
    public class Document
    {
        public int DocumentId { get; set; }


        [Required(ErrorMessage = "DocumentURL is Required!")]
        public string DocumentURL { get; set; }


        [Required(ErrorMessage = "Document Name is Required!")]
        public string DocumentName { get; set; }


        [Required(ErrorMessage = "Document Type is Required!")]
        public int ProofTypeId { get; set; }
        public virtual ProofType? ProofType { get; set; }



        //nav property
        public int ClientUserId { get; set; }
        public virtual ClientUser? ClientUser { get; set; }
    }
}
