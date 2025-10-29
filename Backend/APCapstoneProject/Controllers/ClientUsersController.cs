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
                // When we add auth, get 'bankUserId' from the token instead.
                var newUser = await _userService.CreateClientUserAsync(createDto, bankUserId);
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
            var updatedUser = await _userService.UpdateClientUserAsync(id, updateDto, bankUserId);
            if (updatedUser==null) return NotFound("Client not found or does not belong to this Bank User.");
            return Ok(updatedUser);
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

        // --- FOR TESTING ---
        // Ensure that a BankUser can delete only their own clients
        [HttpDelete("{id}/ownedby/{bankUserId}")]
        public async Task<IActionResult> DeleteClientUser(int id, int bankUserId)
        {
            try
            {
                var success = await _userService.DeleteClientUserAsync(id, bankUserId);
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
        //Authorize for bankuser access only
        [HttpPut("{clientUserId}/approve/ownedby/{bankUserId}")]
        public async Task<IActionResult> ApproveClientUser(int clientUserId, int bankUserId, [FromBody] ClientApprovalDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _userService.ApproveClientUserAsync(clientUserId, bankUserId, dto);
                if (result == null) return NotFound("Client not found or unauthorized.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }





    }
}
