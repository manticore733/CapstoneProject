
using APCapstoneProject.DTO.User;

namespace APCapstoneProject.Service
{
    public interface IUserService
    {
        Task<IEnumerable<UserReadDto>> GetAllAsync();
        Task<UserReadDto?> GetByIdAsync(int id);
        Task<UserReadDto> CreateAsync(UserCreateDto userCreateDto);
        Task<UserReadDto?> UpdateAsync(int id, UserUpdateDto userUpdateDto);
        Task<bool> DeleteAsync(int id);
    }
}
