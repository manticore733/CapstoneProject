namespace APCapstoneProject.DTO.Payment
{
    public class ReadPaymentDto
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionStatus { get; set; }
        public string BeneficiaryName { get; set; }
        public string DestinationAccountNumber { get; set; }
        public string BankName { get; set; }
        public string IFSC { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? Remarks { get; set; }
    }
}
