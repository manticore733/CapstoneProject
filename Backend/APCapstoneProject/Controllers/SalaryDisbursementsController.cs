using APCapstoneProject.DTO.SalaryDisbursement;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryDisbursementsController : ControllerBase
    {
        private readonly ISalaryDisbursementService _disbursementService;

        public SalaryDisbursementsController(ISalaryDisbursementService disbursementService)
        {
            _disbursementService = disbursementService;
        }

        // 🔹 POST: ClientUser initiates a salary disbursement to one employee
        [HttpPost("client/{clientUserId}")]
        public async Task<ActionResult<ReadSalaryDisbursementDto>> CreateDisbursement(int clientUserId, [FromBody] CreateSalaryDisbursementDto dto)
        {
            var result = await _disbursementService.CreateSalaryDisbursementAsync(clientUserId, dto);
            return CreatedAtAction(nameof(GetById), new { id = result.TransactionId }, result);
        }

        // 🔹 GET: All disbursements for a specific client user
        [HttpGet("client/{clientUserId}")]
        public async Task<ActionResult<IEnumerable<ReadSalaryDisbursementDto>>> GetByClientUserId(int clientUserId)
        {
            var disbursements = await _disbursementService.GetSalaryDisbursementsByClientUserIdAsync(clientUserId);
            return Ok(disbursements);
        }

        // 🔹 GET: Pending disbursements for a specific bank user
        [HttpGet("bankuser/{bankUserId}/pending")]
        public async Task<ActionResult<IEnumerable<ReadSalaryDisbursementDto>>> GetPendingByBankUser(int bankUserId)
        {
            var disbursements = await _disbursementService.GetPendingSalaryDisbursementsByBankUserIdAsync(bankUserId);
            return Ok(disbursements);
        }

        // 🔹 PUT: Approve a disbursement by bank user
        [HttpPut("{disbursementId}/approve/by/{bankUserId}")]
        public async Task<ActionResult<ReadSalaryDisbursementDto>> Approve(int disbursementId, int bankUserId)
        {
            var result = await _disbursementService.ApproveSalaryDisbursementAsync(disbursementId, bankUserId);
            if (result == null)
                return NotFound("Salary disbursement not found or not pending.");

            return Ok(result);
        }

        // 🔹 PUT: Reject a disbursement by bank user
        [HttpPut("{disbursementId}/reject/by/{bankUserId}")]
        public async Task<ActionResult<ReadSalaryDisbursementDto>> Reject(int disbursementId, int bankUserId)
        {
            var result = await _disbursementService.RejectSalaryDisbursementAsync(disbursementId, bankUserId);
            if (result == null)
                return NotFound("Salary disbursement not found or not pending.");

            return Ok(result);
        }

        // 🔹 GET: Specific disbursement by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadSalaryDisbursementDto>> GetById(int id)
        {
            // Reuse service method for fetching one
            var all = await _disbursementService.GetSalaryDisbursementsByClientUserIdAsync(0); // adjust if needed
            var found = all.FirstOrDefault(d => d.TransactionId == id);

            if (found == null)
                return NotFound("Salary disbursement not found.");

            return Ok(found);
        }
    }
}
