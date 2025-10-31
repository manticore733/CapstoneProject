namespace APCapstoneProject.DTO.SalaryDisbursement
{
    public class ReadSalaryDisbursementDto
    {
        public int TransactionId { get; set; }
        public int ClientUserId { get; set; }

        public decimal TotalAmount { get; set; }
        public string BankName { get; set; }
        public string IFSC { get; set; }
        public string DestinationAccountNumber { get; set; }

        public string? Remarks { get; set; }
        public int StatusId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime DisbursementDate { get; set; }

        public IEnumerable<SalaryDisbursementDetailReadDto>? Details { get; set; }
    }
}
