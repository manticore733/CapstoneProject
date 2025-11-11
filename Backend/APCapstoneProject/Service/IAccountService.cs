using APCapstoneProject.DTO.Account;

namespace APCapstoneProject.Service
{
    public interface IAccountService
    {
        Task<IEnumerable<ReadAccountDto>> GetAllAccountsAsync();
        Task<ReadAccountDto?> GetAccountByIdAsync(int id);

        Task<bool> CreditAsync(int accountId, decimal amount);
        Task<bool> DebitAsync(int accountId, decimal amount);

        // --- ADD THIS LINE ---
        Task<ReadAccountDto?> GetAccountByClientUserIdAsync(int clientUserId);
    }
}
