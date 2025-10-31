using APCapstoneProject.DTO.Beneficiary;

namespace APCapstoneProject.Service
{
    public interface IBeneficiaryService
    {
        Task<IEnumerable<BeneficiaryReadDto>> GetBeneficiariesByClientIdAsync(int clientUserId);
        Task<BeneficiaryReadDto?> GetBeneficiaryByIdAsync(int id, int clientUserId);
        Task<BeneficiaryReadDto> CreateBeneficiaryAsync(CreateBeneficiaryDto beneficiaryDto, int clientUserId);
        Task<BeneficiaryReadDto> UpdateBeneficiaryAsync(int id, UpdateBeneficiaryDto beneficiaryDto, int clientUserId);
        Task<bool> DeleteBeneficiaryAsync(int id, int clientUserId);
    }
}