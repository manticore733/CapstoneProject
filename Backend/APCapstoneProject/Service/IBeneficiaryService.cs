using APCapstoneProject.Model;

namespace APCapstoneProject.Service
{
    public interface IBeneficiaryService
    {
        Task<IEnumerable<Beneficiary>> GetAllBeneficiariesAsync();
        Task<IEnumerable<Beneficiary>> GetBeneficiariesByClientIdAsync(int clientUserId);
        Task<Beneficiary?> GetBeneficiaryByIdAsync(int id);
        Task<Beneficiary> CreateBeneficiaryAsync(Beneficiary beneficiary);
        Task UpdateBeneficiaryAsync(int id, Beneficiary beneficiary);
        Task DeleteBeneficiaryAsync(int id);
    }
}
