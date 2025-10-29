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

        public async Task<IEnumerable<Account>> GetByClientIdAsync(int clientUserId)
        {
            return await _context.Accounts
                .Include(a => a.Bank)
                .Include(a => a.AccountType)
                .Include(a => a.AccountStatus)
                .Where(a => a.ClientUserId == clientUserId)
                .ToListAsync();
        }

        public async Task<Account?> GetByIdAndClientIdAsync(int accountId, int clientUserId)
        {
            return await _context.Accounts
                .Include(a => a.Bank)
                .Include(a => a.AccountType)
                .Include(a => a.AccountStatus)
                .FirstOrDefaultAsync(a => a.AccountId == accountId && a.ClientUserId == clientUserId);
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _context.Accounts
                .Include(a => a.Bank)
                .Include(a => a.AccountType)
                .Include(a => a.AccountStatus)
                .FirstOrDefaultAsync(a => a.AccountId == id);
        }

        public async Task AddAsync(Account account)
        {
            account.CreatedAt = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            account.UpdatedAt = DateTime.UtcNow;
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return false;

            // Instead of hard delete, mark as banned
            account.IsActive = false; // example: 3 = BANNED (based on your Status table)
            account.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
