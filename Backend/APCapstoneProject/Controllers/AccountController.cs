using APCapstoneProject.DTO.Account;
using APCapstoneProject.Service;
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
    }
}
