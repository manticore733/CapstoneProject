import { Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api-service';
import { Observable } from 'rxjs';
import { Document } from '../../../core/models/Document';

@Injectable({
  providedIn: 'root',
})
export class DocumentService {
   private readonly endpoint = 'Documents';

  constructor(private api: ApiService) {}

  // Get all documents for a given client user
  getDocumentsForClient(clientUserId: number): Observable<Document[]> {
    return this.api.get<Document[]>(`${this.endpoint}?clientUserId=${clientUserId}`);
  }
  
}
