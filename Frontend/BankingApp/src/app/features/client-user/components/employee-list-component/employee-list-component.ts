import { Component } from '@angular/core';
import { Employee } from '../../../../core/models/Employee';
import { EmployeeService } from '../../services/employee-service';
import { CommonModule } from '@angular/common';
import { EmployeeFormComponent } from '../employee-form-component/employee-form-component';

@Component({
  selector: 'app-employee-list-component',
  imports: [CommonModule, EmployeeFormComponent],
  templateUrl: './employee-list-component.html',
  styleUrl: './employee-list-component.css',
})
export class EmployeeListComponent {

  employees: Employee[] = [];
  loading = false;
  error: string | null = null;
  
  showForm = false;
  selectedEmployee: Employee | null = null;

  constructor(private employeeService: EmployeeService) {}

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.loading = true;
    this.employeeService.getMyEmployees().subscribe({
      next: (data) => {
        this.employees = data;
        this.loading = false;
        this.error = null;
      },
      error: (err) => {
        console.error('Error fetching employees', err);
        // This handles the API block for pending/inactive users
        if (err.status === 404 || err.status === 403) {
          this.error = "Your account is not active or approved. Please upload documents to enable this feature.";
        } else {
          this.error = 'Failed to load employees.';
        }
        this.loading = false;
      },
    });
  }

  // --- Form/Modal Controls ---

  openAddForm(): void {
    this.selectedEmployee = null;
    this.showForm = true;
  }

  openEditForm(employee: Employee): void {
    this.selectedEmployee = { ...employee }; // Pass a copy
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
    this.selectedEmployee = null;
  }

  onFormSaved(): void {
    this.loadEmployees(); // Refresh the list
    this.closeForm();
  }

  // --- CRUD Actions ---

  deleteEmployee(id: number): void {
    if (confirm('Are you sure you want to delete this employee?')) {
      this.employeeService.deleteEmployee(id).subscribe({
        next: () => {
          this.loadEmployees(); // Refresh on success
        },
        error: (err) => {
          console.error('Error deleting employee', err);
          alert('Failed to delete employee.');
        },
      });
    }
  }

}
