import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ApiService } from '../../../core/services/api-service';
import { Observable } from 'rxjs';
import { Document } from '../../../core/models/Document';

@Injectable({
  providedIn: 'root',
})
export class DocumentUploadService {

  private readonly endpoint = 'Documents';
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private api: ApiService) {}

 
  uploadDocument(proofTypeId: number, file: File): Observable<Document> {
    const formData = new FormData();
    formData.append('proofTypeId', proofTypeId.toString());
    formData.append('file', file, file.name);


    return this.http.post<Document>(
      `${this.baseUrl}/${this.endpoint}`,
      formData
    );
  }


  getMyDocuments(): Observable<Document[]> {
    return this.api.get<Document[]>(`${this.endpoint}/mydocuments`);
  }
  
}
