using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface ISalaryDisbursementRepository
    {
        Task AddAsync(SalaryDisbursement disbursement);
        Task UpdateAsync(SalaryDisbursement disbursement);
        Task<SalaryDisbursement?> GetByIdAsync(int id);
        Task<IEnumerable<SalaryDisbursement>> GetByClientUserIdAsync(int clientUserId);
        Task<IEnumerable<SalaryDisbursement>> GetPendingByBankUserIdAsync(int bankUserId);
    }
}
