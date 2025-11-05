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
//   employees: Employee[] = []; // For the multi-select
//   selectedFile: File | null = null;
  
//   loading = false;
//   loadingEmployees = false;
//   error: string | null = null;
//   success: string | null = null;

//   constructor(
//     private fb: FormBuilder,
//     private employeeService: EmployeeService,
//     private salaryService: SalaryDisbursementService
//   ) {}

//   ngOnInit(): void {
//     this.form = this.fb.group({
//       // This control manages the form's state
//       disbursementMode: ['manual', [Validators.required]], 
      
//       // Inputs for each mode
//       selectedEmployeeIds: [[]], // For 'manual'
//       remarks: ['', [Validators.maxLength(200)]],
//       // 'allEmployees' and 'csvFile' are handled by component properties
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
//         // This is the check for pending/inactive users
//         this.error = "Your account must be approved to manage employees and disburse salaries.";
//         this.loadingEmployees = false;
//         this.form.disable(); // Disable the form
//       },
//     });
//   }

//   // Helper to get the current mode
//   get mode() {
//     return this.form.value.disbursementMode;
//   }

//   // Store the selected file
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

//     // --- Validation for each mode ---
//     if (this.mode === 'manual' && this.form.value.selectedEmployeeIds.length === 0) {
//       this.error = "Please select at least one employee for manual disbursement.";
//       return;
//     }
//     if (this.mode === 'csv' && !this.selectedFile) {
//       this.error = "Please select a CSV file to upload.";
//       return;
//     }

//     this.loading = true;
//     this.error = null;
//     this.success = null;

//     // --- Build the FormData object ---
//     const formData = new FormData();
//     formData.append('Remarks', this.form.value.remarks || '');

//     if (this.mode === 'all') {
//       formData.append('AllEmployees', 'true');
//     } 
//     else if (this.mode === 'csv') {
//       formData.append('CsvFile', this.selectedFile!);
//     } 
//     else { // 'manual'
//       const employeeIds: number[] = this.form.value.selectedEmployeeIds;
//       // The backend DTO can take a list for 'EmployeeIds'
//       employeeIds.forEach(id => {
//         formData.append('EmployeeIds', id.toString());
//       });
//     }

//     // --- Send the FormData to the service ---
//     this.salaryService.createDisbursement(formData).subscribe({
//       next: (response) => {
//         this.loading = false;
//         this.success = `Salary disbursement for ${response.totalEmployees} employees (Total: ${response.totalAmount}) has been submitted for approval.`;
//         this.form.reset({ disbursementMode: 'manual' });
//         this.selectedFile = null;
//       },
//       error: (err) => {
//         console.error('Error creating disbursement', err);
//         this.loading = false;
//         if (err.error?.message) {
//           this.error = err.error.message;
//         } else if (err.status === 400) {
//           // Catch DTO validation errors from the backend
//           this.error = "Invalid data. Please ensure you have selected employees or a valid CSV.";
//         } else {
//           this.error = 'An unknown error occurred while submitting the request.';
//         }
//       },
//     });
//   }

// }

















import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Employee } from '../../../../core/models/Employee';
import { EmployeeService } from '../../services/employee-service';
import { SalaryDisbursementService } from '../../services/salary-disbursement-service';

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
  
  // Toast notifications
  showSuccessToast = false;
  showErrorToast = false;
  successMessage = '';
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService,
    private salaryService: SalaryDisbursementService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      disbursementMode: ['manual', [Validators.required]], 
      selectedEmployeeIds: [[]],
      remarks: ['', [Validators.maxLength(200)]],
    });

    this.loadEmployees();
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

    // Validation
    if (this.mode === 'manual' && this.selectedEmployeeIds.length === 0) {
      this.errorMessage = "Please select at least one employee for manual disbursement.";
      this.showErrorToast = true;
      setTimeout(() => this.showErrorToast = false, 5000);
      return;
    }
    if (this.mode === 'csv' && !this.selectedFile) {
      this.errorMessage = "Please select a CSV file to upload.";
      this.showErrorToast = true;
      setTimeout(() => this.showErrorToast = false, 5000);
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
        
        setTimeout(() => this.showSuccessToast = false, 5000);
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
        setTimeout(() => this.showErrorToast = false, 5000);
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
