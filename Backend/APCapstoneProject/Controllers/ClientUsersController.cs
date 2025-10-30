using APCapstoneProject.DTO.User;
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
        //[HttpPut("{clientUserId}/approve/ownedby/{bankUserId}")]
        //public async Task<IActionResult> ApproveClientUser(int clientUserId, int bankUserId, [FromBody] ClientApprovalDto dto)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);

        //    try
        //    {
        //        var result = await _userService.ApproveClientUserAsync(clientUserId, bankUserId, dto);
        //        if (result == null) return NotFound("Client not found or unauthorized.");
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}




        // bu approves cu based on valid doc
        // --- ADD THIS NEW ENDPOINT ---
        // PUT /api/clientusers/{clientId}/approveby/{bankUserId}
        // [Authorize(Roles = "BANK_USER")] // <-- Add later
        [HttpPut("{clientId}/approveby/{bankUserId}")]
        public async Task<ActionResult<ClientStatusReadDto>> ApproveClient(int clientId, int bankUserId, [FromBody] ClientApprovalDto approvalDto)
        {
            // LATER: bankUserId will come from JWT token
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userService.ApproveClientUserAsync(clientId, bankUserId, approvalDto);
                if (result == null)
                {
                    // This might occur if the re-fetch fails after save
                    return NotFound("Client processed, but failed to retrieve updated status.");
                }
                // Return 200 OK with the updated client status (including account number if created)
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                // Client or Bank User not found
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Approving user is not a valid Bank User
                // Return 401 Unauthorized or 403 Forbidden depending on auth setup
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Client already processed or already has an account
                return Conflict(new { message = ex.Message }); // 409 Conflict
            }
            catch (DbUpdateException ex) // Catch potential save errors
            {
                // Log the inner exception ex.InnerException for details
                return StatusCode(500, "Database error occurred during the approval process.");
            }
            catch (Exception ex)
            {
                // Log the exception ex for details
                return StatusCode(500, "An unexpected error occurred during client approval.");
            }
        }











    }
}
