import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ApiService } from '../../../core/services/api-service';
import { SalaryDisbursement } from '../../../core/models/SalaryDisbursement';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SalaryDisbursementService {
  private readonly endpoint = 'SalaryDisbursements';
  private baseUrl = environment.apiUrl; // Get base URL from environment

  // We need the full HttpClient here to send FormData
  constructor(private http: HttpClient, private api: ApiService) {}

  /**
   * (Client) Creates a new salary disbursement request.
   * This must send FormData, not JSON.
   */
  createDisbursement(formData: FormData): Observable<SalaryDisbursement> {
    // We use the full HttpClient 'post' method here, not the ApiService wrapper
    // because the wrapper is configured for JSON by default.
    return this.http.post<SalaryDisbursement>(
      `${this.baseUrl}/${this.endpoint}`, 
      formData
      // Angular's HttpClient handles the 'Content-Type: multipart/form-data'
      // header automatically when you send a FormData object.
    );
  }

  /**
   * (Client) Gets their own salary disbursement history
   */
  getMyDisbursements(): Observable<SalaryDisbursement[]> {
    return this.api.get<SalaryDisbursement[]>(this.endpoint);
  }

  /**
   * (Bank User) Gets pending salary disbursements
   */
  getPendingDisbursements(): Observable<SalaryDisbursement[]> {
    return this.api.get<SalaryDisbursement[]>(`${this.endpoint}/pending`);
  }

  /**
   * (Bank User) Approves a salary disbursement
   */
  approveDisbursement(disbursementId: number): Observable<SalaryDisbursement> {
    return this.api.put<SalaryDisbursement>(`${this.endpoint}/${disbursementId}/approve`, {});
  }

  /**
   * (Bank User) Rejects a salary disbursement
   */
  rejectDisbursement(disbursementId: number, payload: { bankRemark: string }): Observable<SalaryDisbursement> {
    return this.api.put<SalaryDisbursement>(`${this.endpoint}/${disbursementId}/reject`, payload);
  }
  
}
