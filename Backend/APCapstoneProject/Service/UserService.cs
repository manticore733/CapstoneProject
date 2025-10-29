using APCapstoneProject.DTO.User;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using AutoMapper;
using BCrypt.Net;

namespace APCapstoneProject.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IBankRepository _bankRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepo, IBankRepository bankRepo, IAccountRepository accRepo IMapper mapper)
        {
            _userRepo = userRepo;
            _bankRepo = bankRepo;
            _accountRepo = accRepo;
            _mapper = mapper;
        }

        // --- SUPERADMIN OPERATIONS ---

        public async Task<IEnumerable<UserReadDto>> GetAllAsync()
        {
            var users = await _userRepo.GetUsersAsync();
            return _mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        public async Task<UserReadDto?> GetByIdAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserReadDto>(user);
        }

        public async Task<UserReadDto> CreateAsync(UserCreateDto userCreateDto)
        {
            if (userCreateDto.UserRoleId != (int)Role.SUPER_ADMIN)
                throw new ArgumentException("This endpoint can only be used by Super Admins.");

            var user = _mapper.Map<User>(userCreateDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreateDto.PasswordHash);
            user.IsActive = true;
            user.BankId = null;

            await _userRepo.AddAsync(user);

            var created = await _userRepo.GetByIdAsync(user.UserId);
            return _mapper.Map<UserReadDto>(created);
        }

        //should only be used by superAdmins
        public async Task<UserReadDto?> UpdateAsync(int id, UserUpdateDto userUpdateDto)
        {
            var existing = await _userRepo.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(userUpdateDto, existing);
            await _userRepo.UpdateAsync(existing);

            var updated = await _userRepo.GetByIdAsync(existing.UserId);
            return _mapper.Map<UserReadDto>(updated);
        }

        //should only be used by superAdmins
        public async Task<bool> DeleteAsync(int id)
        {
            return await _userRepo.DeleteAsync(id);
        }


        // --- BANK USER MANAGEMENT ---

        public async Task<UserReadDto> CreateBankUserAsync(CreateBankUserDto dto)
        {
            if (!await _bankRepo.ExistsAsync(dto.BankId))
                throw new KeyNotFoundException($"Bank with ID {dto.BankId} not found.");

            var bankUser = _mapper.Map<BankUser>(dto);
            bankUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            bankUser.UserRoleId = (int)Role.BANK_USER;
            bankUser.IsActive = true;

            await _userRepo.AddAsync(bankUser);

            var created = await _userRepo.GetByIdAsync(bankUser.UserId);
            return _mapper.Map<UserReadDto>(created);
        }

        public async Task<UserReadDto?> UpdateBankUserAsync(int id, UpdateBankUserDto dto)
        {
            var existing = await _userRepo.GetByIdAsync(id);
            if (existing == null || existing is not BankUser bankUser) return null;

            _mapper.Map(dto, bankUser);
            await _userRepo.UpdateAsync(bankUser);

            var updated = await _userRepo.GetByIdAsync(bankUser.UserId);
            return _mapper.Map<UserReadDto>(updated);
        }






        public async Task<ClientStatusReadDto?> ApproveClientUserAsync(int clientUserId, int bankUserId, ClientApprovalDto dto)
        {
            // Step 1: Verify the approving BankUser
            var bankUser = await _userRepo.GetByIdAsync(bankUserId);
            if (bankUser == null || bankUser is not BankUser)
                throw new UnauthorizedAccessException("Only valid Bank Users can approve clients.");

            // Step 2: Fetch the client to approve
            var client = await _userRepo.GetClientByBankUserIdAsync(clientUserId, bankUserId);
            if (client == null)
                throw new KeyNotFoundException("Client not found or does not belong to this Bank User.");

            // Step 3: Check if account already exists
            if (await _accountRepo.ExistsForClientAsync(clientUserId))
                throw new InvalidOperationException("This client already has an account.");

            // Step 4: Update client’s verification status
            if (dto.IsApproved)
            {
                client.StatusId = 1; // APPROVED
                client.IsActive = true;

                // Step 5: Auto-create account
                var account = new Account
                {
                    ClientUserId = clientUserId,
                    BankId = bankUser.BankId!.Value, // from BankUser
                    Balance = dto.InitialBalance,
                    AccountNumber = GenerateAccountNumber(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _accountRepo.CreateAccountAsync(account);
            }
            else
            {
                client.StatusId = 2; // REJECTED
            }

            // Step 6: Save changes
            await _userRepo.UpdateAsync(client);
            var updatedClient = await _userRepo.GetByIdAsync(client.UserId);

            return _mapper.Map<ClientStatusReadDto>(updatedClient);
        }


        private string GenerateAccountNumber()
        {
            var random = new Random();
            return $"BPA{random.Next(10000000, 99999999)}{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        }









        // --- CLIENT USER MANAGEMENT ---

        public async Task<UserReadDto> CreateClientUserAsync(CreateClientUserDto dto, int creatorBankUserId)
        {
            var clientUser = _mapper.Map<ClientUser>(dto);
            clientUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            clientUser.UserRoleId = (int)Role.CLIENT_USER;
            clientUser.BankUserId = creatorBankUserId;
            clientUser.StatusId = 0; // pending
            clientUser.IsActive = true;

            await _userRepo.AddAsync(clientUser);

            var created = await _userRepo.GetByIdAsync(clientUser.UserId);
            return _mapper.Map<UserReadDto>(created);
        }

        public async Task<IEnumerable<UserReadDto>> GetClientsForBankUserAsync(int bankUserId)
        {
            var clients = await _userRepo.GetClientsByBankUserIdAsync(bankUserId);
            return _mapper.Map<IEnumerable<UserReadDto>>(clients);
        }

        public async Task<UserReadDto?> GetClientForBankUserAsync(int clientId, int bankUserId)
        {
            var client = await _userRepo.GetClientByBankUserIdAsync(clientId, bankUserId);
            return client == null ? null : _mapper.Map<UserReadDto>(client);
        }

        public async Task<UserReadDto?> UpdateClientUserAsync(int clientId, UpdateClientUserDto dto, int bankUserId)
        {
            var existing = await _userRepo.GetClientByBankUserIdAsync(clientId, bankUserId);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            await _userRepo.UpdateAsync(existing);

            var updated = await _userRepo.GetByIdAsync(existing.UserId);
            return _mapper.Map<UserReadDto>(updated);
        }

        public async Task<bool> DeleteClientUserAsync(int clientId, int bankUserId)
        {
            var client = await _userRepo.GetClientByBankUserIdAsync(clientId, bankUserId);
            if (client == null) return false;

            return await _userRepo.DeleteAsync(clientId);
        }






        //




        
    }
}
