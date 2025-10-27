
using APCapstoneProject.DTO.User;

namespace APCapstoneProject.Service
{
    public interface IUserService
    {
        //Task<IEnumerable<UserReadDto>> GetAllAsync();
        //Task<UserReadDto?> GetByIdAsync(int id);
        //Task<UserReadDto> CreateAsync(UserCreateDto userCreateDto);
        //Task<bool> UpdateAsync(int id, UserUpdateDto userUpdateDto);
        //Task<bool> SoftDeleteAsync(int id);






        // General Admin methods
        Task<IEnumerable<UserReadDto>> GetAllAsync();
        Task<UserReadDto?> GetByIdAsync(int id);
        Task<UserReadDto> CreateAsync(UserCreateDto userCreateDto); // For Super Admin -> Super Admin
        Task<bool> UpdateAsync(int id, UserUpdateDto userUpdateDto);
        Task<bool> SoftDeleteAsync(int id);

        // --- ADD THESE NEW METHODS ---

        // Bank User Specific
        Task<UserReadDto> CreateBankUserAsync(CreateBankUserDto bankUserDto);
        Task<bool> UpdateBankUserAsync(int id, UpdateBankUserDto updateDto);

        // Client User Specific
        Task<UserReadDto> CreateClientUserAsync(CreateClientUserDto clientUserDto, int creatorBankUserId);
        Task<IEnumerable<UserReadDto>> GetClientsForBankUserAsync(int bankUserId);
        Task<UserReadDto?> GetClientForBankUserAsync(int clientId, int bankUserId);
        Task<bool> UpdateClientUserAsync(int id, UpdateClientUserDto updateDto, int bankUserId);









    }
}
