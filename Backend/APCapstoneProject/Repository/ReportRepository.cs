using APCapstoneProject.Data;
using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Repository
{
    public class ReportRecordRepository : IReportRecordRepository
    {
        private readonly BankingAppDbContext _context;

        public ReportRecordRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReportRecord record)
        {
            _context.ReportRecords.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReportRecord>> GetByUserIdAsync(int userId)
        {
            return await _context.ReportRecords
                .Where(r => r.RequestedByUserId == userId)
                .OrderByDescending(r => r.GeneratedAt)
                .ToListAsync();
        }
    }
}
    