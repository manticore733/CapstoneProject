using APCapstoneProject.Data;
using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly BankingAppDbContext _context;

        public PaymentRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<Payment?> GetPaymentByPaymentIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.SenderClient)
                .Include(p => p.Beneficiary)
                .Include(p => p.Account)
                .Include(p => p.TransactionStatus)
                .FirstOrDefaultAsync(p => p.TransactionId == id);
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.SenderClient)
                .Include(p => p.Beneficiary)
                .Include(p => p.TransactionStatus)
                .ToListAsync();
        }


        public async Task<IEnumerable<Payment>> GetPaymentsByClientUserIdAsync(int clientUserId)
        {
            return await _context.Payments
                .Where(p => p.SenderClientId == clientUserId)
                .Include(p => p.Beneficiary)
                .Include(p => p.TransactionStatus)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPendingPaymentsByBankUserIdAsync(int bankUserId)
        {
            return await _context.Payments
                .Include(p => p.SenderClient)
                .Include(p => p.Beneficiary)
                .Include(p => p.TransactionStatus)
                .Where(p => p.SenderClient.BankUserId == bankUserId && p.StatusId == 0)
                .ToListAsync();
        }
    }
}
