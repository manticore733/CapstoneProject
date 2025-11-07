using APCapstoneProject.DTO.User;
using APCapstoneProject.DTO.User.ClientUser;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ClientUsersController : ControllerBase
    {
        private readonly IClientUserService _clientUserService;

        public ClientUsersController(IClientUserService clientUserService)
        {
            _clientUserService = clientUserService;
        }

        [HttpPost]
        [Authorize(Roles = "BANK_USER")]
        public async Task<ActionResult<UserReadDto>> CreateClientUser([FromBody] CreateClientUserDto createDto)
        {
            var bankUserId = int.Parse(User.FindFirst("UserId")!.Value);
            try
            {
                // When we add auth, get 'bankUserId' from the token instead.
                var newUser = await _clientUserService.CreateClientUserAsync(createDto, bankUserId);
                return CreatedAtAction(nameof(UserController.GetUserById), "User", new { id = newUser.UserId }, newUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "BANK_USER")]
        public async Task<IActionResult> UpdateClientUser(int id, [FromBody] UpdateClientUserDto updateDto)
        {
            var bankUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var updatedUser = await _clientUserService.UpdateClientUserAsync(id, updateDto, bankUserId);
            if (updatedUser==null) return NotFound("Client not found or does not belong to this Bank User.");
            return Ok(updatedUser);
        }


        [HttpGet("myclients")]
        [Authorize(Roles = "BANK_USER")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetMyClients()
        {
            var bankUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var clients = await _clientUserService.GetClientsForBankUserAsync(bankUserId);
            return Ok(clients);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "BANK_USER")]
        public async Task<ActionResult<UserReadDto>> GetMyClient(int id)
        {
            var bankUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var client = await _clientUserService.GetClientForBankUserAsync(id, bankUserId);
            if (client == null) return NotFound();
            return Ok(client);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "BANK_USER")]
        public async Task<IActionResult> DeleteClientUser(int id)
        {
            var bankUserId = int.Parse(User.FindFirst("UserId")!.Value);
            try
            {
                var success = await _clientUserService.DeleteClientUserAsync(id, bankUserId);
                if (!success)
                    return NotFound("Client not found or does not belong to this Bank User.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ========== CLIENT USER ENDPOINTS ==========

        // ← NEW: Client User gets their own profile
        [Authorize(Roles = "CLIENT_USER , BANK_USER")]
        [HttpGet("myprofile")]
        public async Task<ActionResult<ReadClientUserDto>> GetMyProfile()
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var clientUser = await _clientUserService.GetClientUserByIdAsync(userId);

            if (clientUser == null)
                return NotFound(new { message = "Client user profile not found." });

            return Ok(clientUser);
        }




    }
}
