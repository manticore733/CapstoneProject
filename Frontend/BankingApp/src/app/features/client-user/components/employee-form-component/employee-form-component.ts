

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
  @Input() employee: Employee | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();
  @Output() error = new EventEmitter<string>();  // ← NEW: Emit error to parent

  form!: FormGroup;
  loading = false;
  title = '';

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService,
  ) {}

  ngOnInit(): void {
    this.title = this.employee ? 'Edit Employee' : 'Add New Employee';

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
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    const formValue = this.form.value;

    if (this.employee) {
      // UPDATE
      const dto: UpdateEmployeeDto = { ...formValue };
      
      this.employeeService.updateEmployee(this.employee.employeeId, dto).subscribe({
        next: () => this.handleSuccess(),
        error: (err) => this.handleError(err),
      });
    } else {
      // CREATE
      const dto: CreateEmployeeDto = { ...formValue };

      this.employeeService.createEmployee(dto).subscribe({
        next: () => this.handleSuccess(),
        error: (err) => this.handleError(err),
      });
    }
  }

  private handleSuccess(): void {
    this.loading = false;
    this.saved.emit();
  }

  private handleError(err: any): void {
    this.loading = false;
    console.error('Error saving employee', err);

    // Extract error message from backend
    const errorMessage = 
      err?.error?.message ||           // { message: "..." }
      err?.error ||                     // Plain string
      'An unexpected error occurred.';

    // Emit error to parent component
    this.error.emit(errorMessage);
  }

  cancel(): void {
    this.close.emit();
  }
}
