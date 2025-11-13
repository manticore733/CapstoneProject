import { Injectable } from '@angular/core';
import { CreateBeneficiaryDto } from '../../../core/models/CreateBeneficiaryDto';
import { Beneficiary } from '../../../core/models/Beneficiary';
import { Observable } from 'rxjs';
import { UpdateBeneficiaryDto } from '../../../core/models/UpdateBeneficiaryDto';
import { ApiService } from '../../../core/services/api-service';

@Injectable({
  providedIn: 'root',
})
export class BeneficiaryService {
  private readonly endpoint = 'Beneficiary';

  constructor(private api: ApiService) {}


  getMyBeneficiaries(): Observable<Beneficiary[]> {
    return this.api.get<Beneficiary[]>(`${this.endpoint}/mybeneficiaries`);
  }


  createBeneficiary(dto: CreateBeneficiaryDto): Observable<Beneficiary> {
    return this.api.post<Beneficiary>(this.endpoint, dto);
  }


  updateBeneficiary(id: number, dto: UpdateBeneficiaryDto): Observable<Beneficiary> {
    return this.api.put<Beneficiary>(`${this.endpoint}/${id}`, dto);
  }


  deleteBeneficiary(id: number): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
  
}
