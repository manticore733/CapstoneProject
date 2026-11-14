using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IReportRecordRepository
    {
        Task AddAsync(ReportRecord record);
        Task<IEnumerable<ReportRecord>> GetByUserIdAsync(int userId);
    }
}
