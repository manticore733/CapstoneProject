using APCapstoneProject.Model;

namespace APCapstoneProject.Service
{
    public interface IBankService
    {
        Task<IEnumerable<Bank>> GetAllBanksAsync();
        Task<Bank?> GetBankByIdAsync(int id);
        Task<Bank> CreateBankAsync(Bank bank);
        Task UpdateBankAsync(int id, Bank bank);
        Task DeleteBankAsync(int id);
    }
}
