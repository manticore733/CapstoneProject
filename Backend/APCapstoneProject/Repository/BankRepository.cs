using APCapstoneProject.Data;
using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace APCapstoneProject.Repository
{
    public class BankRepository: IBankRepository
    {

        private readonly BankingAppDbContext _context;

        public BankRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bank>> GetAllAsync()
        {
            return await _context.Banks
                .Include(b => b.Users)
                .Include(b => b.Accounts)
                .Where(b => b.IsActive)
                .ToListAsync();
        }

        public async Task<Bank?> GetByIdAsync(int id)
        {
            return await _context.Banks
                .Include(b => b.Users)
                .Include(b => b.Accounts)
                .FirstOrDefaultAsync(b => b.BankId == id && b.IsActive);
        }

        public async Task<Bank> AddAsync(Bank bank)
        {
            bank.CreatedAt = DateTime.UtcNow;
            bank.UpdatedAt = DateTime.UtcNow;
            _context.Banks.Add(bank);
            await _context.SaveChangesAsync();
            return bank;
        }

        public async Task UpdateAsync(Bank bank)
        {
            bank.UpdatedAt = DateTime.UtcNow;
            _context.Banks.Update(bank);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var bank = await _context.Banks.FindAsync(id);
            if (bank != null)
            {
                bank.IsActive = false;
                bank.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Banks.AnyAsync(b => b.BankId == id && b.IsActive);
        }
    }
}
