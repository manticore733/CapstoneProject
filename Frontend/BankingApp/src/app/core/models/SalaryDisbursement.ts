import { SalaryDisbursementDetail } from "./SalaryDisbursementDetail";

export interface SalaryDisbursement {
  transactionId: number;
  clientUserId: number;
  totalAmount: number;
  remarks?: string | null;
  transactionStatus: string;
  totalEmployees: number;
  successfulCount: number;
  failedCount: number;
  isPartialSuccess?: boolean | null;
  createdAt: string;
  processedAt?: string | null;
  disbursementDate: string;
  details?: SalaryDisbursementDetail[] | null;
}