using APCapstoneProject.DTO.Auth;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.LoginAsync(dto);

            if (!response.IsSuccess)
                return BadRequest(new { message = response.Message });

            return Ok(new
            {
                token = response.Token,
                userId = response.UserId,
                role = response.Role,
                message = response.Message
            });
        }


        // Example of testing authorization
        [HttpGet("secure-test")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult SecureTest()
        {
            var username = User.FindFirst("Username")?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok($"Welcome {username}, your role is {role} and JWT authentication works!");
        }
    }
}
