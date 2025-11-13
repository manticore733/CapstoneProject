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

  createPayment(dto: CreatePaymentDto): Observable<Payment> {
    return this.api.post<Payment>(this.endpoint, dto);
  }


  getMyPayments(): Observable<Payment[]> {
    return this.api.get<Payment[]>(this.endpoint);
  }

  getPendingPayments(): Observable<Payment[]> {
    return this.api.get<Payment[]>(`${this.endpoint}/pending`);
  }


  approvePayment(paymentId: number): Observable<Payment> {
    return this.api.put<Payment>(`${this.endpoint}/${paymentId}/approve`, {});
  }

  rejectPayment(paymentId: number, payload: { bankRemark: string }): Observable<Payment> {
    return this.api.put<Payment>(`${this.endpoint}/${paymentId}/reject`, payload);
  }






  
  
}
