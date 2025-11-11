import { Component } from '@angular/core';
import { Payment } from '../../../../core/models/Payment';
import { SalaryDisbursement } from '../../../../core/models/SalaryDisbursement';
import { PaymentService } from '../../../client-user/services/payment-service';
import { SalaryDisbursementService } from '../../../client-user/services/salary-disbursement-service';
import { forkJoin } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface Toast {
  id: string;
  message: string;
  type: 'success' | 'warning' | 'error';
}

@Component({
  selector: 'app-pending-approvals-component',
  imports: [CommonModule, FormsModule],
  templateUrl: './pending-approvals-component.html',
  styleUrl: './pending-approvals-component.css',
  styles: [`
    @keyframes slideIn {
      from {
        transform: translateX(400px);
        opacity: 0;
      }
      to {
        transform: translateX(0);
        opacity: 1;
      }
    }

    .animate-slide-in {
      animation: slideIn 0.3s ease-out;
    }
  `]
})
export class PendingApprovalsComponent {
  pendingPayments: Payment[] = [];
  pendingSalaries: SalaryDisbursement[] = [];

  loading = true;
  error: string | null = null;
  actionLoading: { [key: string]: boolean } = {};

  // Toast notifications
  toasts: Toast[] = [];

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

  // --- Reject Modal Logic ---
showRejectModal = false;
rejectRemark = '';
rejectContext: { type: 'payment' | 'salary'; id: number } | null = null;

openRejectModal(type: 'payment' | 'salary', id: number): void {
  this.rejectContext = { type, id };
  this.rejectRemark = '';
  this.showRejectModal = true;
}

closeRejectModal(): void {
  this.showRejectModal = false;
  this.rejectContext = null;
  this.rejectRemark = '';
}

confirmRejection(): void {
  if (!this.rejectContext) return;

  const { type, id } = this.rejectContext;
  const remark = this.rejectRemark.trim();

  this.showRejectModal = false;

  if (type === 'payment') {
    this.rejectPaymentWithRemark(id, remark);
  } else if (type === 'salary') {
    this.rejectSalaryWithRemark(id, remark);
  }
}


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
        this.showToast('Failed to load pending approvals.', 'error');
      }
    });
  }

  // Toast methods
  showToast(message: string, type: 'success' | 'warning' | 'error', duration: number = 3000): void {
    const id = Math.random().toString(36).substr(2, 9);
    const toast: Toast = { id, message, type };

    this.toasts.push(toast);

    setTimeout(() => {
      this.removeToast(id);
    }, duration);
  }

  removeToast(id: string): void {
    this.toasts = this.toasts.filter(t => t.id !== id);
  }

  getToastClass(type: string): string {
    switch (type) {
      case 'success':
        return 'bg-green-50 border border-green-200 text-green-800';
      case 'warning':
        return 'bg-yellow-50 border border-yellow-200 text-yellow-800';
      case 'error':
        return 'bg-red-50 border border-red-200 text-red-800';
      default:
        return 'bg-gray-50 border border-gray-200 text-gray-800';
    }
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
    
    const payment = this.pendingPayments.find(p => p.transactionId === paymentId);
    
    this.paymentService.approvePayment(paymentId).subscribe({
      next: () => {
        const message = payment 
          ? `✓ Payment to ${payment.beneficiaryName} (₹${payment.amount}) approved successfully!`
          : '✓ Payment approved successfully!';
        
        this.showToast(message, 'success', 3000);
        this.pendingPayments = this.pendingPayments.filter(p => p.transactionId !== paymentId);
        this.applyPaymentFilters();
        this.actionLoading[key] = false;
      },
      error: (err) => {
        console.error('Error approving payment', err);
        this.showToast('Failed to approve payment. Please try again.', 'error', 4000);
        this.actionLoading[key] = false;
      }
    });
  }


  rejectPaymentWithRemark(paymentId: number, remark: string): void {
    const key = `pay-${paymentId}`;
    this.actionLoading[key] = true;

    const payment = this.pendingPayments.find(p => p.transactionId === paymentId);

    this.paymentService.rejectPayment(paymentId, { bankRemark: remark }).subscribe({
      next: () => {
        const message = payment
          ? `✗ Payment to ${payment.beneficiaryName} (₹${payment.amount}) rejected — ${remark}`
          : `✗ Payment rejected — ${remark}`;

        this.showToast(message, 'warning', 3000);
        this.pendingPayments = this.pendingPayments.filter(p => p.transactionId !== paymentId);
        this.applyPaymentFilters();
        this.actionLoading[key] = false;
      },
      error: (err) => {
        console.error('Error rejecting payment', err);
        this.showToast('Failed to reject payment. Please try again.', 'error', 4000);
        this.actionLoading[key] = false;
      }
    });
  }

  
  // --- Salary Actions ---
  approveSalary(disbursementId: number): void {
    const key = `sal-${disbursementId}`;
    this.actionLoading[key] = true;

    const salary = this.pendingSalaries.find(s => s.transactionId === disbursementId);

    this.salaryService.approveDisbursement(disbursementId).subscribe({
      next: () => {
        const message = salary 
          ? `✓ Salary disbursement for ${salary.totalEmployees} employees (₹${salary.totalAmount}) approved successfully!`
          : '✓ Salary disbursement approved successfully!';
        
        this.showToast(message, 'success', 3000);
        this.pendingSalaries = this.pendingSalaries.filter(s => s.transactionId !== disbursementId);
        this.applySalaryFilters();
        this.actionLoading[key] = false;
      },
      error: (err) => {
        console.error('Error approving salary', err);
        this.showToast('Failed to approve salary disbursement. Please try again.', 'error', 4000);
        this.actionLoading[key] = false;
      }
    });
  }

  rejectSalaryWithRemark(disbursementId: number, remark: string): void {
    const key = `sal-${disbursementId}`;
    this.actionLoading[key] = true;

    const salary = this.pendingSalaries.find(s => s.transactionId === disbursementId);

    this.salaryService.rejectDisbursement(disbursementId, { bankRemark: remark }).subscribe({
      next: () => {
        const message = salary
          ? `✗ Salary disbursement for ${salary.totalEmployees} employees (₹${salary.totalAmount}) rejected — ${remark}`
          : `✗ Salary disbursement rejected — ${remark}`;

        this.showToast(message, 'warning', 3000);
        this.pendingSalaries = this.pendingSalaries.filter(s => s.transactionId !== disbursementId);
        this.applySalaryFilters();
        this.actionLoading[key] = false;
      },
      error: (err) => {
        console.error('Error rejecting salary', err);
        this.showToast('Failed to reject salary disbursement. Please try again.', 'error', 4000);
        this.actionLoading[key] = false;
      }
    });
  }
}
