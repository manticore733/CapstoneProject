using APCapstoneProject.DTO.Payment;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST: ClientUser initiates payment
        [HttpPost("client/{clientUserId}")]
        public async Task<ActionResult<ReadPaymentDto>> CreatePayment(int clientUserId, [FromBody] CreatePaymentDto dto)
        {
            var payment = await _paymentService.CreatePaymentAsync(clientUserId, dto);
            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.TransactionId }, payment);
        }

        // GET: All payments by ClientUser
        [HttpGet("client/{clientUserId}")]
        public async Task<ActionResult<IEnumerable<ReadPaymentDto>>> GetClientPayments(int clientUserId)
        {
            var payments = await _paymentService.GetPaymentsByClientUserIdAsync(clientUserId);
            return Ok(payments);
        }

        // GET: Pending payments for BankUser
        [HttpGet("bankuser/{bankUserId}/pending")]
        public async Task<ActionResult<IEnumerable<ReadPaymentDto>>> GetPendingPayments(int bankUserId)
        {
            var payments = await _paymentService.GetPendingPaymentsByBankUserIdAsync(bankUserId);
            return Ok(payments);
        }

        // PUT: Approve payment
        [HttpPut("{paymentId}/approve/by/{bankUserId}")]
        public async Task<ActionResult<ReadPaymentDto>> ApprovePayment(int paymentId, int bankUserId)
        {
            var result = await _paymentService.ApprovePaymentAsync(paymentId, bankUserId);
            if (result == null) return NotFound("Payment not found or not pending.");
            return Ok(result);
        }

        // PUT: Reject payment
        [HttpPut("{paymentId}/reject/by/{bankUserId}")]
        public async Task<ActionResult<ReadPaymentDto>> RejectPayment(int paymentId, int bankUserId)
        {
            var result = await _paymentService.RejectPaymentAsync(paymentId, bankUserId);
            if (result == null) return NotFound("Payment not found or not pending.");
            return Ok(result);
        }

        // GET: Payment by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadPaymentDto>> GetPaymentById(int id)
        {
            var result = await _paymentService.GetPaymentsByPaymentIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
