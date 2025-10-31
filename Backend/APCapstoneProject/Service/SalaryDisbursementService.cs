using APCapstoneProject.Data;
using APCapstoneProject.DTO.SalaryDisbursement;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using AutoMapper;

namespace APCapstoneProject.Service
{
    public class SalaryDisbursementService : ISalaryDisbursementService
    {
        private readonly ISalaryDisbursementRepository _disbRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IClientUserRepository _clientRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IMapper _mapper;
        private readonly BankingAppDbContext _context;

        public SalaryDisbursementService(
            ISalaryDisbursementRepository disbRepo,
            IEmployeeRepository employeeRepo,
            IClientUserRepository clientRepo,
            IAccountRepository accountRepo,
            IMapper mapper,
            BankingAppDbContext context)
        {
            _disbRepo = disbRepo;
            _employeeRepo = employeeRepo;
            _clientRepo = clientRepo;
            _accountRepo = accountRepo;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ReadSalaryDisbursementDto> CreateSalaryDisbursementAsync(int clientUserId, CreateSalaryDisbursementDto dto)
        {
            var client = await _clientRepo.GetClientUserByIdAsync(clientUserId);
            if (client == null || client.StatusId != 1)
                throw new InvalidOperationException("Client not approved or not found.");

            // ✅ Verify employee ownership using your repository method
            var employee = await _employeeRepo.GetByIdAndClientIdAsync(dto.EmployeeId, clientUserId);
            if (employee == null)
                throw new UnauthorizedAccessException("Employee does not belong to this client user.");

            var disbursement = new SalaryDisbursement
            {
                ClientUserId = clientUserId,
                Amount = employee.Salary,
                TransactionTypeId = 1, // DEBIT
                StatusId = 0, // PENDING
                AccountId = client.Account.AccountId,
                BankName = employee.BankName,
                IFSC = employee.IFSC,
                DestinationAccountNumber = employee.AccountNumber,
                TotalAmount = employee.Salary,
                Remarks = dto.Remarks,
                CreatedAt = DateTime.UtcNow,
                Details = new List<SalaryDisbursementDetail>
                {
                    new SalaryDisbursementDetail
                    {
                        EmployeeId = employee.EmployeeId,
                        Amount = employee.Salary,
                        Remark = "Salary pending approval"
                    }
                }
            };

            await _disbRepo.AddAsync(disbursement);
            return _mapper.Map<ReadSalaryDisbursementDto>(disbursement);
        }

        public async Task<IEnumerable<ReadSalaryDisbursementDto>> GetSalaryDisbursementsByClientUserIdAsync(int clientUserId)
        {
            var disb = await _disbRepo.GetByClientUserIdAsync(clientUserId);
            return _mapper.Map<IEnumerable<ReadSalaryDisbursementDto>>(disb);
        }

        public async Task<IEnumerable<ReadSalaryDisbursementDto>> GetPendingSalaryDisbursementsByBankUserIdAsync(int bankUserId)
        {
            var disb = await _disbRepo.GetPendingByBankUserIdAsync(bankUserId);
            return _mapper.Map<IEnumerable<ReadSalaryDisbursementDto>>(disb);
        }

        public async Task<ReadSalaryDisbursementDto?> ApproveSalaryDisbursementAsync(int disbursementId, int bankUserId)
        {
            var disb = await _disbRepo.GetByIdAsync(disbursementId);
            if (disb == null || disb.StatusId != 0)
                return null;

            var client = await _clientRepo.GetClientUserByIdAsync(disb.ClientUserId);
            if (client == null || client.BankUserId != bankUserId)
                throw new UnauthorizedAccessException("This disbursement does not belong to this bank user.");

            var account = client.Account!;
            if (account.Balance < disb.TotalAmount)
                throw new InvalidOperationException("Insufficient balance for disbursement.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                account.Balance -= disb.TotalAmount;
                await _accountRepo.UpdateAsync(account);

                disb.StatusId = 1; // APPROVED
                disb.ProcessedAt = DateTime.UtcNow;

                if (disb.Details != null)
                {
                    foreach (var detail in disb.Details)
                    {
                        detail.Success = true;
                        detail.Remark = "Disbursed successfully";
                        detail.ProcessedAt = DateTime.UtcNow;
                    }
                }

                await _disbRepo.UpdateAsync(disb);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return _mapper.Map<ReadSalaryDisbursementDto>(disb);
        }

        public async Task<ReadSalaryDisbursementDto?> RejectSalaryDisbursementAsync(int disbursementId, int bankUserId)
        {
            var disb = await _disbRepo.GetByIdAsync(disbursementId);
            if (disb == null || disb.StatusId != 0)
                return null;

            var client = await _clientRepo.GetClientUserByIdAsync(disb.ClientUserId);
            if (client == null || client.BankUserId != bankUserId)
                throw new UnauthorizedAccessException("This disbursement does not belong to this bank user.");

            disb.StatusId = 2; // REJECTED
            disb.ProcessedAt = DateTime.UtcNow;

            if (disb.Details != null)
            {
                foreach (var detail in disb.Details)
                {
                    detail.Success = false;
                    detail.Remark = "Disbursement rejected";
                    detail.ProcessedAt = DateTime.UtcNow;
                }
            }

            await _disbRepo.UpdateAsync(disb);
            return _mapper.Map<ReadSalaryDisbursementDto>(disb);
        }
    }
}
