using APCapstoneProject.DTO.Beneficiary;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Controllers
{
    [Authorize(Roles = "CLIENT_USER")]
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiaryController : ControllerBase
    {
        private readonly IBeneficiaryService _beneficiaryService;

        public BeneficiaryController(IBeneficiaryService beneficiaryService)
        {
            _beneficiaryService = beneficiaryService;
        }

        [HttpGet("mybeneficiaries")]
        public async Task<IActionResult> GetMyBeneficiaries()
        {
            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var beneficiaries = await _beneficiaryService.GetBeneficiariesByClientIdAsync(clientUserId);
            return Ok(beneficiaries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMyBeneficiary(int id)
        {
            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var beneficiary = await _beneficiaryService.GetBeneficiaryByIdAsync(id, clientUserId);
            if (beneficiary == null) return NotFound();
            return Ok(beneficiary);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBeneficiaryDto beneficiaryDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var created = await _beneficiaryService.CreateBeneficiaryAsync(beneficiaryDto, clientUserId);

            return CreatedAtAction(nameof(GetMyBeneficiary), new { id = created.BeneficiaryId, clientUserId = clientUserId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBeneficiaryDto beneficiaryDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var updatedBeneficiary = await _beneficiaryService.UpdateBeneficiaryAsync(id, beneficiaryDto, clientUserId);

            if (updatedBeneficiary==null) return NotFound("Beneficiary not found or you do not have permission.");
            return Ok(updatedBeneficiary);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var success = await _beneficiaryService.DeleteBeneficiaryAsync(id, clientUserId);

            if (!success) return NotFound("Beneficiary not found or you do not have permission.");
            return NoContent();
        }
    }
}