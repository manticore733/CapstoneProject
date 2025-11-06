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

  /**
   * Gets all employees for the currently logged-in Client User
   */
  getMyEmployees(): Observable<Employee[]> {
    return this.api.get<Employee[]>(`${this.endpoint}/myemployees`);
  }

  /**
   * Creates a new employee for the logged-in Client User
   */
  createEmployee(dto: CreateEmployeeDto): Observable<Employee> {
    // The backend Create returns the new EmployeeReadDto
    return this.api.post<Employee>(this.endpoint, dto);
  }

  /**
   * Updates an existing employee
   */
  updateEmployee(id: number, dto: UpdateEmployeeDto): Observable<void> {
    // The backend Update returns NoContent (void)
    return this.api.put<void>(`${this.endpoint}/${id}`, dto);
  }

  /**
   * Soft-deletes an employee
   */
  deleteEmployee(id: number): Observable<void> {
    // The backend Delete returns NoContent (void)
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }


  uploadEmployeeExcel(formData: FormData) {
    return this.api.post<any>(`${this.endpoint}/uploadExcel`, formData);
  }

  
}
