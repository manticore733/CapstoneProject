
using APCapstoneProject.DTO.User;

namespace APCapstoneProject.Service
{
    public interface IUserService
    {
        // --- COMMON USER OPERATIONS ---

        Task<IEnumerable<UserReadDto>> GetAllAsync();
        Task<UserReadDto?> GetByIdAsync(int id);
        Task<UserReadDto> CreateAsync(UserCreateDto userCreateDto);
        Task<UserReadDto?> UpdateAsync(int id, UserUpdateDto userUpdateDto);
        Task<bool> SoftDeleteAsync(int id);

        // --- BANK USER MANAGEMENT ---

        Task<UserReadDto> CreateBankUserAsync(CreateBankUserDto dto);
        Task<UserReadDto?> UpdateBankUserAsync(int id, UpdateBankUserDto dto);

        // --- CLIENT USER MANAGEMENT ---

        Task<UserReadDto> CreateClientUserAsync(CreateClientUserDto dto, int creatorBankUserId);
        Task<IEnumerable<UserReadDto>> GetClientsForBankUserAsync(int bankUserId);
        Task<UserReadDto?> GetClientForBankUserAsync(int clientId, int bankUserId);
        Task<UserReadDto?> UpdateClientUserAsync(int clientId, UpdateClientUserDto dto, int bankUserId);
        Task<bool> DeleteClientUserAsync(int clientId, int bankUserId);
    }
}
