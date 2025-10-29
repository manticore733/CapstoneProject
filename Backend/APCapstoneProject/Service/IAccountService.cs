using APCapstoneProject.DTO.Account;

namespace APCapstoneProject.Service
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountReadDto>> GetAccountsByClientIdAsync(int clientUserId);
        Task<AccountReadDto?> GetAccountByIdAsync(int id, int clientUserId);
        //Task<AccountReadDto> CreateAccountAsync(CreateAccountDto dto, int clientUserId);
        //Task<bool> UpdateAccountAsync(int id, UpdateAccountDto dto, int clientUserId);
        Task<bool> DeleteAccountAsync(int id, int clientUserId);
    }
}
