using APCapstoneProject.DTO.Reports;

public class ClientUserReportDto
{
    public int ClientUserId { get; set; }
    public string ClientName { get; set; } = string.Empty;

    // Payments summary
    public int TotalPayments { get; set; }
    public decimal TotalPaymentAmount { get; set; }
    public int ApprovedPayments { get; set; }
    public int RejectedPayments { get; set; }

    // Salary summary
    public int TotalSalaryDisbursements { get; set; }
    public decimal TotalSalaryAmount { get; set; }

    // Data
    public List<PaymentTransactionDto> Payments { get; set; } = new();
    public List<SalaryBatchDto> SalaryDisbursements { get; set; } = new();  // ✅ new grouped structure
}
