import { Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api-service';
import { Observable, tap } from 'rxjs';
import { BankUser } from '../../../core/models/BankUser';
import { CreateBankUser } from '../../../core/models/CreateBankUser';

@Injectable({
  providedIn: 'root',
})
export class BankUserService {

 private readonly endpoint = 'BankUsers';

  constructor(private api: ApiService) {}

  getAll(): Observable<BankUser[]> {
    return this.api.get<BankUser[]>(this.endpoint);
  }

  create(data: CreateBankUser): Observable<BankUser> {
    return this.api.post<BankUser>(this.endpoint, data);
  }

  update(id: number, data: Partial<BankUser>): Observable<BankUser> {
    return this.api.put<BankUser>(`${this.endpoint}/${id}`, data);
  }

  delete(id: number): Observable<any> {
    return this.api.delete(`${this.endpoint}/${id}`);
  }
}
