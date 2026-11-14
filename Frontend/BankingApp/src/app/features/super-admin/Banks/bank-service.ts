import { Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api-service';
import { Bank } from '../../../core/models/Bank';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BankService {

private readonly endpoint = 'Bank';

  constructor(private api: ApiService) {}

  getAll(): Observable<Bank[]> {
    return this.api.get<Bank[]>(this.endpoint);
  }

  create(data: Partial<Bank>): Observable<Bank> {
    return this.api.post<Bank>(this.endpoint, data);
  }

  update(id: number, data: Partial<Bank>): Observable<Bank> {
    return this.api.put<Bank>(`${this.endpoint}/${id}`, data);
  }

  delete(id: number): Observable<any> {
    return this.api.delete(`${this.endpoint}/${id}`);
  }
  
}
