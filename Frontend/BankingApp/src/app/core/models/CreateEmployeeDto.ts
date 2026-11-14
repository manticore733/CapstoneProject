export interface CreateEmployeeDto {
  employeeName: string;
  email: string;
  accountNumber: string;
  bankName: string;
  ifsc: string;
  salary: number;
  dateOfJoining: string; // HTML date input provides this
}

