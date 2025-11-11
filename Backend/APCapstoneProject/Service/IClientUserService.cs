using APCapstoneProject.DTO.User.ClientUser;

namespace APCapstoneProject.Service
{
    public interface IClientUserService
    {
        Task<ReadClientUserDto> CreateClientUserAsync(CreateClientUserDto dto, int creatorBankUserId);
        Task<IEnumerable<ReadClientUserDto>> GetClientsForBankUserAsync(int bankUserId);
        Task<ReadClientUserDto?> GetClientForBankUserAsync(int clientId, int bankUserId);
        Task<ReadClientUserDto?> UpdateClientUserAsync(int clientId, UpdateClientUserDto dto, int bankUserId);
        Task<bool> DeleteClientUserAsync(int clientId, int bankUserId);


        // ← NEW: Get client user profile by their own user ID
        Task<ReadClientUserDto?> GetClientUserByIdAsync(int userId);
    }
}
