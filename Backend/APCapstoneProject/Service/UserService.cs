
//using APCapstoneProject.DTO.User;
//using APCapstoneProject.Model;
//using APCapstoneProject.Repository;
//using AutoMapper;

//namespace APCapstoneProject.Service
//{
//    public class UserService : IUserService
//    {
//        private readonly IUserRepository _repository;
//        private readonly IMapper _mapper;

//        public UserService(IUserRepository repository, IMapper mapper)
//        {
//            _repository = repository;
//            _mapper = mapper;
//        }

//        public async Task<IEnumerable<UserReadDto>> GetAllAsync()
//        {
//            var users = await _repository.GetAllAsync();
//            return _mapper.Map<IEnumerable<UserReadDto>>(users);
//        }

//        public async Task<UserReadDto?> GetByIdAsync(int id)
//        {
//            var user = await _repository.GetByIdAsync(id);
//            if (user == null) return null;
//            return _mapper.Map<UserReadDto>(user);
//        }

//        public async Task<UserReadDto> CreateAsync(UserCreateDto userCreateDto)
//        {
//            // Role-based validation
//            if (userCreateDto.UserRoleId == (int)Role.BANK_USER)
//            {
//                if (userCreateDto.BankId == null)
//                    throw new ArgumentException("Bank User must have a BankId.");
//            }
//            else
//            {
//                userCreateDto.BankId = null;
//            }

//            var user = _mapper.Map<User>(userCreateDto);
//            user.CreatedAt = DateTime.UtcNow;

//            // 1. Add the user to the context
//            await _repository.AddAsync(user);

//            // 2. Save changes to the database
//            await _repository.SaveChangesAsync();

//            // 3. Re-fetch the user to correctly load the Role navigation property
//            var createdUserWithRole = await _repository.GetByIdAsync(user.UserId);

//            // 4. Map the complete user object to the DTO for the response
//            return _mapper.Map<UserReadDto>(createdUserWithRole);
//        }

//        public async Task<bool> UpdateAsync(int id, UserUpdateDto userUpdateDto)
//        {
//            var existing = await _repository.GetByIdAsync(id);
//            if (existing == null) return false;

//            _mapper.Map(userUpdateDto, existing);
//            existing.UpdatedAt = DateTime.UtcNow;

//            // Call the synchronous Update method from the repository
//            _repository.Update(existing);

//            // Save changes here, at the end of the operation
//            return await _repository.SaveChangesAsync();
//        }

//        public async Task<bool> SoftDeleteAsync(int id)
//        {
//            return await _repository.SoftDeleteAsync(id);
//        }









//    }
//}































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
        private readonly IBankRepository _bankRepo; // <-- ADD THIS
        private readonly IMapper _mapper;

        // --- UPDATE THE CONSTRUCTOR ---
        public UserService(IUserRepository userRepository, IBankRepository bankRepository, IMapper mapper)
        {
            _userRepo = userRepository;
            _bankRepo = bankRepository; // <-- ADD THIS
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserReadDto>> GetAllAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        public async Task<UserReadDto?> GetByIdAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return null;
            return _mapper.Map<UserReadDto>(user);
        }

        // --- THIS METHOD IS NOW MODIFIED ---
        // It's now ONLY for Super Admins to create other Super Admins
        public async Task<UserReadDto> CreateAsync(UserCreateDto userCreateDto)
        {
            // 1. Validation: Only for Super Admins
            if (userCreateDto.UserRoleId != (int)Role.SUPER_ADMIN)
            {
                throw new ArgumentException("This endpoint is only for creating Super Admins.");
            }

            var user = _mapper.Map<User>(userCreateDto);

            // 2. Hash Password: We assume the DTO's 'PasswordHash' field contains plain text
            //    (You should rename PasswordHash to Password in UserCreateDto later)
            //user.PasswordHash = BCrypt.HashPassword(userCreateDto.PasswordHash);
            user .PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreateDto.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;
            user.BankId = null; // Super Admins are not tied to a bank
            user.IsActive = true;

            // 3. Add and Save
            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            // 4. Re-fetch and return
            var createdUserWithRole = await _userRepo.GetByIdAsync(user.UserId);
            return _mapper.Map<UserReadDto>(createdUserWithRole);
        }

        public async Task<bool> UpdateAsync(int id, UserUpdateDto userUpdateDto)
        {
            var existing = await _userRepo.GetByIdAsync(id);
            if (existing == null) return false;

            _mapper.Map(userUpdateDto, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            _userRepo.Update(existing);
            return await _userRepo.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _userRepo.SoftDeleteAsync(id);
        }

        // --- ADD ALL THESE NEW METHODS ---

        #region Bank User Management

        public async Task<UserReadDto> CreateBankUserAsync(CreateBankUserDto bankUserDto)
        {
            // 1. Validate: Check if the bank exists
            if (!await _bankRepo.ExistsAsync(bankUserDto.BankId))
            {
                throw new KeyNotFoundException($"Bank with ID {bankUserDto.BankId} not found.");
            }

            // 2. Map DTO to Model
            var bankUser = _mapper.Map<BankUser>(bankUserDto);

            // 3. Set Logic
            //bankUser.PasswordHash = BCrypt.HashPassword(bankUserDto.Password);
            bankUser.PasswordHash =   BCrypt.Net.BCrypt.HashPassword(bankUserDto.Password);
            bankUser.UserRoleId = (int)Role.BANK_USER;
            bankUser.CreatedAt = DateTime.UtcNow;
            bankUser.IsActive = true;

            // 4. Save
            await _userRepo.AddAsync(bankUser);
            await _userRepo.SaveChangesAsync();

            // 5. Re-fetch and return DTO
            var createdUser = await _userRepo.GetByIdAsync(bankUser.UserId);
            return _mapper.Map<UserReadDto>(createdUser);
        }

        public async Task<bool> UpdateBankUserAsync(int id, UpdateBankUserDto updateDto)
        {
            var existing = await _userRepo.GetByIdAsync(id);
            // Check if it exists and is the correct type
            if (existing == null || !(existing is BankUser)) return false;

            var bankUser = existing as BankUser;
            _mapper.Map(updateDto, bankUser); // AutoMapper handles nulls
            _userRepo.Update(bankUser);
            return await _userRepo.SaveChangesAsync();
        }

        #endregion

        #region Client User Management

        public async Task<UserReadDto> CreateClientUserAsync(CreateClientUserDto clientUserDto, int creatorBankUserId)
        {
            // 1. Map DTO to Model
            var clientUser = _mapper.Map<ClientUser>(clientUserDto);

            // 2. Set Logic
            clientUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(clientUserDto.Password);
            clientUser.UserRoleId = (int)Role.CLIENT_USER;
            clientUser.BankUserId = creatorBankUserId; // Link to the BankUser who created them
            clientUser.EstablishmentDate = DateTime.UtcNow;
            clientUser.StatusId = 0; // Set to PENDING (based on our seed data ID 0)
            clientUser.CreatedAt = DateTime.UtcNow;
            clientUser.IsActive = true; // Or 'false' if they need approval first

            // 3. Save
            await _userRepo.AddAsync(clientUser);
            await _userRepo.SaveChangesAsync();

            // 4. Re-fetch and return DTO
            var createdUser = await _userRepo.GetByIdAsync(clientUser.UserId);
            return _mapper.Map<UserReadDto>(createdUser);
        }

        public async Task<IEnumerable<UserReadDto>> GetClientsForBankUserAsync(int bankUserId)
        {
            var clients = await _userRepo.GetClientsByBankUserIdAsync(bankUserId);
            return _mapper.Map<IEnumerable<UserReadDto>>(clients);
        }

        public async Task<UserReadDto?> GetClientForBankUserAsync(int clientId, int bankUserId)
        {
            var client = await _userRepo.GetClientByBankUserIdAsync(clientId, bankUserId);
            if (client == null) return null;
            return _mapper.Map<UserReadDto>(client);
        }

        public async Task<bool> UpdateClientUserAsync(int id, UpdateClientUserDto updateDto, int bankUserId)
        {
            // Get *only* a client that belongs to this bank user
            var existing = await _userRepo.GetClientByBankUserIdAsync(id, bankUserId);
            if (existing == null) return false; // Not found OR doesn't belong to this user

            _mapper.Map(updateDto, existing);
            _userRepo.Update(existing);
            return await _userRepo.SaveChangesAsync();
        }

        #endregion
    }
}