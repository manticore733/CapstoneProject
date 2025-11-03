export interface ClientUser {
  userId: number;
  userFullName: string;
  userName: string;
  userEmail: string;
  userPhone: string;
  address: string;
  statusId: number;
  statusName: string;
  accountNumber?: string;
  isActive: boolean;
  createdAt?: string;
  updatedAt?: string;
}
