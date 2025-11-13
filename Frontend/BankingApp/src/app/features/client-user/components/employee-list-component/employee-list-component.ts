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

  searchTerm = '';
  currentPage = 1;
  itemsPerPage = 10;

  showForm = false;
  selectedEmployee: Employee | null = null;

  toasts: { message: string; type: 'success' | 'error' | 'warning' | 'info'; autoClose?: boolean }[] = [];

  constructor(private employeeService: EmployeeService) {}

  ngOnInit(): void {
    this.loadEmployees();
  }

  // Toast Methods
  showToast(
    message: string,
    type: 'success' | 'error' | 'warning' | 'info' = 'info',
    autoClose: boolean = true
  ) {
    const toast = { message, type, autoClose };
    this.toasts.push(toast);

    if (autoClose) {
      setTimeout(() => {
        this.toasts.shift();
      }, 5000);  // 5 seconds
    }
  }

  closeToast(index: number) {
    this.toasts.splice(index, 1);
  }

  getToastClass(type: string): string {
    switch (type) {
      case 'success':
        return 'bg-green-50 border-l-4 border-green-500 text-green-700';
      case 'error':
        return 'bg-red-50 border-l-4 border-red-500 text-red-700';
      case 'warning':
        return 'bg-yellow-50 border-l-4 border-yellow-500 text-yellow-700';
      default:
        return 'bg-blue-50 border-l-4 border-blue-500 text-blue-700';
    }
  }

  // Excel Upload
  onExcelSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    if (!file.name.endsWith('.xlsx') && !file.name.endsWith('.xls')) {
      this.showToast('Please upload a valid Excel file (.xlsx or .xls)', 'warning');
      input.value = '';
      return;
    }

    const formData = new FormData();
    formData.append('file', file);

    this.loading = true;
    this.showToast('Uploading Excel file...', 'info');

    this.employeeService.uploadEmployeeExcel(formData).subscribe({
      next: (result) => {
        if (result.failedCount > 0) {
          this.showToast(
            `Upload completed: ${result.successCount} succeeded, ${result.failedCount} failed.`,
            'warning'
          );

          result.failedRows.forEach((row: any) => {
            this.showToast(
              `Row ${row.rowNumber}: ${row.errorMessage}`,
              'error',
              false
            );
          });
        } else {
          this.showToast(
            `All ${result.successCount} employees uploaded successfully!`,
            'success'
          );
        }

        this.loadEmployees();
        this.loading = false;
      },
      error: (err) => {
        console.error('Excel upload failed:', err);
        this.showToast(err.error || 'Failed to upload Excel. Please try again.', 'error');
        this.loading = false;
      },
    });

    input.value = '';
  }

  // Load Employees
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

  // Filters
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
    const isEdit = !!this.selectedEmployee;
    this.loadEmployees();
    this.closeForm();

    this.showToast(
      isEdit ? 'Employee updated successfully!' : 'New employee added successfully!',
      'success'
    );
  }

  //  Handle form errors
  onFormError(errorMessage: string): void {
    // Determine toast type based on error message
    if (errorMessage.includes('already exists') || errorMessage.includes('duplicate')) {
      this.showToast(`⚠️ ${errorMessage}`, 'warning');
    } else if (errorMessage.includes('required') || errorMessage.includes('invalid')) {
      this.showToast(`❌ ${errorMessage}`, 'error');
    } else if (errorMessage.includes('inactive') || errorMessage.includes('not active')) {
      this.showToast(`🔒 ${errorMessage}`, 'warning');
    } else {
      this.showToast(`❌ ${errorMessage}`, 'error');
    }
  }

  // Delete Employee
  deleteEmployee(id: number): void {
    if (confirm('Are you sure you want to delete this employee?')) {
      this.employeeService.deleteEmployee(id).subscribe({
        next: () => {
          this.loadEmployees();
          this.showToast('Employee deleted successfully!', 'success');
        },
        error: (err) => {
          console.error('Error deleting employee', err);
          this.showToast('Failed to delete employee. Please try again.', 'error');
        },
      });
    }
  }
}
