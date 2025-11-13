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
  private baseUrl = environment.apiUrl; 

  constructor(private http: HttpClient, private api: ApiService) {}


  createDisbursement(formData: FormData): Observable<SalaryDisbursement> {
    // no ApiService wrapper because the wrapper is configured for JSON by default.
    return this.http.post<SalaryDisbursement>(
      `${this.baseUrl}/${this.endpoint}`,
      formData
    );
  }

  getMyDisbursements(): Observable<SalaryDisbursement[]> {
    return this.api.get<SalaryDisbursement[]>(this.endpoint);
  }

  getPendingDisbursements(): Observable<SalaryDisbursement[]> {
    return this.api.get<SalaryDisbursement[]>(`${this.endpoint}/pending`);
  }

  approveDisbursement(disbursementId: number): Observable<SalaryDisbursement> {
    return this.api.put<SalaryDisbursement>(`${this.endpoint}/${disbursementId}/approve`, {});
  }


  rejectDisbursement(disbursementId: number, payload: { bankRemark: string }): Observable<SalaryDisbursement> {
    return this.api.put<SalaryDisbursement>(`${this.endpoint}/${disbursementId}/reject`, payload);
  }
  
}
