using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetByClientIdAsync(int clientUserId);
        Task<Account?> GetByIdAndClientIdAsync(int accountId, int clientUserId);
        Task<Account?> GetByIdAsync(int id);
        Task<Account> CreateAccountAsync(Account account);
        Task UpdateAsync(Account account);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsForClientAsync(int clientUserId);
    }
}
