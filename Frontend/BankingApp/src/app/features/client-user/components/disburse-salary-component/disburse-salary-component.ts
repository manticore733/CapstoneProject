

// import { CommonModule } from '@angular/common';
// import { Component } from '@angular/core';
// import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
// import { Employee } from '../../../../core/models/Employee';
// import { EmployeeService } from '../../services/employee-service';
// import { SalaryDisbursementService } from '../../services/salary-disbursement-service';

// @Component({
//   selector: 'app-disburse-salary-component',
//   imports: [CommonModule, ReactiveFormsModule],
//   templateUrl: './disburse-salary-component.html',
//   styleUrl: './disburse-salary-component.css',
// })
// export class DisburseSalaryComponent {
//   form!: FormGroup;
//   employees: Employee[] = [];
//   selectedFile: File | null = null;
//   selectedEmployeeIds: number[] = [];
  
//   loading = false;
//   loadingEmployees = false;
//   error: string | null = null;
  
//   // Toast notifications
//   showSuccessToast = false;
//   showErrorToast = false;
//   successMessage = '';
//   errorMessage = '';

//   constructor(
//     private fb: FormBuilder,
//     private employeeService: EmployeeService,
//     private salaryService: SalaryDisbursementService
//   ) {}

//   ngOnInit(): void {
//     this.form = this.fb.group({
//       disbursementMode: ['manual', [Validators.required]], 
//       selectedEmployeeIds: [[]],
//       remarks: ['', [Validators.maxLength(200)]],
//     });

//     this.loadEmployees();
//   }

//   loadEmployees(): void {
//     this.loadingEmployees = true;
//     this.employeeService.getMyEmployees().subscribe({
//       next: (data) => {
//         this.employees = data;
//         this.loadingEmployees = false;
//       },
//       error: (err) => {
//         console.error('Error fetching employees', err);
//         this.error = "Your account must be approved to manage employees and disburse salaries.";
//         this.loadingEmployees = false;
//         this.form.disable();
//       },
//     });
//   }

//   get mode() {
//     return this.form.value.disbursementMode;
//   }

//   toggleEmployee(employeeId: number): void {
//     const index = this.selectedEmployeeIds.indexOf(employeeId);
//     if (index > -1) {
//       this.selectedEmployeeIds.splice(index, 1);
//     } else {
//       this.selectedEmployeeIds.push(employeeId);
//     }
//   }

//   isEmployeeSelected(employeeId: number): boolean {
//     return this.selectedEmployeeIds.includes(employeeId);
//   }

//   onFileChange(event: any): void {
//     const file = event.target.files?.[0];
//     if (file) {
//       this.selectedFile = file;
//     }
//   }

//   submitDisbursement(): void {
//     if (this.form.invalid) {
//       this.form.markAllAsTouched();
//       return;
//     }

//     // Validation
//     if (this.mode === 'manual' && this.selectedEmployeeIds.length === 0) {
//       this.errorMessage = "Please select at least one employee for manual disbursement.";
//       this.showErrorToast = true;
//       setTimeout(() => this.showErrorToast = false, 5000);
//       return;
//     }
//     if (this.mode === 'csv' && !this.selectedFile) {
//       this.errorMessage = "Please select a CSV file to upload.";
//       this.showErrorToast = true;
//       setTimeout(() => this.showErrorToast = false, 5000);
//       return;
//     }

//     this.loading = true;

//     const formData = new FormData();
//     formData.append('Remarks', this.form.value.remarks || '');

//     if (this.mode === 'all') {
//       formData.append('AllEmployees', 'true');
//     } else if (this.mode === 'csv') {
//       formData.append('CsvFile', this.selectedFile!);
//     } else {
//       this.selectedEmployeeIds.forEach(id => {
//         formData.append('EmployeeIds', id.toString());
//       });
//     }

//     this.salaryService.createDisbursement(formData).subscribe({
//       next: (response) => {
//         this.loading = false;
//         this.successMessage = `Salary disbursement for ${response.totalEmployees} employees (Total: ₹${response.totalAmount.toLocaleString('en-IN')}) has been submitted for approval.`;
//         this.showSuccessToast = true;
//         this.resetForm();
        
//         setTimeout(() => this.showSuccessToast = false, 5000);
//       },
//       error: (err) => {
//         console.error('Error creating disbursement', err);
//         this.loading = false;
        
//         if (err.error?.message) {
//           this.errorMessage = err.error.message;
//         } else if (err.status === 400) {
//           this.errorMessage = "Invalid data. Please ensure you have selected employees or a valid CSV.";
//         } else {
//           this.errorMessage = 'An unknown error occurred while submitting the request.';
//         }
        
//         this.showErrorToast = true;
//         setTimeout(() => this.showErrorToast = false, 5000);
//       },
//     });
//   }

//   resetForm(): void {
//     this.form.reset({ disbursementMode: 'manual' });
//     this.selectedFile = null;
//     this.selectedEmployeeIds = [];
//   }

//   closeSuccessToast(): void {
//     this.showSuccessToast = false;
//   }

//   closeErrorToast(): void {
//     this.showErrorToast = false;
//   }
// }











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

  submitDisbursement(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.mode === 'manual' && this.selectedEmployeeIds.length === 0) {
      this.errorMessage = "Please select at least one employee for manual disbursement.";
      this.showErrorToast = true;
      setTimeout(() => (this.showErrorToast = false), 5000);
      return;
    }
    if (this.mode === 'csv' && !this.selectedFile) {
      this.errorMessage = "Please select a CSV file to upload.";
      this.showErrorToast = true;
      setTimeout(() => (this.showErrorToast = false), 5000);
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
        this.successMessage = `Salary disbursement for ${response.totalEmployees} employees (Total: ₹${response.totalAmount.toLocaleString('en-IN')}) has been submitted for approval.`;
        this.showSuccessToast = true;
        this.resetForm();

        setTimeout(() => (this.showSuccessToast = false), 5000);
      },
      error: (err) => {
        console.error('Error creating disbursement', err);
        this.loading = false;

        if (err.error?.message) {
          this.errorMessage = err.error.message;
        } else if (err.status === 400) {
          this.errorMessage = "Invalid data. Please ensure you have selected employees or a valid CSV.";
        } else {
          this.errorMessage = 'An unknown error occurred while submitting the request.';
        }

        this.showErrorToast = true;
        setTimeout(() => (this.showErrorToast = false), 5000);
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
}
