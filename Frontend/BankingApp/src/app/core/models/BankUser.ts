
export interface BankUser {
  userId: number;
  userFullName: string;
  userName: string;
  userEmail: string;
  userPhone: string;
  bankId: number;
  bankName?: string;
  branch: string;
  isActive: boolean;
  roleName?: string;
  createdAt?: string;
  updatedAt?: string;
  clientCount?: number;
}
