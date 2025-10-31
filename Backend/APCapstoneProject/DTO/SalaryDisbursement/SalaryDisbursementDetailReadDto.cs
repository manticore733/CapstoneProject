namespace APCapstoneProject.DTO.SalaryDisbursement
{
    public class SalaryDisbursementDetailReadDto
    {
        public int SalaryDisbursementDetailId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public decimal Amount { get; set; }
        public bool? Success { get; set; }
        public string Remark { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
