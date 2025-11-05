// import { Component } from '@angular/core';
// import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
// import { Beneficiary } from '../../../../core/models/Beneficiary';
// import { BeneficiaryService } from '../../services/beneficiary-service';
// import { PaymentService } from '../../services/payment-service';
// import { Router } from '@angular/router';
// import { CreatePaymentDto } from '../../../../core/models/CreatePaymentDto';
// import { CommonModule } from '@angular/common';

// @Component({
//   selector: 'app-make-payment-component',
//   imports: [CommonModule, ReactiveFormsModule],
//   templateUrl: './make-payment-component.html',
//   styleUrl: './make-payment-component.css',
// })
// export class MakePaymentComponent {

//   form!: FormGroup;
//   beneficiaries: Beneficiary[] = [];
  
//   loading = false;
//   error: string | null = null;
//   success: string | null = null;

//   constructor(
//     private fb: FormBuilder,
//     private beneficiaryService: BeneficiaryService,
//     private paymentService: PaymentService,
//     private router: Router
//   ) {}

//   ngOnInit(): void {
//     this.form = this.fb.group({
//       beneficiaryId: [null, [Validators.required]],
//       amount: [null, [Validators.required, Validators.min(1)]],
//       remarks: ['', [Validators.maxLength(200)]],
//     });

//     this.loadBeneficiaries();
//   }

//   loadBeneficiaries(): void {
//     this.loading = true;
//     this.beneficiaryService.getMyBeneficiaries().subscribe({
//       next: (data) => {
//         this.beneficiaries = data;
//         this.loading = false;
//       },
//       error: (err) => {
//         console.error('Error fetching beneficiaries', err);
//         // This is the check for pending/inactive users
//         this.error = "Your account must be approved to make payments. Please upload your documents.";
//         this.loading = false;
//         this.form.disable(); // Disable the form if they can't pay
//       },
//     });
//   }

//   submitPayment(): void {
//     if (this.form.invalid) {
//       this.form.markAllAsTouched();
//       return;
//     }

//     this.loading = true;
//     this.error = null;
//     this.success = null;

//     const dto: CreatePaymentDto = {
//       beneficiaryId: +this.form.value.beneficiaryId, // Ensure it's a number
//       amount: +this.form.value.amount,
//       remarks: this.form.value.remarks,
//     };

//     this.paymentService.createPayment(dto).subscribe({
//       next: (response) => {
//         this.loading = false;
//         this.success = `Payment of ${response.amount} to ${response.beneficiaryName} submitted for approval.`;
//         this.form.reset();
//         // Optionally, navigate to a transaction history page
//         // this.router.navigate(['/client/dashboard/payment-history']);
//       },
//       error: (err) => {
//         console.error('Error creating payment', err);
//         this.loading = false;
//         if (err.error?.message) {
//           this.error = err.error.message;
//         } else {
//           this.error = 'An unknown error occurred while submitting the payment.';
//         }
//       },
//     });
//   }

// }










import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Beneficiary } from '../../../../core/models/Beneficiary';
import { BeneficiaryService } from '../../services/beneficiary-service';
import { PaymentService } from '../../services/payment-service';
import { Router } from '@angular/router';
import { CreatePaymentDto } from '../../../../core/models/CreatePaymentDto';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-make-payment-component',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './make-payment-component.html',
  styleUrl: './make-payment-component.css',
})
export class MakePaymentComponent {
  form!: FormGroup;
  beneficiaries: Beneficiary[] = [];
  
  loading = false;
  error: string | null = null;
  
  // Toast notification states
  showSuccessToast = false;
  showErrorToast = false;
  successMessage = '';
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private beneficiaryService: BeneficiaryService,
    private paymentService: PaymentService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      beneficiaryId: [null, [Validators.required]],
      amount: [null, [Validators.required, Validators.min(1)]],
      remarks: ['', [Validators.maxLength(200)]],
    });

    this.loadBeneficiaries();
  }

  loadBeneficiaries(): void {
    this.loading = true;
    this.beneficiaryService.getMyBeneficiaries().subscribe({
      next: (data) => {
        this.beneficiaries = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching beneficiaries', err);
        this.error = "Your account must be approved to make payments. Please upload your documents.";
        this.loading = false;
        this.form.disable();
      },
    });
  }

  submitPayment(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;

    const dto: CreatePaymentDto = {
      beneficiaryId: +this.form.value.beneficiaryId,
      amount: +this.form.value.amount,
      remarks: this.form.value.remarks,
    };

    this.paymentService.createPayment(dto).subscribe({
      next: (response) => {
        this.loading = false;
        this.successMessage = `Payment of ₹${response.amount.toLocaleString('en-IN')} to ${response.beneficiaryName} submitted for approval.`;
        this.showSuccessToast = true;
        this.form.reset();
        
        // Auto-hide success toast after 5 seconds
        setTimeout(() => {
          this.showSuccessToast = false;
        }, 5000);
      },
      error: (err) => {
        console.error('Error creating payment', err);
        this.loading = false;
        
        if (err.error?.message) {
          this.errorMessage = err.error.message;
        } else {
          this.errorMessage = 'An unknown error occurred while submitting the payment.';
        }
        
        this.showErrorToast = true;
        
        // Auto-hide error toast after 5 seconds
        setTimeout(() => {
          this.showErrorToast = false;
        }, 5000);
      },
    });
  }

  resetForm(): void {
    this.form.reset();
  }

  closeSuccessToast(): void {
    this.showSuccessToast = false;
  }

  closeErrorToast(): void {
    this.showErrorToast = false;
  }
}
