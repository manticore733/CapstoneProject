import { Component } from '@angular/core';
import { Payment } from '../../../../core/models/Payment';
import { SalaryDisbursement } from '../../../../core/models/SalaryDisbursement';
import { PaymentService } from '../../../client-user/services/payment-service';
import { SalaryDisbursementService } from '../../../client-user/services/salary-disbursement-service';
import { forkJoin } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pending-approvals-component',
  imports: [CommonModule],
  templateUrl: './pending-approvals-component.html',
  styleUrl: './pending-approvals-component.css',
})
export class PendingApprovalsComponent {

  pendingPayments: Payment[] = [];
  pendingSalaries: SalaryDisbursement[] = [];

  loading = true;
  error: string | null = null;
  actionLoading: { [key: string]: boolean } = {}; // Tracks loading state for each button

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

    // Use forkJoin to run both API calls in parallel
    forkJoin({
      payments: this.paymentService.getPendingPayments(),
      salaries: this.salaryService.getPendingDisbursements()
    }).subscribe({
      next: (results) => {
        this.pendingPayments = results.payments;
        this.pendingSalaries = results.salaries;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching pending approvals', err);
        this.loading = false;
        this.error = "Failed to load pending approvals.";
      }
    });
  }

  // --- Payment Actions ---

  approvePayment(paymentId: number): void {
    const key = `pay-${paymentId}`;
    this.actionLoading[key] = true;
    
    this.paymentService.approvePayment(paymentId).subscribe({
      next: () => {
        // On success, remove the item from the list
        this.pendingPayments = this.pendingPayments.filter(p => p.transactionId !== paymentId);
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
        // On success, remove the item from the list
        this.pendingPayments = this.pendingPayments.filter(p => p.transactionId !== paymentId);
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
