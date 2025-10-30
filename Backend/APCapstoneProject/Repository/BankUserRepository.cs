using APCapstoneProject.Data;
using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Repository
{
    public class BankUserRepository : IBankUserRepository
    {
        private readonly BankingAppDbContext _context;

        public BankUserRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BankUser>> GetBankUsersAsync()
        {
            return await _context.BankUsers
                .Where(u => u.IsActive)
                .Include(u => u.Role)
                .Include(u => u.Bank)
                .Include(u => u.Clients) // include to count clients
                .ToListAsync();
        }

        public async Task<BankUser?> GetBankUserByIdAsync(int id)
        {
            return await _context.BankUsers
                .Include(u => u.Role)
                .Include(u => u.Bank)
                .Include(u => u.Clients)
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);
        }

        public async Task AddBankUserAsync(BankUser user)
        {
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.BankUsers.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBankUserAsync(BankUser user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.BankUsers.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteBankUserAsync(int id)
        {
            var user = await _context.BankUsers.FindAsync(id);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
