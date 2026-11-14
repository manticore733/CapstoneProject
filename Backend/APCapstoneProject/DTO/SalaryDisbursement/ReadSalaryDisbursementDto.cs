namespace APCapstoneProject.DTO.SalaryDisbursement
{
    public class ReadSalaryDisbursementDto
    {
        public int TransactionId { get; set; }
        public int ClientUserId { get; set; }

        public decimal TotalAmount { get; set; }

        public string? Remarks { get; set; } // given while making payment
        public string? BankRemark { get; set; } // reason for approving or denying paynment

        public string TransactionStatus { get; set; }

        public int TotalEmployees { get; set; }
        public int SuccessfulCount { get; set; }
        public int FailedCount { get; set; }
        public bool? IsPartialSuccess { get; set; }

        public List<string>? SkippedEmployees { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime DisbursementDate { get; set; }

        public IEnumerable<SalaryDisbursementDetailReadDto>? Details { get; set; }
    }
}
