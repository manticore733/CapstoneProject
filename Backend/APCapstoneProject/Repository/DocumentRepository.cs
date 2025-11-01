using APCapstoneProject.Data;
using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly BankingAppDbContext _context;

        public DocumentRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Document document)
        {
            // Set Create time (assuming your model doesn't auto-set it)
            //document.CreatedAt = DateTime.UtcNow;
            await _context.Documents.AddAsync(document);
        }

        public async Task<IEnumerable<Document>> GetDocumentsByClientIdAsync(int clientUserId)
        {
            // Include ProofType so we can get its name later
            return await _context.Documents
                .Include(d => d.ProofType)
                .Where(d => d.ClientUserId == clientUserId)
                .ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }
    }
}
