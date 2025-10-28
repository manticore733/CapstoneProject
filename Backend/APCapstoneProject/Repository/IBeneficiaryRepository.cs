//using APCapstoneProject.Model;

//namespace APCapstoneProject.Repository
//{
//    public interface IBeneficiaryRepository
//    {
//        Task<IEnumerable<Beneficiary>> GetAllAsync();
//        Task<IEnumerable<Beneficiary>> GetByClientIdAsync(int clientUserId);
//        Task<Beneficiary?> GetByIdAsync(int id);
//        Task<Beneficiary> AddAsync(Beneficiary beneficiary);
//        Task UpdateAsync(Beneficiary beneficiary);
//        Task DeleteAsync(int id);
//        Task<bool> ExistsAsync(int id);
//    }
//}








using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IBeneficiaryRepository
    {
        // Changed: Gets only beneficiaries for a specific client
        Task<IEnumerable<Beneficiary>> GetByClientIdAsync(int clientUserId);

        // New: Gets a single beneficiary *only* if it belongs to the client
        Task<Beneficiary?> GetByIdAndClientIdAsync(int id, int clientUserId);

        // No changes needed, but for consistency:
        Task<Beneficiary?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Changed: Consistent with IUserRepository
        Task AddAsync(Beneficiary beneficiary);
        void Update(Beneficiary beneficiary);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}