using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IBankUserRepository
    {
        Task<IEnumerable<BankUser>> GetBankUsersAsync();
        Task<BankUser?> GetBankUserByIdAsync(int id);
        Task AddBankUserAsync(BankUser user);
        Task UpdateBankUserAsync(BankUser user);
        Task<bool> DeleteBankUserAsync(int id);

    }
}
