// Matches ClientUserReportDto
export interface ClientUserReportDto {
  clientUserId: number;
  clientName: string;
  totalPayments: number;
  totalPaymentAmount: number;
  approvedPayments: number;
  rejectedPayments: number;
  totalSalaryDisbursements: number;
  totalSalaryAmount: number;
  payments: PaymentTransactionDto[];
  salaryDisbursements: SalaryBatchDto[];
}

// Matches PaymentTransactionDto
export interface PaymentTransactionDto {
  transactionId: number;
  beneficiaryName: string;
  amount: number;
  status: string;
  createdAt: string;
  processedAt?: string | null;
}

// Matches SalaryBatchDto
export interface SalaryBatchDto {
  transactionId: number;
  createdAt: string;
  processedAt?: string | null;
  totalAmount: number;
  status: string;
  isPartialSuccess: boolean;
  employees: SalaryEmployeeDetailDto[];
}

// Matches SalaryEmployeeDetailDto
export interface SalaryEmployeeDetailDto {
  employeeName: string;
  amount: number;
  status: string;
}

// Matches ClientTransactionReportDto
export interface ClientTransactionReportDto {
  transactionId: number;
  clientUserId: number;
  clientName: string;
  type: string; // "Payment" | "Salary"
  beneficiaryOrEmployee?: string;
  amount: number;
  status: string;
  createdAt: string;
  processedAt?: string | null;
}

// Matches SystemSummaryReportDto
export interface SystemSummaryReportDto {
  totalBanks: number;
  totalClients: number;
  totalPayments: number;
  totalPaymentAmount: number;
  approvedPayments: number;
  rejectedPayments: number;
  pendingPayments: number;
  totalSalaryDisbursements: number;
  totalSalaryAmount: number;
  approvedSalaryDisbursements: number;
  rejectedSalaryDisbursements: number;
  pendingSalaryDisbursements: number;
}

// Matches ReportResultDto
export interface ReportResultDto {
  reportRecordId: number;
  fileUrl: string;
  fileName: string;
}

// Matches ReportRecord
export interface ReportRecord {
  reportRecordId: number;
  requestedByUserId: number;
  requestedByRole: string;
  reportName: string;
  filters: string;
  generatedAt: string;
  fileName: string;
  fileUrl: string;
  fileSizeBytes: number;
}