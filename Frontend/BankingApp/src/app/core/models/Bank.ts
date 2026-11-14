export interface Bank {
  bankId: number;
  bankName: string;
  ifsc: string;
  establishmentDate: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  totalBankUsers: number;
  totalClientUserAccountsHandled: number;
}
