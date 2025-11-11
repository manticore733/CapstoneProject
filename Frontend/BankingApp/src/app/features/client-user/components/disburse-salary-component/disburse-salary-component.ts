import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Employee } from '../../../../core/models/Employee';
import { EmployeeService } from '../../services/employee-service';
import { SalaryDisbursementService } from '../../services/salary-disbursement-service';
import { AccountService } from '../../services/account-service';
import { catchError, of } from 'rxjs';

@Component({
  selector: 'app-disburse-salary-component',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './disburse-salary-component.html',
  styleUrl: './disburse-salary-component.css',
})
export class DisburseSalaryComponent {
  form!: FormGroup;
  employees: Employee[] = [];
  selectedFile: File | null = null;
  selectedEmployeeIds: number[] = [];

  loading = false;
  loadingEmployees = false;
  error: string | null = null;
  accountError: string | null = null; // NEW

  // Toast notifications
  showSuccessToast = false;
  showErrorToast = false;
  successMessage = '';
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService,
    private salaryService: SalaryDisbursementService,
    private accountService: AccountService // NEW
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      disbursementMode: ['manual', [Validators.required]],
      selectedEmployeeIds: [[]],
      remarks: ['', [Validators.maxLength(200)]],
    });

    this.checkAccountStatus(); // NEW - Check status first
    this.loadEmployees();
  }

  // NEW METHOD: Check if account is approved
  checkAccountStatus(): void {
    this.accountService.getMyAccount().pipe(
      catchError(err => {
        console.error('Account error', err);
        this.accountError = "Your account is not yet approved. Please upload your documents for approval.";
        this.form.disable();
        return of(null);
      })
    ).subscribe({
      next: (account) => {
        if (!account) {
          this.accountError = "Your account is not yet approved. Please upload your documents for approval.";
          this.form.disable();
        } else {
          this.accountError = null;
          this.form.enable();
        }
      }
    });
  }

  loadEmployees(): void {
    this.loadingEmployees = true;
    this.employeeService.getMyEmployees().subscribe({
      next: (data) => {
        this.employees = data;
        this.loadingEmployees = false;
      },
      error: (err) => {
        console.error('Error fetching employees', err);
        this.error = "Your account must be approved to manage employees and disburse salaries.";
        this.loadingEmployees = false;
        this.form.disable();
      },
    });
  }

  get mode() {
    return this.form.value.disbursementMode;
  }

  toggleEmployee(employeeId: number): void {
    const index = this.selectedEmployeeIds.indexOf(employeeId);
    if (index > -1) {
      this.selectedEmployeeIds.splice(index, 1);
    } else {
      this.selectedEmployeeIds.push(employeeId);
    }
  }

  isEmployeeSelected(employeeId: number): boolean {
    return this.selectedEmployeeIds.includes(employeeId);
  }

  onFileChange(event: any): void {
    const file = event.target.files?.[0];
    if (file) {
      this.selectedFile = file;
    }
  }


//wporking
  // submitDisbursement(): void {
  //   if (this.form.invalid) {
  //     this.form.markAllAsTouched();
  //     return;
  //   }

  //   if (this.mode === 'manual' && this.selectedEmployeeIds.length === 0) {
  //     this.errorMessage = "Please select at least one employee for manual disbursement.";
  //     this.showErrorToast = true;
  //     setTimeout(() => (this.showErrorToast = false), 5000);
  //     return;
  //   }
  //   if (this.mode === 'csv' && !this.selectedFile) {
  //     this.errorMessage = "Please select a CSV file to upload.";
  //     this.showErrorToast = true;
  //     setTimeout(() => (this.showErrorToast = false), 5000);
  //     return;
  //   }

  //   this.loading = true;

  //   const formData = new FormData();
  //   formData.append('Remarks', this.form.value.remarks || '');

  //   if (this.mode === 'all') {
  //     formData.append('AllEmployees', 'true');
  //   } else if (this.mode === 'csv') {
  //     formData.append('CsvFile', this.selectedFile!);
  //   } else {
  //     this.selectedEmployeeIds.forEach(id => {
  //       formData.append('EmployeeIds', id.toString());
  //     });
  //   }

  //   this.salaryService.createDisbursement(formData).subscribe({
  //     next: (response) => {
  //       this.loading = false;
  //       this.successMessage = `Salary disbursement for ${response.totalEmployees} employees (Total: ₹${response.totalAmount.toLocaleString('en-IN')}) has been submitted for approval.`;
  //       this.showSuccessToast = true;
  //       this.resetForm();

  //       setTimeout(() => (this.showSuccessToast = false), 5000);
  //     },
  //     error: (err) => {
  //       console.error('Error creating disbursement', err);
  //       this.loading = false;

  //       let message = 'An unknown error occurred while submitting the request.';
  //       if (err.error?.message) {
  //         message = err.error.message;

  //         // 🧠 Try to extract employee names from backend error
  //         const match = message.match(/following employees:\s*(.+)\./);
  //         if (match && match[1]) {
  //           const employeeList = match[1].split(',').map(e => e.trim());
  //           // Format into multiline bullet-style for better clarity
  //           message = `Salary already disbursed this month for:\n• ${employeeList.join('\n• ')}\n\nPlease remove them before retrying.`;
  //         }
  //       } else if (err.status === 400) {
  //         message = "Invalid data. Please ensure you have selected employees or a valid CSV.";
  //       }

  //       this.errorMessage = message;
  //       this.showErrorToast = true;
  //       // setTimeout(() => (this.showErrorToast = false), 6000);
  //     },
  //   });
  // }

  submitDisbursement(retry: boolean = false): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.mode === 'manual' && this.selectedEmployeeIds.length === 0) {
      this.errorMessage = "Please select at least one employee for manual disbursement.";
      this.showErrorToast = true;
      return;
    }
    if (this.mode === 'csv' && !this.selectedFile) {
      this.errorMessage = "Please select a CSV file to upload.";
      this.showErrorToast = true;
      return;
    }

    this.loading = true;

    const formData = new FormData();
    formData.append('Remarks', this.form.value.remarks || '');

    if (this.mode === 'all') {
      formData.append('AllEmployees', 'true');
    } else if (this.mode === 'csv') {
      formData.append('CsvFile', this.selectedFile!);
    } else {
      this.selectedEmployeeIds.forEach(id => {
        formData.append('EmployeeIds', id.toString());
      });
    }

    this.salaryService.createDisbursement(formData).subscribe({
      next: (response) => {
        this.loading = false;

        // Case 1️⃣ — All employees were skipped
        if (response.totalEmployees === 0 && response.remarks === 'SKIPPED') {
          const skipped = response.skippedEmployees?.length
            ? response.skippedEmployees.join('\n• ')
            : 'No employees listed';

          this.warningToast(`⚠ All selected employees have already been paid this month:\n• ${skipped}`);
          this.resetForm();
          return;
        }

        // Case 2️⃣ — Some skipped, some paid
        if (response.remarks === 'PARTIAL') {
          const skipped = response.skippedEmployees?.length
            ? response.skippedEmployees.join('\n• ')
            : 'None';

          this.successMessage = `✅ Salary disbursement for ${response.totalEmployees} employees (₹${response.totalAmount.toLocaleString('en-IN')}) submitted for approval.\n\n⚠ Skipped these employees:\n• ${skipped}`;
          this.showSuccessToast = true;
          this.resetForm();
          return;
        }

        // Case 3️⃣ — Normal full success
        this.successMessage = `✅ Salary disbursement for ${response.totalEmployees} employees (Total: ₹${response.totalAmount.toLocaleString('en-IN')}) has been submitted for approval.`;
        this.showSuccessToast = true;
        this.resetForm();
      },
      error: (err) => {
        this.loading = false;

        const backendMessage = err.error?.message || '';
        const match = backendMessage.match(/following employees:\s*(.+)\./);

        if (match && match[1]) {
          // 🧠 Extract duplicate employees
          const duplicateNames = match[1].split(',').map((e: string) => e.trim());
          const duplicateIds = this.employees
            .filter(e => duplicateNames.includes(e.employeeName))
            .map(e => e.employeeId);

          // 🔹 Filter out duplicates and retry automatically
          if (!retry) {
            let remainingIds: number[] = [];

            if (this.mode === 'manual') {
              remainingIds = this.selectedEmployeeIds.filter(id => !duplicateIds.includes(id));
            } else if (this.mode === 'all') {
              remainingIds = this.employees
                .filter(e => !duplicateIds.includes(e.employeeId))
                .map(e => e.employeeId);
            } else if (this.mode === 'csv' && this.selectedFile) {
              // Recreate a filtered CSV dynamically
              const csvContent = this.employees
                .filter(e => !duplicateIds.includes(e.employeeId))
                .map(e => e.employeeId)
                .join('\n');

              const blob = new Blob([csvContent], { type: 'text/csv' });
              this.selectedFile = new File([blob], 'filtered.csv', { type: 'text/csv' });
            }

            if (remainingIds.length === 0) {
              this.errorMessage = `All selected employees have already been paid this month:\n• ${duplicateNames.join('\n• ')}`;
              this.showErrorToast = true;
              return;
            }

            // Retry disbursement for remaining ones
            this.selectedEmployeeIds = remainingIds;
            this.warningToast(`Skipped already-paid employees:\n• ${duplicateNames.join('\n• ')}\nContinuing with the rest...`);
            this.submitDisbursement(true);
            return;
          }
        }

        // 🟥 Fallback for generic errors
        this.errorMessage =
          backendMessage ||
          (err.status === 400
            ? 'Invalid data. Please ensure you have selected employees or a valid CSV.'
            : 'An unknown error occurred while submitting the request.');
        this.showErrorToast = true;
      },
    });
  }


  resetForm(): void {
    this.form.reset({ disbursementMode: 'manual' });
    this.selectedFile = null;
    this.selectedEmployeeIds = [];
  }

  closeSuccessToast(): void {
    this.showSuccessToast = false;
  }

  closeErrorToast(): void {
    this.showErrorToast = false;
  }
  warningToast(message: string): void {
    this.errorMessage = message;
    this.showErrorToast = true;
  }

}
