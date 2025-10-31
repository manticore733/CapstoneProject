using APCapstoneProject.Data;
using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Repository
{
    public class SalaryDisbursementRepository : ISalaryDisbursementRepository
    {
        private readonly BankingAppDbContext _context;

        public SalaryDisbursementRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SalaryDisbursement disbursement)
        {
            await _context.SalaryDisbursements.AddAsync(disbursement);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SalaryDisbursement disbursement)
        {
            _context.SalaryDisbursements.Update(disbursement);
            await _context.SaveChangesAsync();
        }

        public async Task<SalaryDisbursement?> GetByIdAsync(int id)
        {
            return await _context.SalaryDisbursements
                .Include(s => s.ClientUser)
                .Include(s => s.TransactionStatus)
                .Include(s => s.Details)
                    .ThenInclude(d => d.Employee)
                .FirstOrDefaultAsync(s => s.TransactionId == id);
        }

        public async Task<IEnumerable<SalaryDisbursement>> GetByClientUserIdAsync(int clientUserId)
        {
            return await _context.SalaryDisbursements
                .Include(s => s.TransactionStatus)
                .Include(s => s.Details)
                .Where(s => s.ClientUserId == clientUserId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SalaryDisbursement>> GetPendingByBankUserIdAsync(int bankUserId)
        {
            return await _context.SalaryDisbursements
                .Include(s => s.ClientUser)
                .Include(s => s.TransactionStatus)
                .Where(s => s.ClientUser.BankUserId == bankUserId && s.StatusId == 0)
                .ToListAsync();
        }
    }
}
