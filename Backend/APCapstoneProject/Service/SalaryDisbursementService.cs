using APCapstoneProject.Data;
using APCapstoneProject.DTO.SalaryDisbursement;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Service
{
    public class SalaryDisbursementService : ISalaryDisbursementService
    {
        private readonly ISalaryDisbursementRepository _disbursementRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IClientUserRepository _clientRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IMapper _mapper;
        // included only to start a transaction in the EF so we can rollback if either of these 2 actions fail:
        // 1. Approving salary disbursement status change
        // 2. deduction of money
        private readonly BankingAppDbContext _context;
        private readonly IEmailService _emailService;

        public SalaryDisbursementService(
            ISalaryDisbursementRepository disbursementRepo,
            IEmployeeRepository employeeRepo,
            IClientUserRepository clientRepo,
            IAccountRepository accountRepo,
            IMapper mapper,
            BankingAppDbContext context,
            IEmailService emailService)
        {
            _disbursementRepo = disbursementRepo;
            _employeeRepo = employeeRepo;
            _clientRepo = clientRepo;
            _accountRepo = accountRepo;
            _mapper = mapper;
            _context = context;
            _emailService = emailService;
        }

        public async Task<ReadSalaryDisbursementDto> CreateSalaryDisbursementAsync(int clientUserId, CreateSalaryDisbursementDto dto)
        {
            var client = await _clientRepo.GetClientUserByIdAsync(clientUserId);
            if (client == null || client.StatusId != 1)
                throw new InvalidOperationException("Client not approved or not found.");

            var employees = new List<Employee>();

            // ✅ STEP 1: Parse CSV if provided
            if (dto.CsvFile != null)
            {
                using var reader = new StreamReader(dto.CsvFile.OpenReadStream());
                var employeeIds = new List<int>();

                Console.WriteLine($"📄 Received CSV file: {dto.CsvFile.FileName}, Length: {dto.CsvFile.Length} bytes");

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    Console.WriteLine($"➡️ Raw line read from CSV: '{line}'");

                    if (string.IsNullOrWhiteSpace(line)) continue;
                    line = line.Trim().Trim('\uFEFF');

                    foreach (var part in line.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (int.TryParse(part.Trim(), out int id))
                            employeeIds.Add(id);
                    }
                }

                Console.WriteLine($"✅ Parsed employee IDs: {string.Join(", ", employeeIds)}");
                dto.EmployeeIds = employeeIds;
            }

            // ✅ STEP 2: Normal employee selection logic (always executes)
            if (dto.AllEmployees)
            {
                employees = (await _employeeRepo.GetByClientIdAsync(clientUserId)).ToList();
            }
            else if (dto.EmployeeIds != null && dto.EmployeeIds.Any())
            {
                foreach (var id in dto.EmployeeIds)
                {
                    var emp = await _employeeRepo.GetByIdAndClientIdAsync(id, clientUserId);
                    if (emp != null) employees.Add(emp);
                }
            }
            else if (dto.EmployeeId.HasValue)
            {
                var emp = await _employeeRepo.GetByIdAndClientIdAsync(dto.EmployeeId.Value, clientUserId);
                if (emp != null) employees.Add(emp);
            }
            else
            {
                throw new InvalidOperationException("No valid employees specified.");
            }

            if (!employees.Any())
                throw new InvalidOperationException("No valid employees found.");

            // ✅ STEP 3: Validate account and balance
            var totalAmount = employees.Sum(e => e.Salary);
            var account = await _accountRepo.GetByClientIdAsync(clientUserId);
            if (account == null || account.Balance < totalAmount)
                throw new InvalidOperationException("Insufficient balance for salary disbursement.");

            // ✅ STEP 4: Create salary disbursement record
            var disbursement = new SalaryDisbursement
            {
                ClientUserId = clientUserId,
                TotalAmount = totalAmount,
                AllEmployees = dto.AllEmployees,
                TransactionTypeId = 1,
                StatusId = 0,
                AccountId = account.AccountId,
                Remarks = dto.Remarks,
                TotalEmployees = employees.Count,
                CreatedAt = DateTime.UtcNow
            };

            await _disbursementRepo.AddAsync(disbursement);

            // ✅ STEP 5: Create detail entries
            var details = employees.Select(e => new SalaryDisbursementDetail
            {
                SalaryDisbursementId = disbursement.TransactionId,
                EmployeeId = e.EmployeeId,
                BankName = e.BankName,
                IFSC = e.IFSC,
                DestinationAccountNumber = e.AccountNumber,
                Amount = e.Salary,
                Remark = "Awaiting approval by BankUser",
                IsSuccessful = null
            }).ToList();

            await _disbursementRepo.AddDetailsAsync(details);
            disbursement.Details = details;


            try
            {
                var tokens = new Dictionary<string, string?>
                {
                    ["FullName"] = client.UserFullName,
                    ["TransactionId"] = disbursement.TransactionId.ToString(),
                    ["TotalEmployees"] = disbursement.TotalEmployees.ToString(),
                    ["TotalAmount"] = disbursement.TotalAmount.ToString("F2"),
                    ["CreatedAt"] = disbursement.CreatedAt.ToString("dd MMM yyyy, HH:mm")
                };
                await _emailService.SendTemplateEmailAsync(client.UserEmail,
                    $"Salary disbursement request received for ₹{disbursement.TotalAmount:F2}",
                    "SalaryDisbursementRequest.html",
                    tokens);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email send failed: {ex.Message}");
            }


            return _mapper.Map<ReadSalaryDisbursementDto>(disbursement);
        }

        public async Task<IEnumerable<ReadSalaryDisbursementDto>> GetSalaryDisbursementsByClientUserIdAsync(int clientUserId)
        {
            var disb = await _disbursementRepo.GetByClientUserIdAsync(clientUserId);
            return _mapper.Map<IEnumerable<ReadSalaryDisbursementDto>>(disb);
        }

        public async Task<IEnumerable<ReadSalaryDisbursementDto>> GetPendingSalaryDisbursementsByBankUserIdAsync(int bankUserId)
        {
            var disb = await _disbursementRepo.GetPendingByBankUserIdAsync(bankUserId);
            return _mapper.Map<IEnumerable<ReadSalaryDisbursementDto>>(disb);
        }

        public async Task<ReadSalaryDisbursementDto?> ApproveSalaryDisbursementAsync(int disbursementId, int bankUserId)
        {
            var disbursement = await _disbursementRepo.GetByIdAsync(disbursementId);
            if (disbursement == null || disbursement.StatusId != 0) return null;

            var client = await _clientRepo.GetClientUserByIdAsync(disbursement.ClientUserId);
            if (client == null || client.BankUserId != bankUserId)
                throw new UnauthorizedAccessException("Disbursement does not belong to this bank user.");

            var account = await _accountRepo.GetByClientIdAsync(client.UserId);
            if (account == null) throw new InvalidOperationException("Account not found.");

            int successCount = 0, failCount = 0;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var detail in disbursement.Details!)
                {
                    if (account.Balance >= detail.Amount)
                    {
                        account.Balance -= detail.Amount;
                        detail.IsSuccessful = true;
                        detail.Remark = "Paid";
                        successCount++;
                    }
                    else
                    {
                        detail.IsSuccessful = false;
                        detail.Remark = "Insufficient balance";
                        failCount++;
                    }
                    detail.ProcessedAt = DateTime.UtcNow;
                }

                await _accountRepo.UpdateAsync(account);
                disbursement.SuccessfulCount = successCount;
                disbursement.FailedCount = failCount;
                disbursement.IsPartialSuccess = (successCount > 0 && failCount > 0);
                disbursement.StatusId = (failCount == 0) ? 1 : 2;
                disbursement.ProcessedAt = DateTime.UtcNow;

                await _disbursementRepo.UpdateAsync(disbursement);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }


            try
            {
                var tokens = new Dictionary<string, string?>
                {
                    ["FullName"] = client.UserFullName,
                    ["TransactionId"] = disbursement.TransactionId.ToString(),
                    ["TotalEmployees"] = disbursement.TotalEmployees.ToString(),
                    ["TotalAmount"] = disbursement.TotalAmount.ToString("F2"),
                    ["SuccessfulCount"] = disbursement.SuccessfulCount.ToString(),
                    ["FailedCount"] = disbursement.FailedCount.ToString(),
                    ["DebitedAmount"] = (disbursement.TotalAmount - (disbursement.FailedCount * (disbursement.TotalAmount / disbursement.TotalEmployees))).ToString("F2"),
                    ["ProcessedAt"] = disbursement.ProcessedAt.ToString("dd MMM yyyy, HH:mm"),
                    ["BankRemark"] = disbursement.Remarks ?? "Processed successfully"
                };
                await _emailService.SendTemplateEmailAsync(client.UserEmail,
                    "Salary disbursement processed successfully",
                    "SalaryDisbursementApproved.html",
                    tokens);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
            }


            return _mapper.Map<ReadSalaryDisbursementDto>(disbursement);
        }

        public async Task<ReadSalaryDisbursementDto?> RejectSalaryDisbursementAsync(int disbursementId, int bankUserId)
        {
            var disb = await _disbursementRepo.GetByIdAsync(disbursementId);
            if (disb == null || disb.StatusId != 0) return null;

            var client = await _clientRepo.GetClientUserByIdAsync(disb.ClientUserId);
            if (client == null || client.BankUserId != bankUserId)
                throw new UnauthorizedAccessException("Disbursement does not belong to this bank user.");

            foreach (var detail in disb.Details!)
            {
                detail.IsSuccessful = false;
                detail.Remark = "Rejected";
                detail.ProcessedAt = DateTime.UtcNow;
            }

            disb.StatusId = 2;
            disb.ProcessedAt = DateTime.UtcNow;
            disb.Remarks = "Rejected by bank user";
            await _disbursementRepo.UpdateAsync(disb);


            try
            {
                var tokens = new Dictionary<string, string?>
                {
                    ["FullName"] = client.UserFullName,
                    ["TransactionId"] = disb.TransactionId.ToString(),
                    ["BankRemark"] = disb.Remarks ?? "Rejected by bank user"
                };
                await _emailService.SendTemplateEmailAsync(client.UserEmail,
                    "Salary disbursement request was rejected",
                    "SalaryDisbursementRejected.html",
                    tokens);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
            }



            return _mapper.Map<ReadSalaryDisbursementDto>(disb);
        }

        public async Task<ReadSalaryDisbursementDto?> GetSalaryDisbursementByIdAsync(int id)
        {
            var disb = await _disbursementRepo.GetByIdAsync(id);
            return _mapper.Map<ReadSalaryDisbursementDto>(disb);
        }
    }
}
