using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IBankRepository
    {

        Task<IEnumerable<Bank>> GetAllAsync();
        Task<Bank?> GetByIdAsync(int id);
        Task<Bank> AddAsync(Bank bank);
        Task UpdateAsync(Bank bank);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);


    }
}
