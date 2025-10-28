using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetUsersAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);

        //Task<bool> SaveChangesAsync();

        Task<IEnumerable<User>> GetClientsByBankUserIdAsync(int bankUserId);
        Task<ClientUser?> GetClientByBankUserIdAsync(int clientId, int bankUserId);





    }
}
