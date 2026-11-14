namespace APCapstoneProject.DTO.Reports
{
    public class ClientTransactionReportDto
    {
        public int TransactionId { get; set; }
        public int ClientUserId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Payment" | "Salary"
        public string? BeneficiaryOrEmployee { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; } // nullable to avoid ToLocalTime errors
    }
}
