using APCapstoneProject.Data;
using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Repository
{
    public class ClientUserRepository : IClientUserRepository
    {
        private readonly BankingAppDbContext _context;

        public ClientUserRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        // ✅ Basic CRUD Operations

        public async Task<IEnumerable<ClientUser>> GetClientUsersAsync()
        {
            return await _context.ClientUsers
                .Where(c => c.IsActive)
                .Include(c => c.Role)
                .Include(c => c.Bank)
                .Include(c => c.VerificationStatus)
                .Include(c => c.Account)        // 🔹 each client has only one account
                .Include(c => c.Beneficiaries)  // optional: if needed
                .Include(c => c.Employees)      // optional: if needed
                .ToListAsync();
        }

        public async Task<ClientUser?> GetClientUserByIdAsync(int id)
        {
            return await _context.ClientUsers
                .Include(c => c.Role)
                .Include(c => c.Bank)
                .Include(c => c.VerificationStatus)
                .Include(c => c.Account)
                .Include(c => c.Beneficiaries)
                .Include(c => c.Employees)
                .FirstOrDefaultAsync(c => c.UserId == id && c.IsActive);
        }

        public async Task AddClientUserAsync(ClientUser clientUser)
        {
            clientUser.CreatedAt = DateTime.UtcNow;
            clientUser.UpdatedAt = DateTime.UtcNow;
            clientUser.IsActive = true;

            await _context.ClientUsers.AddAsync(clientUser);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClientUserAsync(ClientUser clientUser)
        {
            clientUser.UpdatedAt = DateTime.UtcNow;
            _context.ClientUsers.Update(clientUser);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteClientUserAsync(int id)
        {
            var clientUser = await _context.ClientUsers.FindAsync(id);
            if (clientUser == null) return false;

            clientUser.IsActive = false;
            clientUser.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // ✅ Business Logic Functions

        // 🔹 Returns all active ClientUsers belonging to a specific BankUser
        public async Task<IEnumerable<ClientUser>> GetClientsByBankUserIdAsync(int bankUserId)
        {
            return await _context.ClientUsers
                .Include(c => c.Role)
                .Include(c => c.VerificationStatus)
                .Include(c => c.Account)
                .Where(c => c.IsActive && c.BankUserId == bankUserId)
                .ToListAsync();
        }

        // 🔹 Returns a specific ClientUser by ClientId + BankUserId
        public async Task<ClientUser?> GetClientByBankUserIdAsync(int clientId, int bankUserId)
        {
            return await _context.ClientUsers
                .Include(c => c.Role)
                .Include(c => c.VerificationStatus)
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c =>
                    c.UserId == clientId &&
                    c.BankUserId == bankUserId &&
                    c.IsActive);
        }

        // 🔹 Checks if a ClientUser is active
        public async Task<bool> IsActiveClientUserAsync(int userId)
        {
            return await _context.ClientUsers
                .AnyAsync(c => c.UserId == userId && c.IsActive);
        }
    }
}
