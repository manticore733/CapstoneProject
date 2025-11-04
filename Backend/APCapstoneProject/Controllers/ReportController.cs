using APCapstoneProject.DTO.Reports;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APCapstoneProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst("UserId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(claim?.Value, out var id) ? id : 0;
        }

        // 🔹 SYSTEM SUMMARY REPORT (SUPER ADMIN)
        [HttpGet("system-summary")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> GetSystemSummary(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? export = null)
        {
            int userId = GetCurrentUserId();

            if (export?.ToLower() == "excel")
            {
                var result = await _reportService.GenerateSystemSummaryExcelAsync(userId, "SUPER_ADMIN", startDate, endDate);
                return Ok(new
                {
                    message = "Excel report generated successfully.",
                    reportId = result.ReportRecordId,
                    fileUrl = result.FileUrl,
                    fileName = result.FileName
                });
            }

            var data = await _reportService.GetSystemSummaryAsync(startDate, endDate);
            return Ok(data);
        }

        // 🔹 BANK USER REPORT (TRANSACTIONS / PAYMENTS / SALARIES)
        [HttpGet("bank/{bankUserId}/transactions")]
        //[Authorize(Roles = "BANK_USER")]
        public async Task<IActionResult> GetBankUserReport(
            int bankUserId,
            [FromQuery] string reportType = "both", //"salaries" or "payments"
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? export = null)
        {
            if (export?.ToLower() == "excel")
            {
                var result = await _reportService.GenerateBankUserReportExcelAsync(bankUserId, "BANK_USER", reportType, startDate, endDate);
                return Ok(new
                {
                    message = $"Excel report generated for {reportType.ToUpper()}",
                    reportId = result.ReportRecordId,
                    fileUrl = result.FileUrl,
                    fileName = result.FileName
                });
            }

            var data = await _reportService.GetClientTransactionsByBankUserAsync(bankUserId, startDate, endDate, reportType);
            return Ok(data);
        }

        // 🔹 CLIENT USER REPORT (PAYMENTS + SALARIES)
        [HttpGet("client/{clientUserId}")]
        //[Authorize(Roles = "CLIENT_USER")]
        public async Task<IActionResult> GetClientUserReport(
            int clientUserId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? export = null)
        {
            if (export?.ToLower() == "excel")
            {
                var result = await _reportService.GenerateClientUserReportExcelAsync(clientUserId, "CLIENT_USER", startDate, endDate);
                return Ok(new
                {
                    message = "Excel report generated successfully.",
                    reportId = result.ReportRecordId,
                    fileUrl = result.FileUrl,
                    fileName = result.FileName
                });
            }

            var data = await _reportService.GetClientUserReportAsync(clientUserId, startDate, endDate);
            if (data == null) return NotFound();
            return Ok(data);
        }
    }
}
