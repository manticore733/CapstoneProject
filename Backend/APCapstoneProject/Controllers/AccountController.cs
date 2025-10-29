using APCapstoneProject.DTO.Account;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountsController(IAccountService service)
        {
            _service = service;
        }

        [HttpGet("myaccounts/{clientUserId}")]
        public async Task<IActionResult> GetMyAccounts(int clientUserId)
        {
            var accounts = await _service.GetAccountsByClientIdAsync(clientUserId);
            return Ok(accounts);
        }

        [HttpGet("{id}/ownedby/{clientUserId}")]
        public async Task<IActionResult> GetMyAccount(int id, int clientUserId)
        {
            var account = await _service.GetAccountByIdAsync(id, clientUserId);
            if (account == null) return NotFound();
            return Ok(account);
        }


        [HttpDelete("{id}/ownedby/{clientUserId}")]
        public async Task<IActionResult> Delete(int id, int clientUserId)
        {
            var success = await _service.DeleteAccountAsync(id, clientUserId);
            if (!success) return NotFound("Account not found or you do not have permission.");
            return NoContent();
        }
    }
}
