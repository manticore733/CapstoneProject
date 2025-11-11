using APCapstoneProject.DTO.Account;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadAccountDto>>> GetAllAccounts()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReadAccountDto>> GetAccount(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null) return NotFound();
            return Ok(account);
        }

        [HttpPut("{id}/credit")]
        public async Task<IActionResult> CreditAccount(int id, [FromBody] TransactionAmountDto dto)
        {
            if (dto.Amount <= 0) return BadRequest("Invalid amount.");
            var success = await _accountService.CreditAsync(id, dto.Amount);
            if (!success) return NotFound();
            return Ok(new { message = "Account credited successfully." });
        }

        [HttpPut("{id}/debit")]
        public async Task<IActionResult> DebitAccount(int id, [FromBody] TransactionAmountDto dto)
        {
            if (dto.Amount <= 0) return BadRequest("Invalid amount.");
            var success = await _accountService.DebitAsync(id, dto.Amount);
            if (!success) return BadRequest("Insufficient balance or invalid account.");
            return Ok(new { message = "Account debited successfully." });
        }


        // --- ADD THIS ENTIRE METHOD ---
        [Authorize(Roles = "CLIENT_USER")]
        [HttpGet("myaccount")]
        public async Task<ActionResult<ReadAccountDto>> GetMyAccount()
        {
            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var account = await _accountService.GetAccountByClientUserIdAsync(clientUserId);

            if (account == null)
            {
                // This is not an error. It just means the BU hasn't approved them yet.
                return NotFound(new { message = "Account not yet created or approved." });
            }

            return Ok(account);
        }
    }
}
