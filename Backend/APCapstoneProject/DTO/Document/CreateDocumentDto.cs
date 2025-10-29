using System.ComponentModel.DataAnnotations;

namespace APCapstoneProject.DTO.Document
{
    public class CreateDocumentDto
    {
        [Required]
        public int ProofTypeId { get; set; }

        [Required]
        public IFormFile File { get; set; } // The actual file being uploaded
    }
}
