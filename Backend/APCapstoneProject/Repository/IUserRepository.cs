using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> SoftDeleteAsync(int id);
        //Task<bool> SaveChangesAsync();



        Task<IEnumerable<User>> GetClientsByBankUserIdAsync(int bankUserId);
        Task<ClientUser?> GetClientByBankUserIdAsync(int clientId, int bankUserId);



    }
}
