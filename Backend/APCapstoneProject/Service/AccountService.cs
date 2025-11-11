using APCapstoneProject.DTO.Account;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using AutoMapper;

namespace APCapstoneProject.Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepo, IMapper mapper)
        {
            _accountRepo = accountRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReadAccountDto>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<ReadAccountDto>>(accounts);
        }

        public async Task<ReadAccountDto?> GetAccountByIdAsync(int id)
        {
            var account = await _accountRepo.GetByIdAsync(id);
            return account == null ? null : _mapper.Map<ReadAccountDto>(account);
        }

        public async Task<bool> CreditAsync(int accountId, decimal amount)
        {
            var account = await _accountRepo.GetByIdAsync(accountId);
            if (account == null || !account.IsActive) return false;

            account.Balance += amount;
            await _accountRepo.UpdateAsync(account);
            return true;
        }

        public async Task<bool> DebitAsync(int accountId, decimal amount)
        {
            var account = await _accountRepo.GetByIdAsync(accountId);
            if (account == null || !account.IsActive || account.Balance < amount)
                return false;

            account.Balance -= amount;
            await _accountRepo.UpdateAsync(account);
            return true;
        }





        // --- ADD THIS ENTIRE METHOD ---
        public async Task<ReadAccountDto?> GetAccountByClientUserIdAsync(int clientUserId)
        {
            var account = await _accountRepo.GetByClientIdAsync(clientUserId);
            if (account == null) return null;

            // Map it to the DTO to return to the frontend
            return _mapper.Map<ReadAccountDto>(account);
        }


    }
}
