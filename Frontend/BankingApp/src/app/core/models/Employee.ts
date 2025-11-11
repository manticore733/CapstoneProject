export interface Employee {
  employeeId: number;
  clientUserId: number;
  employeeName: string;
  email: string;
  accountNumber: string;
  bankName: string;
  ifsc: string;
  salary: number;
  dateOfJoining: string; // Comes as a string, e.g., "2023-10-28T00:00:00"
  dateOfLeaving?: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string | null;
}