import { Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api-service';
import { Account } from '../../../core/models/ReadAccountDto';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private readonly endpoint = 'Accounts';

  constructor(private api: ApiService) {}

  /**
   * (Client) Gets their own account details (number, balance)
   */
  getMyAccount(): Observable<Account> {
    return this.api.get<Account>(`${this.endpoint}/myaccount`);
  }
  
}
