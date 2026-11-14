namespace APCapstoneProject.DTO.Reports
{
    public class SalaryBatchDto
    {
        public int TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsPartialSuccess { get; set; }
        public List<SalaryEmployeeDetailDto> Employees { get; set; } = new();
    }
}
