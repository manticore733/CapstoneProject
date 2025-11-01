using APCapstoneProject.DTO.Auth;

namespace APCapstoneProject.Service
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    }
}
