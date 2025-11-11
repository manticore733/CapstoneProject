import { Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api-service';
import { ClientUser } from '../../../core/models/ClientUser';
import { Observable } from 'rxjs';
import { ClientApproval } from '../../../core/models/ClientApproval';

@Injectable({
  providedIn: 'root',
})
export class ClientService {

  private readonly endpoint = 'ClientUsers';
  private readonly approveEndpoint = 'BankUsers';

  constructor(private api: ApiService) {}

  // ✅ Get all clients for the logged-in Bank User
  getMyClients(): Observable<ClientUser[]> {
    return this.api.get<ClientUser[]>(`${this.endpoint}/myclients`);
  }

  // ✅ Get a single client (details view)
   getClientById(clientId: number): Observable<ClientUser> {
    return this.api.get<ClientUser>(`${this.endpoint}/${clientId}`);
  }

  // ✅ Update client details (Bank User can only update limited info)
  updateClient(id: number, data: Partial<ClientUser>): Observable<ClientUser> {
    return this.api.put<ClientUser>(`${this.endpoint}/${id}`, data);
  }

  // ✅ Delete a client (soft delete)
  deleteClient(id: number): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  // ✅ Approve or Reject a client
  approveClient(clientId: number, data: ClientApproval): Observable<ClientUser> {
    return this.api.put<ClientUser>(`${this.approveEndpoint}/${clientId}/approve`, data);
  }

  // ✅ new method
  createClient(data: any): Observable<ClientUser> {
    return this.api.post<ClientUser>(`${this.endpoint}`, data);
  }
 
  
}
