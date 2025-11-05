// import { Component } from '@angular/core';
// import { Payment } from '../../../../core/models/Payment';
// import { SalaryDisbursement } from '../../../../core/models/SalaryDisbursement';
// import { PaymentService } from '../../../client-user/services/payment-service';
// import { SalaryDisbursementService } from '../../../client-user/services/salary-disbursement-service';
// import { forkJoin } from 'rxjs';
// import { CommonModule } from '@angular/common';

// @Component({
//   selector: 'app-pending-approvals-component',
//   imports: [CommonModule],
//   templateUrl: './pending-approvals-component.html',
//   styleUrl: './pending-approvals-component.css',
// })
// export class PendingApprovalsComponent {

//   pendingPayments: Payment[] = [];
//   pendingSalaries: SalaryDisbursement[] = [];

//   loading = true;
//   error: string | null = null;
//   actionLoading: { [key: string]: boolean } = {}; // Tracks loading state for each button

//   constructor(
//     private paymentService: PaymentService,
//     private salaryService: SalaryDisbursementService
//   ) {}

//   ngOnInit(): void {
//     this.loadAllPending();
//   }

//   loadAllPending(): void {
//     this.loading = true;
//     this.error = null;

//     // Use forkJoin to run both API calls in parallel
//     forkJoin({
//       payments: this.paymentService.getPendingPayments(),
//       salaries: this.salaryService.getPendingDisbursements()
//     }).subscribe({
//       next: (results) => {
//         this.pendingPayments = results.payments;
//         this.pendingSalaries = results.salaries;
//         this.loading = false;
//       },
//       error: (err) => {
//         console.error('Error fetching pending approvals', err);
//         this.loading = false;
//         this.error = "Failed to load pending approvals.";
//       }
//     });
//   }

//   // --- Payment Actions ---

//   approvePayment(paymentId: number): void {
//     const key = `pay-${paymentId}`;
//     this.actionLoading[key] = true;
    
//     this.paymentService.approvePayment(paymentId).subscribe({
//       next: () => {
//         // On success, remove the item from the list
//         this.pendingPayments = this.pendingPayments.filter(p => p.transactionId !== paymentId);
//         this.actionLoading[key] = false;
//       },
//       error: (err) => {
//         console.error('Error approving payment', err);
//         alert('Failed to approve payment.');
//         this.actionLoading[key] = false;
//       }
//     });
//   }

//   rejectPayment(paymentId: number): void {
//     const key = `pay-${paymentId}`;
//     this.actionLoading[key] = true;

//     this.paymentService.rejectPayment(paymentId).subscribe({
//       next: () => {
//         // On success, remove the item from the list
//         this.pendingPayments = this.pendingPayments.filter(p => p.transactionId !== paymentId);
//         this.actionLoading[key] = false;
//       },
//       error: (err) => {
//         console.error('Error rejecting payment', err);
//         alert('Failed to reject payment.');
//         this.actionLoading[key] = false;
//       }
//     });
//   }

//   // --- Salary Actions ---

//   approveSalary(disbursementId: number): void {
//     const key = `sal-${disbursementId}`;
//     this.actionLoading[key] = true;

//     this.salaryService.approveDisbursement(disbursementId).subscribe({
//       next: () => {
//         this.pendingSalaries = this.pendingSalaries.filter(s => s.transactionId !== disbursementId);
//         this.actionLoading[key] = false;
//       },
//       error: (err) => {
//         console.error('Error approving salary', err);
//         alert('Failed to approve salary disbursement.');
//         this.actionLoading[key] = false;
//       }
//     });
//   }

//   rejectSalary(disbursementId: number): void {
//     const key = `sal-${disbursementId}`;
//     this.actionLoading[key] = true;
    
//     this.salaryService.rejectDisbursement(disbursementId).subscribe({
//       next: () => {
//         this.pendingSalaries = this.pendingSalaries.filter(s => s.transactionId !== disbursementId);
//         this.actionLoading[key] = false;
//       },
//       error: (err) => {
//         console.error('Error rejecting salary', err);
//         alert('Failed to reject salary disbursement.');
//         this.actionLoading[key] = false;
//       }
//     });
//   }

// }










import { Component } from '@angular/core';
import { Payment } from '../../../../core/models/Payment';
import { SalaryDisbursement } from '../../../../core/models/SalaryDisbursement';
import { PaymentService } from '../../../client-user/services/payment-service';
import { SalaryDisbursementService } from '../../../client-user/services/salary-disbursement-service';
import { forkJoin } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-pending-approvals-component',
  imports: [CommonModule, FormsModule],
  templateUrl: './pending-approvals-component.html',
  styleUrl: './pending-approvals-component.css',
})
export class PendingApprovalsComponent {
  pendingPayments: Payment[] = [];
  pendingSalaries: SalaryDisbursement[] = [];

  loading = true;
  error: string | null = null;
  actionLoading: { [key: string]: boolean } = {};

  // Payment Filters
  paymentSearchTerm = '';
  paymentDateFilter = '';
  filteredPayments: Payment[] = [];
  paymentCurrentPage = 1;
  paymentItemsPerPage = 10;

  // Salary Filters
  salaryDateFilter = '';
  filteredSalaries: SalaryDisbursement[] = [];
  salaryCurrentPage = 1;
  salaryItemsPerPage = 10;

  constructor(
    private paymentService: PaymentService,
    private salaryService: SalaryDisbursementService
  ) {}

  ngOnInit(): void {
    this.loadAllPending();
  }

  loadAllPending(): void {
    this.loading = true;
    this.error = null;

    forkJoin({
      payments: this.paymentService.getPendingPayments(),
      salaries: this.salaryService.getPendingDisbursements()
    }).subscribe({
      next: (results) => {
        this.pendingPayments = results.payments;
        this.pendingSalaries = results.salaries;
        this.filteredPayments = [...results.payments];
        this.filteredSalaries = [...results.salaries];
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching pending approvals', err);
        this.loading = false;
        this.error = "Failed to load pending approvals.";
      }
    });
  }

  // Payment Filtering
  applyPaymentFilters(): void {
    this.filteredPayments = this.pendingPayments.filter(payment => {
      const matchesSearch = !this.paymentSearchTerm || 
        payment.beneficiaryName.toLowerCase().includes(this.paymentSearchTerm.toLowerCase()) ||
        payment.destinationAccountNumber.includes(this.paymentSearchTerm);

      const matchesDate = !this.paymentDateFilter || 
        new Date(payment.createdAt).toDateString() === new Date(this.paymentDateFilter).toDateString();

      return matchesSearch && matchesDate;
    });
    this.paymentCurrentPage = 1;
  }

  // Salary Filtering
  applySalaryFilters(): void {
    this.filteredSalaries = this.pendingSalaries.filter(salary => {
      const matchesDate = !this.salaryDateFilter || 
        new Date(salary.createdAt).toDateString() === new Date(this.salaryDateFilter).toDateString();

      return matchesDate;
    });
    this.salaryCurrentPage = 1;
  }

  // Payment Pagination
  getPaginatedPayments(): Payment[] {
    const start = (this.paymentCurrentPage - 1) * this.paymentItemsPerPage;
    const end = start + this.paymentItemsPerPage;
    return this.filteredPayments.slice(start, end);
  }

  getPaymentStartIndex(): number {
    return (this.paymentCurrentPage - 1) * this.paymentItemsPerPage;
  }

  getPaymentEndIndex(): number {
    return Math.min(this.paymentCurrentPage * this.paymentItemsPerPage, this.filteredPayments.length);
  }

  getPaymentTotalPages(): number {
    return Math.ceil(this.filteredPayments.length / this.paymentItemsPerPage);
  }

  previousPaymentPage(): void {
    if (this.paymentCurrentPage > 1) {
      this.paymentCurrentPage--;
    }
  }

  nextPaymentPage(): void {
    if (this.paymentCurrentPage < this.getPaymentTotalPages()) {
      this.paymentCurrentPage++;
    }
  }

  // Salary Pagination
  getPaginatedSalaries(): SalaryDisbursement[] {
    const start = (this.salaryCurrentPage - 1) * this.salaryItemsPerPage;
    const end = start + this.salaryItemsPerPage;
    return this.filteredSalaries.slice(start, end);
  }

  getSalaryStartIndex(): number {
    return (this.salaryCurrentPage - 1) * this.salaryItemsPerPage;
  }

  getSalaryEndIndex(): number {
    return Math.min(this.salaryCurrentPage * this.salaryItemsPerPage, this.filteredSalaries.length);
  }

  getSalaryTotalPages(): number {
    return Math.ceil(this.filteredSalaries.length / this.salaryItemsPerPage);
  }

  previousSalaryPage(): void {
    if (this.salaryCurrentPage > 1) {
      this.salaryCurrentPage--;
    }
  }

  nextSalaryPage(): void {
    if (this.salaryCurrentPage < this.getSalaryTotalPages()) {
      this.salaryCurrentPage++;
    }
  }

  // --- Payment Actions ---
  approvePayment(paymentId: number): void {
    const key = `pay-${paymentId}`;
    this.actionLoading[key] = true;
    
    this.paymentService.approvePayment(paymentId).subscribe({
      next: () => {
        this.pendingPayments = this.pendingPayments.filter(p => p.transactionId !== paymentId);
        this.applyPaymentFilters();
        this.actionLoading[key] = false;
      },
      error: (err) => {
        console.error('Error approving payment', err);
        alert('Failed to approve payment.');
        this.actionLoading[key] = false;
      }
    });
  }

  rejectPayment(paymentId: number): void {
    const key = `pay-${paymentId}`;
    this.actionLoading[key] = true;

    this.paymentService.rejectPayment(paymentId).subscribe({
      next: () => {
        this.pendingPayments = this.pendingPayments.filter(p => p.transactionId !== paymentId);
        this.applyPaymentFilters();
        this.actionLoading[key] = false;
      },
      error: (err) => {
        console.error('Error rejecting payment', err);
        alert('Failed to reject payment.');
        this.actionLoading[key] = false;
      }
    });
  }

  // --- Salary Actions ---
  approveSalary(disbursementId: number): void {
    const key = `sal-${disbursementId}`;
    this.actionLoading[key] = true;

    this.salaryService.approveDisbursement(disbursementId).subscribe({
      next: () => {
        this.pendingSalaries = this.pendingSalaries.filter(s => s.transactionId !== disbursementId);
        this.applySalaryFilters();
        this.actionLoading[key] = false;
      },
      error: (err) => {
        console.error('Error approving salary', err);
        alert('Failed to approve salary disbursement.');
        this.actionLoading[key] = false;
      }
    });
  }

  rejectSalary(disbursementId: number): void {
    const key = `sal-${disbursementId}`;
    this.actionLoading[key] = true;
    
    this.salaryService.rejectDisbursement(disbursementId).subscribe({
      next: () => {
        this.pendingSalaries = this.pendingSalaries.filter(s => s.transactionId !== disbursementId);
        this.applySalaryFilters();
        this.actionLoading[key] = false;
      },
      error: (err) => {
        console.error('Error rejecting salary', err);
        alert('Failed to reject salary disbursement.');
        this.actionLoading[key] = false;
      }
    });
  }
}
