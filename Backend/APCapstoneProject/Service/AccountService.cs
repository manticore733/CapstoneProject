using APCapstoneProject.DTO.Account;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using AutoMapper;

namespace APCapstoneProject.Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository repository, IUserRepository userRepository, IMapper mapper)
        {
            _repository = repository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountReadDto>> GetAccountsByClientIdAsync(int clientUserId)
        {
            var accounts = await _repository.GetByClientIdAsync(clientUserId);
            return _mapper.Map<IEnumerable<AccountReadDto>>(accounts);
        }

        public async Task<AccountReadDto?> GetAccountByIdAsync(int id, int clientUserId)
        {
            var account = await _repository.GetByIdAndClientIdAsync(id, clientUserId);
            return account == null ? null : _mapper.Map<AccountReadDto>(account);
        }

        public async Task<AccountReadDto> CreateAccountAsync(CreateAccountDto dto, int clientUserId)
        {
            var isClientUser = await _userRepository.IsActiveClientUserAsync(clientUserId);
            if (!isClientUser)
                throw new KeyNotFoundException($"No active ClientUser found with ID '{clientUserId}'.");

            // Check if user already has an account (1:1 rule)
            var existing = await _repository.GetByClientIdAsync(clientUserId);
            if (existing.Any())
                throw new InvalidOperationException("ClientUser already has an account.");

            var account = _mapper.Map<Account>(dto);
            account.ClientUserId = clientUserId;
            account.AccountNumber = GenerateAccountNumber();
            account.StatusId = 1; // e.g. Active

            await _repository.AddAsync(account);
            return _mapper.Map<AccountReadDto>(account);
        }

        public async Task<bool> UpdateAccountAsync(int id, UpdateAccountDto dto, int clientUserId)
        {
            var existing = await _repository.GetByIdAndClientIdAsync(id, clientUserId);
            if (existing == null) return false;

            _mapper.Map(dto, existing);
            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAccountAsync(int id, int clientUserId)
        {
            var existing = await _repository.GetByIdAndClientIdAsync(id, clientUserId);
            if (existing == null) return false;

            return await _repository.DeleteAsync(id);
        }

        private string GenerateAccountNumber()
        {
            var random = new Random();
            string eightDigits = random.Next(10000000, 99999999).ToString();
            string sixAlphanumeric = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            return $"BPA{eightDigits}{sixAlphanumeric}";
        }
    }
}
