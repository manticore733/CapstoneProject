using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(int id);
        Task<Account?> GetByClientIdAsync(int clientUserId);

        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task<bool> SaveChangesAsync();
    }
}
