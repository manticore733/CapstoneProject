using APCapstoneProject.DTO.SalaryDisbursement;
using APCapstoneProject.Model;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Authorization;
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




        [Authorize(Roles = "CLIENT_USER")]
        [HttpPost]
        public async Task<ActionResult<ReadSalaryDisbursementDto>> CreateDisbursement([FromForm] CreateSalaryDisbursementDto dto)
        {
            try
            {
                var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
                var result = await _disbursementService.CreateSalaryDisbursementAsync(clientUserId, dto);
                return CreatedAtAction(nameof(GetById), new { id = result.TransactionId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }









        //  GET: All disbursements for a specific client user
        [Authorize(Roles = "CLIENT_USER")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadSalaryDisbursementDto>>> GetByClientUserId()
        {
            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var disbursements = await _disbursementService.GetSalaryDisbursementsByClientUserIdAsync(clientUserId);
            return Ok(disbursements);
        }

        //  GET: Pending disbursements for a specific bank user
        [Authorize(Roles = "BANK_USER")]
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<ReadSalaryDisbursementDto>>> GetPendingByBankUser()
        {
            var bankUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var disbursements = await _disbursementService.GetPendingSalaryDisbursementsByBankUserIdAsync(bankUserId);
            return Ok(disbursements);
        }

        //  PUT: Approve a disbursement by bank user
        [Authorize(Roles = "BANK_USER")]
        [HttpPut("{disbursementId}/approve")]
        public async Task<ActionResult<ReadSalaryDisbursementDto>> Approve(int disbursementId)
        {
            var bankUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var result = await _disbursementService.ApproveSalaryDisbursementAsync(disbursementId, bankUserId);
            if (result == null)
                return NotFound("Salary disbursement not found or not pending.");

            return Ok(result);
        }

        // PUT: Reject a disbursement by bank user
        [Authorize(Roles = "BANK_USER")]
        [HttpPut("{disbursementId}/reject")]
        public async Task<ActionResult<ReadSalaryDisbursementDto>> Reject(int disbursementId)

        {
            
            var bankUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var result = await _disbursementService.RejectSalaryDisbursementAsync(disbursementId, bankUserId);
            if (result == null)
                return NotFound("Salary disbursement not found or not pending.");

            return Ok(result);
        }

        //  GET: Specific disbursement by ID
        [Authorize(Roles = "SUPER_ADMIN")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadSalaryDisbursementDto>> GetById(int id)
        {
            var salaryDisbursement = await _disbursementService.GetSalaryDisbursementByIdAsync(id);

            if (salaryDisbursement == null)
                return NotFound("Salary disbursement not found.");

            return Ok(salaryDisbursement);
        }
    }
}
