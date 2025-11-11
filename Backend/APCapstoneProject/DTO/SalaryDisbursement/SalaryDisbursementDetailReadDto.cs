namespace APCapstoneProject.DTO.SalaryDisbursement
{
    public class SalaryDisbursementDetailReadDto
    {
        public int SalaryDisbursementDetailId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string IFSC { get; set; } = string.Empty;
        public string DestinationAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public bool? IsSuccessful { get; set; }
        public string Remark { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
