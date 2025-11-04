namespace APCapstoneProject.DTO.Reports
{
    public class PaymentTransactionDto
    {
        public int TransactionId { get; set; }
        public string BeneficiaryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }

}
