using APCapstoneProject.Model;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiaryController : ControllerBase
    {
        private readonly IBeneficiaryService _beneficiaryService;

        public BeneficiaryController(IBeneficiaryService beneficiaryService)
        {
            _beneficiaryService = beneficiaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var beneficiaries = await _beneficiaryService.GetAllBeneficiariesAsync();
            return Ok(beneficiaries);
        }

        [HttpGet("client/{clientUserId}")]
        public async Task<IActionResult> GetByClientId(int clientUserId)
        {
            var beneficiaries = await _beneficiaryService.GetBeneficiariesByClientIdAsync(clientUserId);
            return Ok(beneficiaries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var beneficiary = await _beneficiaryService.GetBeneficiaryByIdAsync(id);
            if (beneficiary == null) return NotFound();
            return Ok(beneficiary);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Beneficiary beneficiary)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _beneficiaryService.CreateBeneficiaryAsync(beneficiary);
            return CreatedAtAction(nameof(GetById), new { id = created.BeneficiaryId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Beneficiary beneficiary)
        {
            await _beneficiaryService.UpdateBeneficiaryAsync(id, beneficiary);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _beneficiaryService.DeleteBeneficiaryAsync(id);
            return NoContent();
        }
    }
}
