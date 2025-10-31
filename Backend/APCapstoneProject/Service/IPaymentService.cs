using APCapstoneProject.DTO.Payment;

namespace APCapstoneProject.Service
{
    public interface IPaymentService
    {
        Task<ReadPaymentDto> CreatePaymentAsync(int clientUserId, CreatePaymentDto dto);
        Task<ReadPaymentDto?> GetPaymentsByPaymentIdAsync(int paymentId);
        Task<IEnumerable<ReadPaymentDto>> GetPaymentsByClientUserIdAsync(int clientUserId);
        Task<IEnumerable<ReadPaymentDto>> GetPendingPaymentsByBankUserIdAsync(int bankUserId);
        Task<ReadPaymentDto?> ApprovePaymentAsync(int paymentId, int bankUserId);
        Task<ReadPaymentDto?> RejectPaymentAsync(int paymentId, int bankUserId);
    }
}
