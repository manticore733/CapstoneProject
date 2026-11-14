using APCapstoneProject.Data;
using APCapstoneProject.DTO.Payment;
using APCapstoneProject.DTO.SalaryDisbursement;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IClientUserRepository _clientUserRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IMapper _mapper;
        // included only to start a transaction in the EF so we can rollback if either of these 2 actions fail:
        // 1. Status change
        // 2. Deduct amount
        private readonly BankingAppDbContext _context;
        private readonly IEmailService _emailService;

        public PaymentService(IPaymentRepository paymentRepo, IClientUserRepository clientUserRepo, IAccountRepository accountRepo, IMapper mapper, BankingAppDbContext context, IEmailService emailService)
        {
            _paymentRepo = paymentRepo;
            _clientUserRepo = clientUserRepo;
            _accountRepo = accountRepo;
            _mapper = mapper;
            _context = context;
            _emailService = emailService;
        }

        public async Task<ReadPaymentDto> CreatePaymentAsync(int clientUserId, CreatePaymentDto dto)
        {
            // Ensure client exists and is approved
            var client = await _clientUserRepo.GetClientUserByIdAsync(clientUserId);
            if (client == null || client.StatusId != 1)
                throw new InvalidOperationException("Client not approved or not found.");

            // Ensure beneficiary belongs to this client
            var beneficiary = client.Beneficiaries?.FirstOrDefault(b => b.BeneficiaryId == dto.BeneficiaryId && b.IsActive);
            if (beneficiary == null)
                throw new UnauthorizedAccessException("Beneficiary does not belong to this client.");

            // Create payment
            var payment = new Payment
            {
                Amount = dto.Amount,
                TransactionTypeId = 1, // DEBIT
                StatusId = 0, // PENDING
                SenderClientId = clientUserId,
                BeneficiaryId = dto.BeneficiaryId,
                AccountId = client.Account.AccountId,
                BankName = beneficiary.BankName,
                IFSC = beneficiary.IFSC,
                DestinationAccountNumber = beneficiary.AccountNumber,
                Remarks = dto.Remarks,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepo.AddPaymentAsync(payment);


            var paymentFromDb = await _paymentRepo.GetPaymentByPaymentIdAsync(payment.TransactionId);



            try
            {
                var tokens = new Dictionary<string, string?>
                {
                    ["FullName"] = client.UserFullName,
                    ["TransactionId"] = paymentFromDb.TransactionId.ToString(),
                    ["Amount"] = paymentFromDb.Amount.ToString("F2"),
                    ["BeneficiaryName"] = beneficiary.BeneficiaryName,
                    ["ClientRemarks"] = paymentFromDb.Remarks ?? "-",
                    ["CreatedAt"] = DateTime.UtcNow.ToString("dd MMM yyyy, HH:mm")
                };
                await _emailService.SendTemplateEmailAsync(client.UserEmail,
                    $"Payment request received for ₹{paymentFromDb.Amount:F2}",
                    "PaymentRequestReceived.html",
                    tokens);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email send failed: {ex.Message}");
            }




            return _mapper.Map<ReadPaymentDto>(paymentFromDb);
        }

        public async Task<ReadPaymentDto?> GetPaymentsByPaymentIdAsync(int paymentId)
        {
            var payments = await _paymentRepo.GetPaymentByPaymentIdAsync(paymentId);
            return _mapper.Map<ReadPaymentDto>(payments);
        }

        public async Task<IEnumerable<ReadPaymentDto>> GetPaymentsByClientUserIdAsync(int clientUserId)
        {
            var payments = await _paymentRepo.GetPaymentsByClientUserIdAsync(clientUserId);
            return _mapper.Map<IEnumerable<ReadPaymentDto>>(payments);
        }

        public async Task<IEnumerable<ReadPaymentDto>> GetPendingPaymentsByBankUserIdAsync(int bankUserId)
        {
            var payments = await _paymentRepo.GetPendingPaymentsByBankUserIdAsync(bankUserId);
            return _mapper.Map<IEnumerable<ReadPaymentDto>>(payments);
        }

        public async Task<ReadPaymentDto?> ApprovePaymentAsync(int paymentId, int bankUserId)
        {
            var payment = await _paymentRepo.GetPaymentByPaymentIdAsync(paymentId);
            if (payment == null || payment.StatusId != 0)
                return null;

            var client = await _clientUserRepo.GetClientUserByIdAsync(payment.SenderClientId);
            if (client == null || client.BankUserId != bankUserId)
                throw new UnauthorizedAccessException("Payment does not belong to this bank user.");

            var account = client.Account!;
            if (account.Balance < payment.Amount)
                throw new InvalidOperationException("Insufficient balance.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Deduct amount
                account.Balance -= payment.Amount;
                await _accountRepo.UpdateAsync(account);

                // Update payment status
                payment.StatusId = 1; // APPROVED
                payment.ApprovedAt = DateTime.UtcNow;
                payment.ProcessedAt = DateTime.UtcNow;
                await _paymentRepo.UpdatePaymentAsync(payment);

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
                    ["TransactionId"] = payment.TransactionId.ToString(),
                    ["BeneficiaryName"] = payment.Beneficiary?.BeneficiaryName,
                    ["Amount"] = payment.Amount.ToString("F2"),
                    ["ProcessedAt"] = DateTime.UtcNow.ToString("dd MMM yyyy, HH:mm"),
                    ["BankRemark"] = payment.BankRemark ?? "Processed successfully"
                };
                await _emailService.SendTemplateEmailAsync(client.UserEmail,
                    $"Payment of ₹{payment.Amount:F2} has been approved",
                    "PaymentApproved.html",
                    tokens);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
            }


            return _mapper.Map<ReadPaymentDto>(payment);
        }

        public async Task<ReadPaymentDto?> RejectPaymentAsync(int paymentId, int bankUserId)
        {
            var payment = await _paymentRepo.GetPaymentByPaymentIdAsync(paymentId);
            if (payment == null || payment.StatusId != 0)
                return null;

            var client = await _clientUserRepo.GetClientUserByIdAsync(payment.SenderClientId);
            if (client == null || client.BankUserId != bankUserId)
                throw new UnauthorizedAccessException("Payment does not belong to this bank user.");

            payment.StatusId = 2; // REJECTED
            payment.ApprovedAt = DateTime.UtcNow;

            await _paymentRepo.UpdatePaymentAsync(payment);


            try
            {
                var tokens = new Dictionary<string, string?>
                {
                    ["FullName"] = client.UserFullName,
                    ["TransactionId"] = payment.TransactionId.ToString(),
                    ["BankRemark"] = payment.BankRemark ?? "Rejected by bank"
                };
                await _emailService.SendTemplateEmailAsync(client.UserEmail,
                    "Payment request was rejected",
                    "PaymentRejected.html",
                    tokens);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
            }



            var paymentFromDb = await _paymentRepo.GetPaymentByPaymentIdAsync(payment.TransactionId);
            return _mapper.Map<ReadPaymentDto>(paymentFromDb);
        }
    }
}
