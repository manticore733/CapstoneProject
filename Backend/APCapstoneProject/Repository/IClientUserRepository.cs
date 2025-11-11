using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IClientUserRepository
    {
        //basic CRUD:
        Task<IEnumerable<ClientUser>> GetClientUsersAsync();
        Task<ClientUser?> GetClientUserByIdAsync(int id);
        Task AddClientUserAsync(ClientUser user);
        Task UpdateClientUserAsync(ClientUser user);
        Task<bool> DeleteClientUserAsync(int id);


        //business logic
        Task<IEnumerable<ClientUser>> GetClientsByBankUserIdAsync(int bankUserId);
        Task<ClientUser?> GetClientByBankUserIdAsync(int clientId, int bankUserId);
        Task<bool> IsActiveClientUserAsync(int userId);
    }
}
