
import { Component } from '@angular/core';
import { Employee } from '../../../../core/models/Employee';
import { EmployeeService } from '../../services/employee-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeFormComponent } from '../employee-form-component/employee-form-component';

@Component({
  selector: 'app-employee-list-component',
  imports: [CommonModule, FormsModule, EmployeeFormComponent],
  templateUrl: './employee-list-component.html',
  styleUrl: './employee-list-component.css',
})
export class EmployeeListComponent {
  employees: Employee[] = [];
  filteredEmployees: Employee[] = [];
  loading = false;
  error: string | null = null;

  // Search
  searchTerm = '';

  // Pagination
  currentPage = 1;
  itemsPerPage = 10;

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
        this.filteredEmployees = [...data];
        this.loading = false;
        this.error = null;
      },
      error: (err) => {
        console.error('Error fetching employees', err);
        if (err.status === 404 || err.status === 403) {
          this.error = "Your account is not active or approved. Please upload documents to enable this feature.";
        } else {
          this.error = 'Failed to load employees.';
        }
        this.loading = false;
      },
    });
  }

  // Search and Filter
  applyFilters(): void {
    this.filteredEmployees = this.employees.filter(e => {
      const matchesSearch = !this.searchTerm ||
        e.employeeName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        e.email.toLowerCase().includes(this.searchTerm.toLowerCase());

      return matchesSearch;
    });
    this.currentPage = 1;
  }

  // Pagination
  getPaginatedEmployees(): Employee[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.filteredEmployees.slice(start, end);
  }

  getStartIndex(): number {
    return (this.currentPage - 1) * this.itemsPerPage;
  }

  getEndIndex(): number {
    return Math.min(this.currentPage * this.itemsPerPage, this.filteredEmployees.length);
  }

  getTotalPages(): number {
    return Math.ceil(this.filteredEmployees.length / this.itemsPerPage);
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  nextPage(): void {
    if (this.currentPage < this.getTotalPages()) {
      this.currentPage++;
    }
  }

  // Form Controls
  openAddForm(): void {
    this.selectedEmployee = null;
    this.showForm = true;
  }

  openEditForm(employee: Employee): void {
    this.selectedEmployee = { ...employee };
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
    this.selectedEmployee = null;
  }

  onFormSaved(): void {
    this.loadEmployees();
    this.closeForm();
  }

  // CRUD Actions
  deleteEmployee(id: number): void {
    if (confirm('Are you sure you want to delete this employee?')) {
      this.employeeService.deleteEmployee(id).subscribe({
        next: () => {
          this.loadEmployees();
        },
        error: (err) => {
          console.error('Error deleting employee', err);
          alert('Failed to delete employee.');
        },
      });
    }
  }
}
