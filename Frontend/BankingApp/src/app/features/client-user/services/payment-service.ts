import { Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api-service';
import { CreatePaymentDto } from '../../../core/models/CreatePaymentDto';
import { Payment } from '../../../core/models/Payment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  private readonly endpoint = 'Payments';

  constructor(private api: ApiService) {}

  /**
   * (Client) Creates a new payment request
   */
  createPayment(dto: CreatePaymentDto): Observable<Payment> {
    return this.api.post<Payment>(this.endpoint, dto);
  }

  /**
   * (Client) Gets their own payment history
   */
  getMyPayments(): Observable<Payment[]> {
    return this.api.get<Payment[]>(this.endpoint);
  }

  /**
   * (Bank User) Gets all pending payments for their clients
   */
  getPendingPayments(): Observable<Payment[]> {
    return this.api.get<Payment[]>(`${this.endpoint}/pending`);
  }

  /**
   * (Bank User) Approves a payment
   */
  approvePayment(paymentId: number): Observable<Payment> {
    return this.api.put<Payment>(`${this.endpoint}/${paymentId}/approve`, {});
  }

  /**
   * (Bank User) Rejects a payment
   */
  rejectPayment(paymentId: number): Observable<Payment> {
    return this.api.put<Payment>(`${this.endpoint}/${paymentId}/reject`, {});
  }






  
  
}
