using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetByClientIdAsync(int clientUserId);
        Task<Account?> GetByIdAndClientIdAsync(int accountId, int clientUserId);
        Task<Account?> GetByIdAsync(int id);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task<bool> DeleteAsync(int id);
    }
}
