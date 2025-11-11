import { Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api-service';
import { ClientUser } from '../../../core/models/ClientUser';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ClientUserService {
  private readonly endpoint = 'ClientUsers';

  constructor(private api: ApiService) {}

  /**
   * Gets the logged-in client user's profile
   */
  getMyProfile(): Observable<ClientUser> {
    return this.api.get<ClientUser>(`${this.endpoint}/myprofile`);
  }
  
}
