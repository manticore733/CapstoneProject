using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IPaymentRepository
    {
        Task AddPaymentAsync(Payment payment);
        Task UpdatePaymentAsync(Payment payment);
        Task<Payment?> GetPaymentByPaymentIdAsync(int id);
        Task<IEnumerable<Payment>> GetPaymentsByClientUserIdAsync(int clientUserId);
        Task<IEnumerable<Payment>> GetPendingPaymentsByBankUserIdAsync(int bankUserId);
    }
}
