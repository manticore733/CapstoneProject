using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IBeneficiaryRepository
    {
        Task<IEnumerable<Beneficiary>> GetAllAsync();
        Task<IEnumerable<Beneficiary>> GetByClientIdAsync(int clientUserId);
        Task<Beneficiary?> GetByIdAsync(int id);
        Task<Beneficiary> AddAsync(Beneficiary beneficiary);
        Task UpdateAsync(Beneficiary beneficiary);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
