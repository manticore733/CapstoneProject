export interface SalaryDisbursementDetail {
  salaryDisbursementDetailId: number;
  employeeId: number;
  employeeName: string;
  bankName: string;
  ifsc: string;
  destinationAccountNumber: string;
  amount: number;
  isSuccessful?: boolean | null;
  remark: string;
  processedAt: string;
}