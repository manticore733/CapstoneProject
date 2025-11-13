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

        public async Task AddDetailsAsync(IEnumerable<SalaryDisbursementDetail> details)
        {
            await _context.SalaryDisbursementDetails.AddRangeAsync(details);
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
       
        public async Task<IEnumerable<SalaryDisbursement>> GetAllAsync()
        {
            return await _context.SalaryDisbursements
                .Include(s => s.ClientUser)
                .Include(s => s.TransactionStatus)
                .Include(s => s.Details)!
                    .ThenInclude(d => d.Employee)
                .ToListAsync();
        }

        

        public async Task<IEnumerable<SalaryDisbursement>> GetByBankUserIdAsync(int bankUserId)
        {
            return await _context.SalaryDisbursements
                .Include(s => s.ClientUser)
                .Include(s => s.TransactionStatus)
                .Include(s => s.Details)!
                    .ThenInclude(d => d.Employee)
                .Where(s => s.ClientUser != null && s.ClientUser.BankUserId == bankUserId)
                .ToListAsync();
        }


        public async Task<IEnumerable<SalaryDisbursement>> GetByClientUserIdAsync(int clientUserId)
        {
            return await _context.SalaryDisbursements
                .Include(s => s.TransactionStatus)
                .Include(s => s.Details)
                    .ThenInclude(d => d.Employee)
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




        //  Check if disbursement exists for specific month
        public async Task<SalaryDisbursement?> GetExistingMonthDisbursementAsync(int clientUserId, int month, int year)
        {
            return await _context.SalaryDisbursements
                .Where(s => s.ClientUserId == clientUserId &&
                            s.CreatedAt.Month == month &&
                            s.CreatedAt.Year == year &&
                            (s.StatusId == 0 || s.StatusId == 1))  // PENDING or APPROVED
                .FirstOrDefaultAsync();
        }


        public async Task<List<int>> GetEmployeeDisbursementsInMonthAsync(int clientUserId, List<int> employeeIds, int month, int year)
        {
            return await _context.SalaryDisbursementDetails
                .Where(d =>
                    d.SalaryDisbursement.ClientUserId == clientUserId &&
                    d.SalaryDisbursement.CreatedAt.Month == month &&
                    d.SalaryDisbursement.CreatedAt.Year == year &&
                    employeeIds.Contains(d.EmployeeId) &&
                    d.SalaryDisbursement.StatusId != 2 // skip REJECTED ones
                )
                .Select(d => d.EmployeeId)
                .Distinct()
                .ToListAsync();
        }



    }
}
