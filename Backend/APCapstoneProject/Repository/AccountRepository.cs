using APCapstoneProject.Data;
using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingAppDbContext _context;

        public AccountRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        // 🔹 Get all active accounts (for BankUser/Admin)
        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _context.Accounts
                .Include(a => a.ClientUser)
                .Include(a => a.Bank)
                .Where(a => a.IsActive)
                .ToListAsync();
        }

        // 🔹 Get account by ID
        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _context.Accounts
                .Include(a => a.ClientUser)
                .Include(a => a.Bank)
                .FirstOrDefaultAsync(a => a.AccountId == id && a.IsActive);
        }

        // 🔹 Get account by Client ID
        public async Task<Account?> GetByClientIdAsync(int clientUserId)
        {
            return await _context.Accounts
                .Include(a => a.ClientUser) 
                .Include(a => a.Bank)    
                .FirstOrDefaultAsync(a => a.ClientUserId == clientUserId && a.IsActive);
        }

        // 🔹 Credit / Debit
        public async Task UpdateAsync(Account account)
        {
            account.UpdatedAt = DateTime.UtcNow;
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
