using APCapstoneProject.DTO.Auth;
using APCapstoneProject.DTO.JWT;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APCapstoneProject.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JWTSettings _jwtSettings;

        public AuthService(IAuthRepository authRepository, IOptions<JWTSettings> jwtOptions)
        {
            _authRepository = authRepository;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _authRepository.GetUserByUsernameAsync(dto.Username);

            // ❌ User not found
            if (user == null)
            {
                return new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found."
                };
            }

            // 🔐 Verify hashed password
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid password."
                };
            }

            // ✅ Generate JWT token
            string token = GenerateToken(user);

            return new LoginResponseDto
            {
                IsSuccess = true,
                Token = token,
                Role = user.Role.Role.ToString(),
                UserId = user.UserId,
                Message = "Login successful."
            };
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("UserId", user.UserId.ToString()),
                new Claim("Username", user.UserName),
                new Claim(ClaimTypes.Role, user.Role.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
