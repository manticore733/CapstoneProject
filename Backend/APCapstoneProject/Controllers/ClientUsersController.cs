using APCapstoneProject.DTO.User;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientUsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public ClientUsersController(IUserService userService)
        {
            _userService = userService;
        }

        // --- FOR TESTING ---
        // We'll pass the creator's ID in the route.
        // The real endpoint will be just [HttpPost]
        [HttpPost("createdby/{bankUserId}")]
        public async Task<ActionResult<UserReadDto>> CreateClientUser(int bankUserId, CreateClientUserDto createDto)
        {
            try
            {
                // When we add auth, we'll get 'bankUserId' from the token instead.
                var newUser = await _userService.CreateClientUserAsync(createDto, bankUserId);
                return CreatedAtAction(nameof(UserController.GetUser), "User", new { id = newUser.UserId }, newUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // --- FOR TESTING ---
        // We need the BankUser's ID to check if they own this client.
        [HttpPut("{id}/ownedby/{bankUserId}")]
        public async Task<IActionResult> UpdateClientUser(int id, int bankUserId, UpdateClientUserDto updateDto)
        {
            if (id != updateDto.UserId) return BadRequest("Mismatched ID");

            var success = await _userService.UpdateClientUserAsync(id, updateDto, bankUserId);
            if (!success) return NotFound("Client not found or does not belong to this Bank User.");
            return NoContent();
        }

        // --- FOR TESTING ---
        // We need the BankUser's ID to get their clients.
        [HttpGet("myclients/{bankUserId}")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetMyClients(int bankUserId)
        {
            var clients = await _userService.GetClientsForBankUserAsync(bankUserId);
            return Ok(clients);
        }

        // --- FOR TESTING ---
        [HttpGet("{id}/ownedby/{bankUserId}")]
        public async Task<ActionResult<UserReadDto>> GetMyClient(int id, int bankUserId)
        {
            var client = await _userService.GetClientForBankUserAsync(id, bankUserId);
            if (client == null) return NotFound();
            return Ok(client);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientUser(int id)
        {
            // The generic soft delete is fine for any user
            var success = await _userService.SoftDeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
