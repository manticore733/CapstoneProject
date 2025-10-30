//using APCapstoneProject.Model;

//namespace APCapstoneProject.Repository
//{
//    public interface IAccountRepository
//    {
//        Task<IEnumerable<Account>> GetByClientIdAsync(int clientUserId);
//        Task<Account?> GetByIdAndClientIdAsync(int accountId, int clientUserId);
//        Task<Account?> GetByIdAsync(int id);
//        Task<Account> CreateAccountAsync(Account account);
//        Task UpdateAsync(Account account);
//        Task<bool> DeleteAsync(int id);
//        Task<bool> ExistsForClientAsync(int clientUserId);
//    }
//}










// new one not all crud operations basic add upadte 
using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IAccountRepository
    {
        Task AddAsync(Account account);
        Task<Account?> GetByIdAsync(int id);
        Task<Account?> GetByClientIdAsync(int clientUserId);
        void Update(Account account); // <-- ADD THIS
        Task<bool> SaveChangesAsync();


        Task<Account?> GetByIdAndClientIdAsync(int id, int clientUserId);
        Task<bool> SoftDeleteAsync(int id);

    }
}
