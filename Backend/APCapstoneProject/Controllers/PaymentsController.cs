using APCapstoneProject.DTO.Payment;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "CLIENT_USER")]
        [HttpPost]
        public async Task<ActionResult<ReadPaymentDto>> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var payment = await _paymentService.CreatePaymentAsync(clientUserId, dto);
            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.TransactionId }, payment);
        }

        [Authorize(Roles = "CLIENT_USER")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadPaymentDto>>> GetClientPayments()
        {
            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var payments = await _paymentService.GetPaymentsByClientUserIdAsync(clientUserId);
            return Ok(payments);
        }

        // Pending payments for BankUser
        [Authorize(Roles = "BANK_USER")]
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<ReadPaymentDto>>> GetPendingPayments()
        {
            var bankUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var payments = await _paymentService.GetPendingPaymentsByBankUserIdAsync(bankUserId);
            return Ok(payments);
        }

        // PUT: Approve payment
        [Authorize(Roles = "BANK_USER")]
        [HttpPut("{paymentId}/approve")]
        public async Task<ActionResult<ReadPaymentDto>> ApprovePayment(int paymentId)
        {
            var bankUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var result = await _paymentService.ApprovePaymentAsync(paymentId, bankUserId);
            if (result == null) return NotFound("Payment not found or not pending.");
            return Ok(result);
        }

        // PUT: Reject payment
        [Authorize(Roles = "BANK_USER")]
        [HttpPut("{paymentId}/reject")]
        public async Task<ActionResult<ReadPaymentDto>> RejectPayment(int paymentId)
        {
            var bankUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var result = await _paymentService.RejectPaymentAsync(paymentId, bankUserId);
            if (result == null) return NotFound("Payment not found or not pending.");
            return Ok(result);
        }

        // GET: Payment by ID
        [Authorize(Roles = "SUPER_ADMIN")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadPaymentDto>> GetPaymentById(int id)
        {
            var result = await _paymentService.GetPaymentsByPaymentIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
