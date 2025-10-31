﻿using APCapstoneProject.DTO.User;
using APCapstoneProject.DTO.User.BankUser;
using APCapstoneProject.DTO.User.ClientUser;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankUsersController : ControllerBase
    {
        private readonly IBankUserService _bankUserService;

        public BankUsersController(IBankUserService bankUserService)
        {
            _bankUserService = bankUserService;
        }

        [HttpPost]
        public async Task<ActionResult<UserReadDto>> CreateBankUser(CreateBankUserDto createDto)
        {
            try
            {
                var newUser = await _bankUserService.CreateBankUserAsync(createDto);
                // Redirects to the 'GetUser' action on 'UserController'
                return CreatedAtAction(nameof(UserController.GetAllUsers), "User", new { id = newUser.UserId }, newUser);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // "Bank not found"
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBankUser(int id, UpdateBankUserDto updateDto)
        {
            var updatedUser = await _bankUserService.UpdateBankUserAsync(id, updateDto);
            if (updatedUser == null) return NotFound();
            return Ok(updatedUser);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAllBankUsers()
        {
            // This gets all users and filters for Bank Users in the controller.
            var users = await _bankUserService.GetAllBankUsersAsync();
            var bankUsers = users.Where(u => u.RoleName == "BANK_USER");
            return Ok(bankUsers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadDto>> GetBankUser(int id)
        {
            var user = await _bankUserService.GetBankUserByIdAsync(id);
            if (user == null || user.RoleName != "BANK_USER") return NotFound();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankUser(int id)
        {
            // Uses the generic soft delete from the service
            var success = await _bankUserService.DeleteBankUserAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }




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
        // PUT /api/clientusers/{clientId}/approvedby/{bankUserId}
        // [Authorize(Roles = "BANK_USER")] // <-- Add later
        [HttpPut("{clientId}/approveby/{bankUserId}")]
        public async Task<ActionResult<ReadClientUserDto>> ApproveClient(int clientId, int bankUserId, [FromBody] ClientApprovalDto approvalDto)
        {
            // LATER: bankUserId will come from JWT token
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _bankUserService.ApproveClientUserAsync(clientId, bankUserId, approvalDto);
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
