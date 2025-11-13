import { Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api-service';
import { Observable } from 'rxjs';
import { Employee } from '../../../core/models/Employee';
import { CreateEmployeeDto } from '../../../core/models/CreateEmployeeDto';
import { UpdateEmployeeDto } from '../../../core/models/UpdateEmployeeDto';

@Injectable({
  providedIn: 'root',
})
export class EmployeeService {
  private readonly endpoint = 'Employees';

  constructor(private api: ApiService) {}


  getMyEmployees(): Observable<Employee[]> {
    return this.api.get<Employee[]>(`${this.endpoint}/myemployees`);
  }

  createEmployee(dto: CreateEmployeeDto): Observable<Employee> {

    return this.api.post<Employee>(this.endpoint, dto);
  }


  updateEmployee(id: number, dto: UpdateEmployeeDto): Observable<void> {

    return this.api.put<void>(`${this.endpoint}/${id}`, dto);
  }

  deleteEmployee(id: number): Observable<void> {

    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }


  uploadEmployeeExcel(formData: FormData) {
    return this.api.post<any>(`${this.endpoint}/uploadExcel`, formData);
  }

  
}
