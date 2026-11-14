namespace APCapstoneProject.DTO.Reports
{
    public class SystemSummaryReportDto
    {
        public int TotalBanks { get; set; }
        public int TotalClients { get; set; }

        public int TotalPayments { get; set; }
        public decimal TotalPaymentAmount { get; set; }
        public int ApprovedPayments { get; set; }
        public int RejectedPayments { get; set; }
        public int PendingPayments { get; set; }

        public int TotalSalaryDisbursements { get; set; }
        public decimal TotalSalaryAmount { get; set; }
        public int ApprovedSalaryDisbursements { get; set; }
        public int RejectedSalaryDisbursements { get; set; }
        public int PendingSalaryDisbursements { get; set; }
    }
}
