export interface CreatePaymentDto {
  beneficiaryId: number;
  amount: number;
  remarks?: string | null;
}