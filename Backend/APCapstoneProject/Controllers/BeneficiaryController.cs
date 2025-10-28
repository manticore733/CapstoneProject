//using APCapstoneProject.Model;
//using APCapstoneProject.Service;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace APCapstoneProject.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class BeneficiaryController : ControllerBase
//    {
//        private readonly IBeneficiaryService _beneficiaryService;

//        public BeneficiaryController(IBeneficiaryService beneficiaryService)
//        {
//            _beneficiaryService = beneficiaryService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var beneficiaries = await _beneficiaryService.GetAllBeneficiariesAsync();
//            return Ok(beneficiaries);
//        }

//        [HttpGet("client/{clientUserId}")]
//        public async Task<IActionResult> GetByClientId(int clientUserId)
//        {
//            var beneficiaries = await _beneficiaryService.GetBeneficiariesByClientIdAsync(clientUserId);
//            return Ok(beneficiaries);
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(int id)
//        {
//            var beneficiary = await _beneficiaryService.GetBeneficiaryByIdAsync(id);
//            if (beneficiary == null) return NotFound();
//            return Ok(beneficiary);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create([FromBody] Beneficiary beneficiary)
//        {
//            if (!ModelState.IsValid) return BadRequest(ModelState);
//            var created = await _beneficiaryService.CreateBeneficiaryAsync(beneficiary);
//            return CreatedAtAction(nameof(GetById), new { id = created.BeneficiaryId }, created);
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(int id, [FromBody] Beneficiary beneficiary)
//        {
//            await _beneficiaryService.UpdateBeneficiaryAsync(id, beneficiary);
//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            await _beneficiaryService.DeleteBeneficiaryAsync(id);
//            return NoContent();
//        }
//    }
//}
















using APCapstoneProject.DTO.Beneficiary;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Controllers
{
    [Route("api/beneficiaries")] // Changed route for simplicity
    [ApiController]
    public class BeneficiaryController : ControllerBase
    {
        private readonly IBeneficiaryService _beneficiaryService;

        public BeneficiaryController(IBeneficiaryService beneficiaryService)
        {
            _beneficiaryService = beneficiaryService;
        }

        // [Authorize(Roles = "CLIENT_USER")] // <-- Add this later
        [HttpGet("mybeneficiaries/{clientUserId}")]
        public async Task<IActionResult> GetMyBeneficiaries(int clientUserId)
        {
            // LATER: clientUserId will come from the JWT token
            var beneficiaries = await _beneficiaryService.GetBeneficiariesByClientIdAsync(clientUserId);
            return Ok(beneficiaries);
        }

        // [Authorize(Roles = "CLIENT_USER")] // <-- Add this later
        [HttpGet("{id}/ownedby/{clientUserId}")]
        public async Task<IActionResult> GetMyBeneficiary(int id, int clientUserId)
        {
            // LATER: clientUserId will come from the JWT token
            var beneficiary = await _beneficiaryService.GetBeneficiaryByIdAsync(id, clientUserId);
            if (beneficiary == null) return NotFound();
            return Ok(beneficiary);
        }

        // [Authorize(Roles = "CLIENT_USER")] // <-- Add this later
        [HttpPost("createdby/{clientUserId}")]
        public async Task<IActionResult> Create(int clientUserId, [FromBody] CreateBeneficiaryDto beneficiaryDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // LATER: clientUserId will come from the JWT token
            var created = await _beneficiaryService.CreateBeneficiaryAsync(beneficiaryDto, clientUserId);

            return CreatedAtAction(nameof(GetMyBeneficiary), new { id = created.BeneficiaryId, clientUserId = clientUserId }, created);
        }

        // [Authorize(Roles = "CLIENT_USER")] // <-- Add this later
        [HttpPut("{id}/ownedby/{clientUserId}")]
        public async Task<IActionResult> Update(int id, int clientUserId, [FromBody] UpdateBeneficiaryDto beneficiaryDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // LATER: clientUserId will come from the JWT token
            var success = await _beneficiaryService.UpdateBeneficiaryAsync(id, beneficiaryDto, clientUserId);

            if (!success) return NotFound("Beneficiary not found or you do not have permission.");
            return NoContent();
        }

        // [Authorize(Roles = "CLIENT_USER")] // <-- Add this later
        [HttpDelete("{id}/ownedby/{clientUserId}")]
        public async Task<IActionResult> Delete(int id, int clientUserId)
        {
            // LATER: clientUserId will come from the JWT token
            var success = await _beneficiaryService.DeleteBeneficiaryAsync(id, clientUserId);

            if (!success) return NotFound("Beneficiary not found or you do not have permission.");
            return NoContent();
        }
    }
}