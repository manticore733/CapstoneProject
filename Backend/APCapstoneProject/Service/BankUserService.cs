using APCapstoneProject.Data;
using APCapstoneProject.DTO.User;
using APCapstoneProject.DTO.User.BankUser;
using APCapstoneProject.DTO.User.ClientUser;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Service
{
    public class BankUserService: IBankUserService
    {

        private readonly IBankUserRepository _bankUserRepo;
        private readonly IClientUserRepository _clientUserRepo;
        private readonly IBankRepository _bankRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IMapper _mapper;
        private readonly BankingAppDbContext _context;

        public BankUserService(IBankUserRepository bankUserRepo, BankingAppDbContext context, IClientUserRepository clientUserRepo, IBankRepository bankRepo, IAccountRepository accRepo, IMapper mapper)
        {
            _bankUserRepo = bankUserRepo;
            _clientUserRepo = clientUserRepo;
            _bankRepo = bankRepo;
            _accountRepo = accRepo;
            _mapper = mapper;
            _context = context;
        }



        public async Task<ReadBankUserDto> CreateBankUserAsync(CreateBankUserDto dto)
        {
            if (!await _bankRepo.ExistsAsync(dto.BankId))
                throw new KeyNotFoundException($"Bank with ID {dto.BankId} not found.");

            var bankUser = _mapper.Map<BankUser>(dto);
            bankUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            bankUser.UserRoleId = (int)Role.BANK_USER;
            bankUser.IsActive = true;

            await _bankUserRepo.AddBankUserAsync(bankUser);

            var created = await _bankUserRepo.GetBankUserByIdAsync(bankUser.UserId);
            return _mapper.Map<ReadBankUserDto>(created);
        }

        public async Task<ReadBankUserDto?> UpdateBankUserAsync(int id, UpdateBankUserDto dto)
        {
            var existing = await _bankUserRepo.GetBankUserByIdAsync(id);
            if (existing == null || existing is not BankUser bankUser) return null;

            _mapper.Map(dto, bankUser);
            await _bankUserRepo.UpdateBankUserAsync(bankUser);

            var updated = await _bankUserRepo.GetBankUserByIdAsync(bankUser.UserId);
            return _mapper.Map<ReadBankUserDto>(updated);
        }


        public async Task<IEnumerable<ReadBankUserDto>> GetAllBankUsersAsync()
        {
            var users = await _bankUserRepo.GetBankUsersAsync();
            return _mapper.Map<IEnumerable<ReadBankUserDto>>(users);
        }

        public async Task<ReadBankUserDto?> GetBankUserByIdAsync(int id)
        {
            var user = await _bankUserRepo.GetBankUserByIdAsync(id);
            return user == null ? null : _mapper.Map<ReadBankUserDto>(user);
        }

        public async Task<bool> DeleteBankUserAsync(int id)
        {
            return await _bankUserRepo.DeleteBankUserAsync(id);
        }


        //public async Task<ReadClientUserDto?> ApproveClientUserAsync(int clientUserId, int bankUserId, ClientApprovalDto dto)
        //{
        //    // --- Step 1: Verify the approving BankUser ---
        //    var bankUser = await _bankUserRepo.GetBankUserByIdAsync(bankUserId);
        //    if (bankUser == null || !(bankUser is BankUser) || bankUser.BankId == null)
        //    {
        //        throw new UnauthorizedAccessException("Approving user is not a valid Bank User or is missing Bank association.");
        //    }

        //    // --- Step 2: Fetch the client to approve, ensuring ownership ---
        //    var client = await _clientUserRepo.GetClientByBankUserIdAsync(clientUserId, bankUserId);
        //    if (client == null)
        //    {
        //        throw new KeyNotFoundException("Client not found or does not belong to this Bank User.");
        //    }

        //    // --- Step 3: Check if already processed ---
        //    if (client.StatusId != 0) // Assuming 0 is PENDING
        //    {
        //        throw new InvalidOperationException($"Client is already in status '{client.VerificationStatus?.StatusEnum.ToString() ?? client.StatusId.ToString()}'. Cannot re-process.");
        //    }

        //    Account? createdAccount = null; // To hold account details if created

        //    // --- Step 4: Apply Approval/Rejection ---
        //    if (dto.IsApproved)
        //    {
        //        // Check if account already exists
        //        var existingAccount = await _accountRepo.GetByClientIdAsync(clientUserId);
        //        if (existingAccount != null)
        //        {
        //            throw new InvalidOperationException("This client unexpectedly already has an account.");
        //        }

        //        // Update client status
        //        client.StatusId = 1; // Assuming 1 is APPROVED
        //        client.IsActive = true;
        //        client.UpdatedAt = DateTime.UtcNow;


        //        // --- CHANGE 1: Call existing UpdateAsync which saves immediately ---
        //        await _clientUserRepo.UpdateClientUserAsync(client);

        //        // Create the account object
        //        var newAccount = new Account
        //        {
        //            ClientUserId = clientUserId,
        //            BankId = bankUser.BankId.Value,
        //            Balance = dto.InitialBalance,
        //            AccountNumber = GenerateAccountNumber(),
        //            IsActive = true,
        //            CreatedAt = DateTime.UtcNow
        //        };




        //        // --- CHANGE 2: Add and Save the account separately ---
        //        await _accountRepo.AddAsync(newAccount);
        //        bool accountSaved = await _accountRepo.SaveChangesAsync();
        //        if (!accountSaved)
        //        {
        //            // CRITICAL: Account creation failed AFTER client was approved.
        //            // Log this error. Manual intervention might be needed.
        //            // Consider throwing an exception to signal the partial failure.
        //            throw new DbUpdateException($"Client {clientUserId} was approved, but failed to create the associated account.");
        //        }
        //        createdAccount = newAccount; // Store for the return DTO
        //    }
        //    else // Client is Rejected
        //    {
        //        client.StatusId = 2; // Assuming 2 is REJECTED
        //                             //  client.IsActive = false;
        //        client.UpdatedAt = DateTime.UtcNow;

        //        // --- CHANGE 3: Call existing UpdateAsync which saves immediately ---
        //        await _clientUserRepo.UpdateClientUserAsync(client);
        //    }

        //    // --- Step 5: Fetch updated client data and return DTO ---
        //    // Re-fetch AFTER saving to ensure Status is updated
        //    var updatedClient = await _clientUserRepo.GetClientByBankUserIdAsync(clientUserId, bankUserId);
        //    if (updatedClient == null)
        //    {
        //        throw new KeyNotFoundException("Failed to retrieve updated client information after processing.");
        //    }

        //    // Manually add AccountNumber to DTO if account was created in this call
        //    var resultDto = _mapper.Map<ReadClientUserDto>(updatedClient);
        //    if (createdAccount != null)
        //    {
        //        resultDto.AccountNumber = createdAccount.AccountNumber;
        //    }
        //    else if (updatedClient.Account != null)
        //    {
        //        // If account existed previously (shouldn't happen with PENDING check)
        //        resultDto.AccountNumber = updatedClient.Account.AccountNumber;
        //    }


        //    return resultDto;
        //}


        public async Task<ReadClientUserDto?> ApproveClientUserAsync(int clientUserId, int bankUserId, ClientApprovalDto dto)
        {
            // --- Step 1: Verify the approving BankUser ---
            var bankUser = await _bankUserRepo.GetBankUserByIdAsync(bankUserId);
            if (bankUser == null || bankUser.BankId == null)
            {
                throw new UnauthorizedAccessException("Approving user is not a valid Bank User or missing bank association.");
            }

            // --- Step 2: Fetch the client to approve, ensuring ownership ---
            var client = await _clientUserRepo.GetClientByBankUserIdAsync(clientUserId, bankUserId);
            if (client == null)
            {
                throw new KeyNotFoundException("Client not found or does not belong to this Bank User.");
            }

            // --- Step 3: Check if already processed ---
            if (client.StatusId != 0) // 0 → PENDING
            {
                throw new InvalidOperationException(
                    $"Client is already in status '{client.VerificationStatus?.StatusEnum.ToString() ?? client.StatusId.ToString()}'. Cannot re-process.");
            }

            // --- Step 4: Begin DB Transaction ---
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Account? createdAccount = null;

                // --- Step 5: Process Approval / Rejection ---
                if (dto.IsApproved)
                {
                    // Ensure no account already exists
                    var existingAccount = await _accountRepo.GetByClientIdAsync(clientUserId);
                    if (existingAccount != null)
                        throw new InvalidOperationException("This client unexpectedly already has an account.");

                    // Update client approval
                    client.StatusId = 1; // APPROVED
                    client.IsActive = true;
                    client.BankId = bankUser.BankId.Value;
                    client.UpdatedAt = DateTime.UtcNow;
                    await _clientUserRepo.UpdateClientUserAsync(client);

                    // Create and save account
                    var newAccount = new Account
                    {
                        ClientUserId = clientUserId,
                        BankId = bankUser.BankId.Value,
                        Balance = dto.InitialBalance,
                        AccountNumber = GenerateAccountNumber(),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _accountRepo.AddAsync(newAccount);
                    await _accountRepo.SaveChangesAsync();
                    createdAccount = newAccount;
                }
                else
                {
                    // Reject client
                    client.StatusId = 2; // REJECTED
                    client.BankId = bankUser.BankId.Value;
                    client.UpdatedAt = DateTime.UtcNow;
                    await _clientUserRepo.UpdateClientUserAsync(client);
                }

                // --- Step 6: Commit transaction ---
                await transaction.CommitAsync();

                // --- Step 7: Fetch and return updated client info ---
                var updatedClient = await _clientUserRepo.GetClientByBankUserIdAsync(clientUserId, bankUserId);
                if (updatedClient == null)
                {
                    throw new KeyNotFoundException("Failed to retrieve updated client information after processing.");
                }

                var resultDto = _mapper.Map<ReadClientUserDto>(updatedClient);
                if (createdAccount != null)
                {
                    resultDto.AccountNumber = createdAccount.AccountNumber;
                }
                else if (updatedClient.Account != null)
                {
                    resultDto.AccountNumber = updatedClient.Account.AccountNumber;
                }

                return resultDto;
            }
            catch
            {
                // --- Step 8: Rollback on any failure ---
                await transaction.RollbackAsync();
                throw; // rethrow to controller for proper error response
            }
        }



        private string GenerateAccountNumber()
        {
            // Your existing logic - ensures format matches ^BPA\d{8}[A-Z0-9]{6}$
            var random = new Random();
            var randomDigits = random.Next(10000000, 99999999).ToString();
            // Using Guid for better randomness than Substring
            var randomChars = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            return $"BPA{randomDigits}{randomChars}";
        }
    }
}
