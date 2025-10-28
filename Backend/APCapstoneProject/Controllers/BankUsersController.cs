using APCapstoneProject.DTO.User;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankUsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public BankUsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<UserReadDto>> CreateBankUser(CreateBankUserDto createDto)
        {
            try
            {
                var newUser = await _userService.CreateBankUserAsync(createDto);
                // Redirects to the 'GetUser' action on 'UserController'
                return CreatedAtAction(nameof(UserController.GetUser), "User", new { id = newUser.UserId }, newUser);
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
            var success = await _userService.UpdateBankUserAsync(id, updateDto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAllBankUsers()
        {
            // This gets all users and filters for Bank Users in the controller.
            var users = await _userService.GetAllAsync();
            var bankUsers = users.Where(u => u.RoleName == "BANK_USER");
            return Ok(bankUsers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadDto>> GetBankUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null || user.RoleName != "BANK_USER") return NotFound();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankUser(int id)
        {
            // Uses the generic soft delete from the service
            var success = await _userService.SoftDeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
