using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IBeneficiaryRepository
    {
        Task<IEnumerable<Beneficiary>> GetByClientIdAsync(int clientUserId);
        Task<Beneficiary?> GetByIdAndClientIdAsync(int id, int clientUserId);
        Task<Beneficiary> AddAsync(Beneficiary beneficiary);
        Task UpdateAsync(Beneficiary beneficiary);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
