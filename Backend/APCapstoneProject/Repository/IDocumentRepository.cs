using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IDocumentRepository
    {
        Task AddAsync(Document document);
        Task<IEnumerable<Document>> GetDocumentsByClientIdAsync(int clientUserId);
        // We might add Delete later if needed
        Task<bool> SaveChangesAsync();
    }
}
