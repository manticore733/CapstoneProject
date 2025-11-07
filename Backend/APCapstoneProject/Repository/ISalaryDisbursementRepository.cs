using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface ISalaryDisbursementRepository
    {
        Task AddAsync(SalaryDisbursement disbursement);
        Task UpdateAsync(SalaryDisbursement disbursement);
        Task AddDetailsAsync(IEnumerable<SalaryDisbursementDetail> details);
        Task<IEnumerable<SalaryDisbursement>> GetAllAsync();
        Task<IEnumerable<SalaryDisbursement>> GetByBankUserIdAsync(int bankUserId);

        Task<SalaryDisbursement?> GetByIdAsync(int id);
        Task<IEnumerable<SalaryDisbursement>> GetByClientUserIdAsync(int clientUserId);
        Task<IEnumerable<SalaryDisbursement>> GetPendingByBankUserIdAsync(int bankUserId);


        // ← NEW: Check if disbursement exists for this month
        //Task<SalaryDisbursement?> GetExistingMonthDisbursementAsync(int clientUserId, int month, int year);

        Task<List<int>> GetEmployeeDisbursementsInMonthAsync(int clientUserId, List<int> employeeIds, int month, int year);

    }
}
        


