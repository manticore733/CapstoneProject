using APCapstoneProject.DTO.Reports;

namespace APCapstoneProject.Service
{
    public interface IReportService
    {
        // Data-only report getters (useful for UI)
        Task<SystemSummaryReportDto> GetSystemSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<ClientTransactionReportDto>> GetClientTransactionsByBankUserAsync(int bankUserId, DateTime? startDate = null, DateTime? endDate = null, string reportType = "both");
        Task<ClientUserReportDto?> GetClientUserReportAsync(int clientUserId, DateTime? startDate = null, DateTime? endDate = null);

        // Generate and persist Excel, return report result (url + id)
        Task<ReportResultDto> GenerateSystemSummaryExcelAsync(int requestedByUserId, string role, DateTime? startDate = null, DateTime? endDate = null);
        Task<ReportResultDto> GenerateBankUserReportExcelAsync(int bankUserId, string role, string reportType = "both", DateTime? startDate = null, DateTime? endDate = null);
        Task<ReportResultDto> GenerateClientUserReportExcelAsync(int clientUserId, string role, DateTime? startDate = null, DateTime? endDate = null);
    }
}
