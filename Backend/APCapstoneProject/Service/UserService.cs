using APCapstoneProject.DTO.User;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using AutoMapper;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        // --- SUPERADMIN OPERATIONS ---

        public async Task<IEnumerable<UserReadDto>> GetAllAsync()
        {
            var users = await _userRepo.GetUsersAsync();
            return _mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        public async Task<UserReadDto?> GetByIdAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserReadDto>(user);
        }

        public async Task<UserReadDto> CreateAsync(UserCreateDto userCreateDto)
        {
            if (userCreateDto.UserRoleId != (int)Role.SUPER_ADMIN)
                throw new ArgumentException("This endpoint can only be used by Super Admins.");

            var user = _mapper.Map<User>(userCreateDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreateDto.PasswordHash);
            user.IsActive = true;
            user.BankId = null;

            await _userRepo.AddAsync(user);

            var created = await _userRepo.GetByIdAsync(user.UserId);
            return _mapper.Map<UserReadDto>(created);
        }

        //should only be used by superAdmins
        public async Task<UserReadDto?> UpdateAsync(int id, UserUpdateDto userUpdateDto)
        {
            var existing = await _userRepo.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(userUpdateDto, existing);
            await _userRepo.UpdateAsync(existing);

            var updated = await _userRepo.GetByIdAsync(existing.UserId);
            return _mapper.Map<UserReadDto>(updated);
        }

        //should only be used by superAdmins
        public async Task<bool> DeleteAsync(int id)
        {
            return await _userRepo.DeleteAsync(id);
        }

    }
}
