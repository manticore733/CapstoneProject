using APCapstoneProject.DTO.Document;

namespace APCapstoneProject.Service
{
    public interface IDocumentService
    {
        Task<DocumentReadDto> UploadDocumentAsync(int clientUserId, int proofTypeId, IFormFile file);
        Task<IEnumerable<DocumentReadDto>> GetDocumentsForClientAsync(int bankUserId, int clientUserId);



        // --- ADD THIS LINE ---
        Task<IEnumerable<DocumentReadDto>> GetMyDocumentsAsync(int clientUserId);

    }
}
