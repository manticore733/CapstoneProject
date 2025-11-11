using APCapstoneProject.DTO.SalaryDisbursement;

namespace APCapstoneProject.Service
{
    public interface ISalaryDisbursementService
    {
        Task<ReadSalaryDisbursementDto> CreateSalaryDisbursementAsync(int clientUserId, CreateSalaryDisbursementDto dto);
        Task<ReadSalaryDisbursementDto> GetSalaryDisbursementByIdAsync(int disbursementId);
        Task<IEnumerable<ReadSalaryDisbursementDto>> GetSalaryDisbursementsByClientUserIdAsync(int clientUserId);
        Task<IEnumerable<ReadSalaryDisbursementDto>> GetPendingSalaryDisbursementsByBankUserIdAsync(int bankUserId);
        Task<ReadSalaryDisbursementDto?> ApproveSalaryDisbursementAsync(int disbursementId, int bankUserId);
        Task<ReadSalaryDisbursementDto?> RejectSalaryDisbursementAsync(int disbursementId, int bankUserId);
    }
}

