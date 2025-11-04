import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Employee } from '../../../../core/models/Employee';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EmployeeService } from '../../services/employee-service';
import { CommonModule, formatDate } from '@angular/common';
import { UpdateEmployeeDto } from '../../../../core/models/UpdateEmployeeDto';
import { CreateEmployeeDto } from '../../../../core/models/CreateEmployeeDto';

@Component({
  selector: 'app-employee-form-component',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './employee-form-component.html',
  styleUrl: './employee-form-component.css',
})
export class EmployeeFormComponent {

  @Input() employee: Employee | null = null; // for edit mode
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  form!: FormGroup;
  loading = false;
  title = '';

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService
  ) {}

  ngOnInit(): void {
    this.title = this.employee ? 'Edit Employee' : 'Add New Employee';

    // Helper to format the date for the HTML date input
    const joiningDate = this.employee 
      ? formatDate(this.employee.dateOfJoining, 'yyyy-MM-dd', 'en-US') 
      : '';

    this.form = this.fb.group({
      employeeName: [this.employee?.employeeName || '', [Validators.required]],
      email: [this.employee?.email || '', [Validators.required, Validators.email]],
      accountNumber: [this.employee?.accountNumber || '', [Validators.required, Validators.pattern(/^[0-9]+$/)]],
      bankName: [this.employee?.bankName || '', [Validators.required]],
      ifsc: [this.employee?.ifsc || '', [Validators.required]],
      salary: [this.employee?.salary || '', [Validators.required, Validators.min(0.01)]],
      dateOfJoining: [joiningDate, [Validators.required]],
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched(); // Show validation errors
      return;
    }

    this.loading = true;
    const formValue = this.form.value;

    if (this.employee) {
      // --- UPDATE (Edit) ---
      const dto: UpdateEmployeeDto = { ...formValue };
      
      this.employeeService.updateEmployee(this.employee.employeeId, dto).subscribe({
        next: () => this.handleSuccess(),
        error: (err) => this.handleError(err),
      });

    } else {
      // --- CREATE (Add) ---
      const dto: CreateEmployeeDto = { ...formValue };

      this.employeeService.createEmployee(dto).subscribe({
        next: () => this.handleSuccess(),
        error: (err) => this.handleError(err),
      });
    }
  }

  private handleSuccess(): void {
    this.loading = false;
    this.saved.emit(); // Tell parent to refresh
  }

  private handleError(err: any): void {
    console.error('Error saving employee', err);
    this.loading = false;
    alert('An error occurred. Please try again.');
  }

  cancel(): void {
    this.close.emit(); // Tell parent to close
  }

}
