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
        // included only to start a transaction in the EF so we can rollback if either of these 2 actions fail:
        // 1. Approving clientUser
        // 2. Creation of clientUser account.
        private readonly BankingAppDbContext _context;
        private readonly IEmailService _emailService;

        public BankUserService(IBankUserRepository bankUserRepo, BankingAppDbContext context, IClientUserRepository clientUserRepo, IBankRepository bankRepo, IAccountRepository accRepo, IMapper mapper, IEmailService emailService)
        {
            _bankUserRepo = bankUserRepo;
            _clientUserRepo = clientUserRepo;
            _bankRepo = bankRepo;
            _accountRepo = accRepo;
            _mapper = mapper;
            _context = context;
            _emailService = emailService;
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



        public async Task<ReadClientUserDto?> ApproveClientUserAsync(int clientUserId, int bankUserId, ClientApprovalDto dto)
        {
            //  Verify the approving BankUser 
            var bankUser = await _bankUserRepo.GetBankUserByIdAsync(bankUserId);
            if (bankUser == null || bankUser.BankId == null)
            {
                throw new UnauthorizedAccessException("Approving user is not a valid Bank User or missing bank association.");
            }

            // Fetch the client to approve, ensuring ownership
            var client = await _clientUserRepo.GetClientByBankUserIdAsync(clientUserId, bankUserId);
            if (client == null)
            {
                throw new KeyNotFoundException("Client not found or does not belong to this Bank User.");
            }

            //  Check if already processed 
            if (client.StatusId != 0) // 0 is PENDING
            {
                throw new InvalidOperationException(
                    $"Client is already in status '{client.VerificationStatus?.StatusEnum.ToString() ?? client.StatusId.ToString()}'. Cannot re-process.");
            }

            //  Begin DB Transaction 
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Account? createdAccount = null;

                // Process Approval / Rejection 
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
                    client.Remark = dto.Remark;
                    await _clientUserRepo.UpdateClientUserAsync(client);
                }

                //  Commit transaction
                await transaction.CommitAsync();




                // Fetch and return updated client info 
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

                try
                {
                    var tokens = new Dictionary<string, string?>
                    {
                        ["FullName"] = updatedClient.UserFullName,
                        ["AccountNumber"] = resultDto.AccountNumber,
                        ["BankName"] = bankUser.Bank?.BankName,
                        ["ApprovedAt"] = DateTime.UtcNow.ToString("dd MMM yyyy, HH:mm"),
                        ["Remark"] = dto.Remark
                    };

                    if (dto.IsApproved)
                    {
                        await _emailService.SendTemplateEmailAsync(updatedClient.UserEmail,
                            "Your client account has been approved",
                            "ClientApproved.html",
                            tokens);
                    }
                    else
                    {
                        tokens["Remark"] = dto.Remark ?? "Not specified";
                        await _emailService.SendTemplateEmailAsync(updatedClient.UserEmail,
                            "Your client registration was not approved",
                            "ClientRejected.html",
                            tokens);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email send failed: {ex.Message}");
                }


                return resultDto;
            }
            catch
            {
                //  Rollback on any failure 
                await transaction.RollbackAsync();
                throw; 
            }
        }



        private string GenerateAccountNumber()
        {
          
            var random = new Random();
            var randomDigits = random.Next(10000000, 99999999).ToString();
            var randomChars = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            return $"BPA{randomDigits}{randomChars}";
        }
    }
}
