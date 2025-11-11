namespace APCapstoneProject.DTO.Document
{
    public class DocumentReadDto
    {
        public int DocumentId { get; set; }
        public string DocumentURL { get; set; }
        public string DocumentName { get; set; }
        public int ProofTypeId { get; set; }
        public string ProofTypeName { get; set; } // We'll show the name (e.g., "TAX_ID_PROOF")
        public int ClientUserId { get; set; }
    }
}
