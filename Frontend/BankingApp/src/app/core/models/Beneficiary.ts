export interface Beneficiary {
  beneficiaryId: number;
  clientUserId: number;
  beneficiaryName: string;
  accountNumber: string;
  bankName: string;
  ifsc: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string | null;
}