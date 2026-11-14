export interface Payment {
  transactionId: number;
  amount: number;
  transactionStatus: string;
  beneficiaryName: string;
  destinationAccountNumber: string;
  bankName: string;
  ifsc: string;
  createdAt: string;
  approvedAt?: string | null;
  processedAt?: string | null;
  remarks?: string | null;
}