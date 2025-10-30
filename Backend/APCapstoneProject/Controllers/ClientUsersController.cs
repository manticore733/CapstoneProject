using APCapstoneProject.DTO.User;
using APCapstoneProject.DTO.User.ClientUser;
using APCapstoneProject.Service;
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

        // --- FOR TESTING ---
        // We'll pass the creator's ID in the route.
        // The real endpoint will be just [HttpPost]
        [HttpPost("createdby/{bankUserId}")]
        public async Task<ActionResult<UserReadDto>> CreateClientUser(int bankUserId, CreateClientUserDto createDto)
        {
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

        // --- FOR TESTING ---
        // We need the BankUser's ID to check if they own this client.
        [HttpPut("{id}/ownedby/{bankUserId}")]
        public async Task<IActionResult> UpdateClientUser(int id, int bankUserId, UpdateClientUserDto updateDto)
        {
            var updatedUser = await _clientUserService.UpdateClientUserAsync(id, updateDto, bankUserId);
            if (updatedUser==null) return NotFound("Client not found or does not belong to this Bank User.");
            return Ok(updatedUser);
        }

        // --- FOR TESTING ---
        // We need the BankUser's ID to get their clients.
        [HttpGet("myclients/{bankUserId}")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetMyClients(int bankUserId)
        {
            var clients = await _clientUserService.GetClientsForBankUserAsync(bankUserId);
            return Ok(clients);
        }

        // --- FOR TESTING ---
        [HttpGet("{id}/ownedby/{bankUserId}")]
        public async Task<ActionResult<UserReadDto>> GetMyClient(int id, int bankUserId)
        {
            var client = await _clientUserService.GetClientForBankUserAsync(id, bankUserId);
            if (client == null) return NotFound();
            return Ok(client);
        }

        // --- FOR TESTING ---
        // Ensure that a BankUser can delete only their own clients
        [HttpDelete("{id}/ownedby/{bankUserId}")]
        public async Task<IActionResult> DeleteClientUser(int id, int bankUserId)
        {
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

        // Additional methods for business logic

    }
}
