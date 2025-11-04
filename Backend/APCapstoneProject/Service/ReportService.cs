using APCapstoneProject.DTO.Reports;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using APCapstoneProject.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ClosedXML.Excel;
using Microsoft.Extensions.Options;
using System.Text;

namespace APCapstoneProject.Service
{
    public class ReportService : IReportService
    {
        private readonly IBankRepository _bankRepo;
        private readonly IClientUserRepository _clientRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly ISalaryDisbursementRepository _salaryRepo;
        private readonly IReportRecordRepository _reportRecordRepo;
        private readonly Cloudinary _cloudinary;

        public ReportService(
            IBankRepository bankRepo,
            IClientUserRepository clientRepo,
            IPaymentRepository paymentRepo,
            ISalaryDisbursementRepository salaryRepo,
            IReportRecordRepository reportRecordRepo,
            IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _bankRepo = bankRepo;
            _clientRepo = clientRepo;
            _paymentRepo = paymentRepo;
            _salaryRepo = salaryRepo;
            _reportRecordRepo = reportRecordRepo;

            var acc = new CloudinaryDotNet.Account(cloudinaryConfig.Value.CloudName, cloudinaryConfig.Value.ApiKey, cloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }

        // -------------------
        // Helper functions
        // -------------------
        private static string FormatDate(DateTime? dt) =>
            dt.HasValue ? dt.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") : "-";

        private async Task<string> UploadToCloudinaryAsync(byte[] fileBytes, string fileName, string role, int userId)
        {
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, new MemoryStream(fileBytes)),
                Folder = $"corporate_banking/reports/{role.ToLower()}/{userId}",
                PublicId = Path.GetFileNameWithoutExtension(fileName) + "_" + Guid.NewGuid().ToString(),
                Overwrite = true
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception($"Cloudinary upload failed: {result.Error.Message}");

            return result.SecureUrl.ToString();
        }

        private async Task<int> SaveReportRecordAsync(int requestedByUserId, string role, string reportName, string filters, string fileName, string fileUrl, long fileSizeBytes)
        {
            var record = new ReportRecord
            {
                RequestedByUserId = requestedByUserId,
                RequestedByRole = role,
                ReportName = reportName,
                Filters = filters,
                FileName = fileName,
                FileUrl = fileUrl,
                FileSizeBytes = fileSizeBytes,
                GeneratedAt = DateTime.UtcNow
            };

            await _reportRecordRepo.AddAsync(record);
            return record.ReportRecordId;
        }

        // -------------------
        // Data-only methods
        // -------------------
        public async Task<SystemSummaryReportDto> GetSystemSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var banks = await _bankRepo.GetAllAsync();
            var clients = await _clientRepo.GetClientUsersAsync();
            var payments = await _paymentRepo.GetAllAsync();
            var salaries = await _salaryRepo.GetAllAsync();

            if (startDate.HasValue)
            {
                payments = payments.Where(p => p.CreatedAt >= startDate.Value);
                salaries = salaries.Where(s => s.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                payments = payments.Where(p => p.CreatedAt <= endInclusive);
                salaries = salaries.Where(s => s.CreatedAt <= endInclusive);
            }

            return new SystemSummaryReportDto
            {
                TotalBanks = banks.Count(),
                TotalClients = clients.Count(),
                TotalPayments = payments.Count(),
                TotalPaymentAmount = payments.Sum(p => p.Amount),
                ApprovedPayments = payments.Count(p => p.StatusId == 1),
                RejectedPayments = payments.Count(p => p.StatusId == 2),
                PendingPayments = payments.Count(p => p.StatusId == 0),
                TotalSalaryDisbursements = salaries.Count(),
                TotalSalaryAmount = salaries.Sum(s => s.TotalAmount),
                ApprovedSalaryDisbursements = salaries.Count(s => s.StatusId == 1),
                RejectedSalaryDisbursements = salaries.Count(s => s.StatusId == 2),
                PendingSalaryDisbursements = salaries.Count(s => s.StatusId == 0)
            };
        }

        public async Task<IEnumerable<ClientTransactionReportDto>> GetClientTransactionsByBankUserAsync(int bankUserId, DateTime? startDate = null, DateTime? endDate = null, string reportType = "both")
        {
            var clients = await _clientRepo.GetClientsByBankUserIdAsync(bankUserId);
            var clientIds = clients.Select(c => c.UserId).ToList();
            reportType = (reportType ?? "both").ToLower();

            var result = new List<ClientTransactionReportDto>();

            if (reportType == "payments" || reportType == "both")
            {
                var payments = await _paymentRepo.GetAllAsync();
                payments = payments.Where(p => clientIds.Contains(p.SenderClientId));
                if (startDate.HasValue) payments = payments.Where(p => p.CreatedAt >= startDate.Value);
                if (endDate.HasValue) payments = payments.Where(p => p.CreatedAt <= endDate.Value.Date.AddDays(1).AddTicks(-1));

                result.AddRange(payments.Select(p => new ClientTransactionReportDto
                {
                    TransactionId = p.TransactionId,
                    ClientUserId = p.SenderClientId,
                    ClientName = p.SenderClient?.UserFullName ?? "",
                    Type = "Payment",
                    BeneficiaryOrEmployee = p.Beneficiary?.BeneficiaryName,
                    Amount = p.Amount,
                    Status = p.TransactionStatus?.StatusEnum.ToString() ?? "",
                    CreatedAt = p.CreatedAt,
                    ProcessedAt = p.ProcessedAt == default ? (DateTime?)null : p.ProcessedAt
                }));
            }

            if (reportType == "salaries" || reportType == "both")
            {
                var salaries = await _salaryRepo.GetByBankUserIdAsync(bankUserId);
                if (startDate.HasValue) salaries = salaries.Where(s => s.CreatedAt >= startDate.Value);
                if (endDate.HasValue) salaries = salaries.Where(s => s.CreatedAt <= endDate.Value.Date.AddDays(1).AddTicks(-1));

                result.AddRange(salaries.SelectMany(s => s.Details!.Select(d => new ClientTransactionReportDto
                {
                    TransactionId = s.TransactionId,
                    ClientUserId = s.ClientUserId,
                    ClientName = s.ClientUser?.UserFullName ?? "",
                    Type = "Salary",
                    BeneficiaryOrEmployee = d.Employee?.EmployeeName,
                    Amount = d.Amount,
                    Status = d.IsSuccessful == null
                        ? "PENDING"
                        : d.IsSuccessful.Value ? "APPROVED" : "REJECTED",
                    CreatedAt = s.CreatedAt,
                    ProcessedAt = d.ProcessedAt == default ? (DateTime?)null : d.ProcessedAt
                })));

            }

            return result.OrderByDescending(r => r.CreatedAt).ToList();
        }

        public async Task<ClientUserReportDto?> GetClientUserReportAsync(int clientUserId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var client = await _clientRepo.GetClientUserByIdAsync(clientUserId);
            if (client == null) return null;

            var payments = await _paymentRepo.GetPaymentsByClientUserIdAsync(clientUserId);
            var salaries = await _salaryRepo.GetByClientUserIdAsync(clientUserId);

            if (startDate.HasValue)
            {
                payments = payments.Where(p => p.CreatedAt >= startDate.Value);
                salaries = salaries.Where(s => s.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                payments = payments.Where(p => p.CreatedAt <= endInclusive);
                salaries = salaries.Where(s => s.CreatedAt <= endInclusive);
            }

            // --- PAYMENTS SECTION ---
            var paymentsList = payments.Select(p => new PaymentTransactionDto
            {
                TransactionId = p.TransactionId,
                BeneficiaryName = p.Beneficiary?.BeneficiaryName ?? "-",
                Amount = p.Amount,
                Status = p.TransactionStatus?.StatusEnum.ToString() ?? "PENDING",
                CreatedAt = p.CreatedAt,
                ProcessedAt = p.ProcessedAt == default ? (DateTime?)null : p.ProcessedAt
            }).ToList();

            // --- SALARIES SECTION (Grouped by Disbursement Batch) ---
            var salaryBatches = salaries.Select(s => new SalaryBatchDto
            {
                TransactionId = s.TransactionId,
                CreatedAt = s.CreatedAt,
                ProcessedAt = s.ProcessedAt == default ? (DateTime?)null : s.ProcessedAt,
                TotalAmount = s.TotalAmount,
                Status = s.TransactionStatus?.StatusEnum.ToString() ?? "PENDING",
                IsPartialSuccess = s.IsPartialSuccess ?? false,
                Employees = s.Details!.Select(d => new SalaryEmployeeDetailDto
                {
                    EmployeeName = d.Employee?.EmployeeName ?? "-",
                    Amount = d.Amount,
                    Status = d.IsSuccessful == null
                        ? "PENDING"
                        : d.IsSuccessful.Value ? "APPROVED" : "REJECTED"
                }).ToList()
            }).ToList();

            return new ClientUserReportDto
            {
                ClientUserId = clientUserId,
                ClientName = client.UserFullName,
                TotalPayments = paymentsList.Count,
                TotalPaymentAmount = paymentsList.Sum(p => p.Amount),
                ApprovedPayments = payments.Count(p => p.StatusId == 1),
                RejectedPayments = payments.Count(p => p.StatusId == 2),
                TotalSalaryDisbursements = salaries.Count(),
                TotalSalaryAmount = salaries.Sum(s => s.TotalAmount),
                Payments = paymentsList,
                SalaryDisbursements = salaryBatches
            };
        }


        // -------------------
        // Excel generation + upload + save (returns ReportResultDto)
        // -------------------
        public async Task<ReportResultDto> GenerateSystemSummaryExcelAsync(int requestedByUserId, string role, DateTime? startDate = null, DateTime? endDate = null)
        {
            var dto = await GetSystemSummaryAsync(startDate, endDate);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("System Summary");

            ws.Cell(1, 1).Value = "System Summary Report";
            ws.Range("A1:B1").Merge();
            ws.Cell(2, 1).Value = "Generated On:";
            ws.Cell(2, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ws.Cell(3, 1).Value = "Filters:";
            ws.Cell(3, 2).Value = $"startDate={(startDate?.ToString("yyyy-MM-dd") ?? "N/A")}; endDate={(endDate?.ToString("yyyy-MM-dd") ?? "N/A")}";

            int row = 5;
            ws.Cell(row++, 1).Value = "Metric"; ws.Cell(row - 1, 2).Value = "Value";
            ws.Cell(row++, 1).Value = "Total Banks"; ws.Cell(row - 1, 2).Value = dto.TotalBanks;
            ws.Cell(row++, 1).Value = "Total Clients"; ws.Cell(row - 1, 2).Value = dto.TotalClients;
            ws.Cell(row++, 1).Value = "Total Payments"; ws.Cell(row - 1, 2).Value = dto.TotalPayments;
            ws.Cell(row++, 1).Value = "Total Payment Amount"; ws.Cell(row - 1, 2).Value = dto.TotalPaymentAmount;
            ws.Cell(row++, 1).Value = "Approved Payments"; ws.Cell(row - 1, 2).Value = dto.ApprovedPayments;
            ws.Cell(row++, 1).Value = "Rejected Payments"; ws.Cell(row - 1, 2).Value = dto.RejectedPayments;
            ws.Cell(row++, 1).Value = "Pending Payments"; ws.Cell(row - 1, 2).Value = dto.PendingPayments;
            ws.Cell(row++, 1).Value = "Total Salary Disbursements"; ws.Cell(row - 1, 2).Value = dto.TotalSalaryDisbursements;
            ws.Cell(row++, 1).Value = "Total Salary Amount"; ws.Cell(row - 1, 2).Value = dto.TotalSalaryAmount;
            ws.Cell(row++, 1).Value = "Approved Salary Disbursements"; ws.Cell(row - 1, 2).Value = dto.ApprovedSalaryDisbursements;
            ws.Cell(row++, 1).Value = "Rejected Salary Disbursements"; ws.Cell(row - 1, 2).Value = dto.RejectedSalaryDisbursements;
            ws.Cell(row++, 1).Value = "Pending Salary Disbursements"; ws.Cell(row - 1, 2).Value = dto.PendingSalaryDisbursements;

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var fileBytes = stream.ToArray();

            var fileName = $"system_summary_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            var fileUrl = await UploadToCloudinaryAsync(fileBytes, fileName, role, requestedByUserId);

            var recordId = await SaveReportRecordAsync(requestedByUserId, role, "SystemSummary", $"startDate={startDate};endDate={endDate}", fileName, fileUrl, fileBytes.Length);

            return new ReportResultDto { ReportRecordId = recordId, FileUrl = fileUrl, FileName = fileName };
        }

        public async Task<ReportResultDto> GenerateBankUserReportExcelAsync(int bankUserId, string role, string reportType = "both", DateTime? startDate = null, DateTime? endDate = null)
        {
            var data = (await GetClientTransactionsByBankUserAsync(bankUserId, startDate, endDate, reportType)).ToList();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Bank Transactions");

            ws.Cell(1, 1).Value = "Bank User Transactions Report";
            ws.Range("A1:G1").Merge();
            ws.Cell(2, 1).Value = "Generated On:";
            ws.Cell(2, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ws.Cell(3, 1).Value = "Filters:";
            ws.Cell(3, 2).Value = $"reportType={reportType}; startDate={(startDate?.ToString("yyyy-MM-dd") ?? "N/A")}; endDate={(endDate?.ToString("yyyy-MM-dd") ?? "N/A")}";

            var headerRow = 5;
            var headers = new[] { "Transaction ID", "Client Name", "Type", "Beneficiary / Employee", "Amount", "Status", "Created At" };
            for (int i = 0; i < headers.Length; i++) ws.Cell(headerRow, i + 1).Value = headers[i];

            int row = headerRow + 1;
            foreach (var t in data)
            {
                ws.Cell(row, 1).Value = t.TransactionId;
                ws.Cell(row, 2).Value = t.ClientName;
                ws.Cell(row, 3).Value = t.Type;
                ws.Cell(row, 4).Value = t.BeneficiaryOrEmployee;
                ws.Cell(row, 5).Value = t.Amount;
                ws.Cell(row, 6).Value = t.Status;
                ws.Cell(row, 7).Value = t.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var bytes = stream.ToArray();

            var fileName = $"bank_user_{bankUserId}_{reportType}_report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            var fileUrl = await UploadToCloudinaryAsync(bytes, fileName, role, bankUserId);
            var recordId = await SaveReportRecordAsync(bankUserId, role, "BankUserReport", $"reportType={reportType};startDate={startDate};endDate={endDate}", fileName, fileUrl, bytes.Length);

            return new ReportResultDto { ReportRecordId = recordId, FileUrl = fileUrl, FileName = fileName };
        }

        public async Task<ReportResultDto> GenerateClientUserReportExcelAsync(int clientUserId, string role, DateTime? startDate = null, DateTime? endDate = null)
        {
            var report = await GetClientUserReportAsync(clientUserId, startDate, endDate);
            if (report == null)
                throw new KeyNotFoundException($"Client {clientUserId} not found or no data.");

            using var workbook = new XLWorkbook();

            // --- Sheet 1: Payments ---
            var paymentsSheet = workbook.Worksheets.Add("Payments");
            paymentsSheet.Cell(1, 1).Value = $"Client User Report - Payments ({report.ClientName})";
            paymentsSheet.Range("A1:F1").Merge();
            paymentsSheet.Cell(2, 1).Value = "Generated On:";
            paymentsSheet.Cell(2, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            paymentsSheet.Cell(3, 1).Value = "Filters:";
            paymentsSheet.Cell(3, 2).Value = $"startDate={(startDate?.ToString("yyyy-MM-dd") ?? "N/A")}; endDate={(endDate?.ToString("yyyy-MM-dd") ?? "N/A")}";

            paymentsSheet.Cell(5, 1).Value = "Transaction ID";
            paymentsSheet.Cell(5, 2).Value = "Beneficiary Name";
            paymentsSheet.Cell(5, 3).Value = "Amount";
            paymentsSheet.Cell(5, 4).Value = "Status";
            paymentsSheet.Cell(5, 5).Value = "Created At";
            paymentsSheet.Cell(5, 6).Value = "Processed At";

            int row = 6;
            foreach (var p in report.Payments)
            {
                paymentsSheet.Cell(row, 1).Value = p.TransactionId;
                paymentsSheet.Cell(row, 2).Value = p.BeneficiaryName;
                paymentsSheet.Cell(row, 3).Value = p.Amount;
                paymentsSheet.Cell(row, 4).Value = p.Status;
                paymentsSheet.Cell(row, 5).Value = p.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                paymentsSheet.Cell(row, 6).Value = p.ProcessedAt?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") ?? "-";
                row++;
            }
            paymentsSheet.Columns().AdjustToContents();

            // --- Sheet 2: Salary Disbursements ---
            var salarySheet = workbook.Worksheets.Add("Salary Disbursements");
            salarySheet.Cell(1, 1).Value = $"Client User Report - Salary Disbursements ({report.ClientName})";
            salarySheet.Range("A1:G1").Merge();
            salarySheet.Cell(2, 1).Value = "Generated On:";
            salarySheet.Cell(2, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            salarySheet.Cell(3, 1).Value = "Filters:";
            salarySheet.Cell(3, 2).Value = $"startDate={(startDate?.ToString("yyyy-MM-dd") ?? "N/A")}; endDate={(endDate?.ToString("yyyy-MM-dd") ?? "N/A")}";

            salarySheet.Cell(5, 1).Value = "Batch ID";
            salarySheet.Cell(5, 2).Value = "Total Amount";
            salarySheet.Cell(5, 3).Value = "Status";
            salarySheet.Cell(5, 4).Value = "Is Partial Success";
            salarySheet.Cell(5, 5).Value = "Created At";
            salarySheet.Cell(5, 6).Value = "Processed At";
            salarySheet.Cell(5, 7).Value = "Employees (Name - Amount - Status)";

            row = 6;
            foreach (var batch in report.SalaryDisbursements)
            {
                salarySheet.Cell(row, 1).Value = batch.TransactionId;
                salarySheet.Cell(row, 2).Value = batch.TotalAmount;
                salarySheet.Cell(row, 3).Value = batch.Status;
                salarySheet.Cell(row, 4).Value = batch.IsPartialSuccess ? "Yes" : "No";
                salarySheet.Cell(row, 5).Value = batch.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                salarySheet.Cell(row, 6).Value = batch.ProcessedAt?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") ?? "-";

                // Combine employee info into one string for display
                var empDetails = string.Join("\n", batch.Employees.Select(e => $"{e.EmployeeName} - {e.Amount:C} - {e.Status}"));
                salarySheet.Cell(row, 7).Value = empDetails;
                salarySheet.Cell(row, 7).Style.Alignment.WrapText = true;

                row++;
            }
            salarySheet.Columns().AdjustToContents();

            // --- Save and Upload ---
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var bytes = stream.ToArray();

            var fileName = $"client_{clientUserId}_report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            var fileUrl = await UploadToCloudinaryAsync(bytes, fileName, role, clientUserId);
            var recordId = await SaveReportRecordAsync(clientUserId, role, "ClientUserReport",
                $"startDate={startDate};endDate={endDate}", fileName, fileUrl, bytes.Length);

            return new ReportResultDto
            {
                ReportRecordId = recordId,
                FileUrl = fileUrl,
                FileName = fileName
            };
        }

    }
}
