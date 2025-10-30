//using APCapstoneProject.Data;
//using APCapstoneProject.Model;
//using Microsoft.EntityFrameworkCore;

//namespace APCapstoneProject.Repository
//{
//    public class AccountRepository : IAccountRepository
//    {
//        private readonly BankingAppDbContext _context;

//        public AccountRepository(BankingAppDbContext context)
//        {
//            _context = context;
//        }

//        // 🔹 Get all accounts belonging to a specific ClientUser
//        public async Task<IEnumerable<Account>> GetByClientIdAsync(int clientUserId)
//        {
//            return await _context.Accounts
//                .Include(a => a.Bank)
//                .Include(a => a.ClientUser)
//                .Include(a => a.Transactions)
//                .Where(a => a.ClientUserId == clientUserId && a.IsActive)
//                .ToListAsync();
//        }

//        // 🔹 Get a specific account by ID and ownership check
//        public async Task<Account?> GetByIdAndClientIdAsync(int accountId, int clientUserId)
//        {
//            return await _context.Accounts
//                .Include(a => a.Bank)
//                .Include(a => a.ClientUser)
//                .Include(a => a.Transactions)
//                .FirstOrDefaultAsync(a => a.AccountId == accountId && a.ClientUserId == clientUserId && a.IsActive);
//        }

//        // 🔹 Get account by ID, wont be of use mostly
//        public async Task<Account?> GetByIdAsync(int id)
//        {
//            return await _context.Accounts
//                .Include(a => a.Bank)
//                .Include(a => a.ClientUser)
//                .Include(a => a.Transactions)
//                .FirstOrDefaultAsync(a => a.AccountId == id && a.IsActive);
//        }

//        // 🔹 Used in approval flow (returns created account)
//        public async Task<Account> CreateAccountAsync(Account account)
//        {
//            account.CreatedAt = DateTime.UtcNow;
//            account.UpdatedAt = DateTime.UtcNow;
//            account.IsActive = true;

//            await _context.Accounts.AddAsync(account);
//            await _context.SaveChangesAsync();
//            return account;
//        }

//        // 🔹 Prevent duplicate accounts for the same client
//        public async Task<bool> ExistsForClientAsync(int clientUserId)
//        {
//            return await _context.Accounts.AnyAsync(a => a.ClientUserId == clientUserId && a.IsActive);
//        }

//        // 🔹 Update existing account
//        public async Task UpdateAsync(Account account)
//        {
//            account.UpdatedAt = DateTime.UtcNow;
//            _context.Accounts.Update(account);
//            await _context.SaveChangesAsync();
//        }

//        // 🔹 Soft delete (mark inactive instead of remove)
//        public async Task<bool> DeleteAsync(int id)
//        {
//            var account = await _context.Accounts.FindAsync(id);
//            if (account == null) return false;

//            account.IsActive = false;
//            account.UpdatedAt = DateTime.UtcNow;
//            await _context.SaveChangesAsync();
//            return true;
//        }

//        private string GenerateAccountNumber()
//        {
//            var random = new Random();
//            return $"BPA{random.Next(10000000, 99999999)}{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
//        }
//    }
//}

















/// // New simplified version with basic Add, Get, Update, and SaveChanges methods



using APCapstoneProject.Data;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingAppDbContext _context;

        public AccountRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            // We must .Include() related data for the DTOs
            return await _context.Accounts
                .Include(a => a.Bank)
                .Include(a => a.ClientUser)
                .FirstOrDefaultAsync(a => a.AccountId == id && a.IsActive);
        }

        public async Task<Account?> GetByClientIdAsync(int clientUserId)
        {
            // We must .Include() related data for the DTOs
            return await _context.Accounts
                .Include(a => a.Bank)
                .Include(a => a.ClientUser)
                .FirstOrDefaultAsync(a => a.ClientUserId == clientUserId && a.IsActive);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

        public void Update(Account account)
        {
            _context.Accounts.Update(account);
        }

        // --- ADD THESE TWO METHODS ---

        public async Task<Account?> GetByIdAndClientIdAsync(int id, int clientUserId)
        {
            return await _context.Accounts
                .Include(a => a.Bank)
                .Include(a => a.ClientUser)
                .FirstOrDefaultAsync(a => a.AccountId == id && a.ClientUserId == clientUserId && a.IsActive);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return false;

            account.IsActive = false;
            account.UpdatedAt = DateTime.UtcNow;
            // A soft delete is a complete operation, so we save here.
            return (await _context.SaveChangesAsync() > 0);
        }





    }
}

