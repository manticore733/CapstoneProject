

import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Account } from '../../../../core/models/ReadAccountDto';
import { Payment } from '../../../../core/models/Payment';
import { SalaryDisbursement } from '../../../../core/models/SalaryDisbursement';
import { PaymentService } from '../../services/payment-service';
import { SalaryDisbursementService } from '../../services/salary-disbursement-service';
import { AccountService } from '../../services/account-service';
import { catchError, forkJoin, of } from 'rxjs';

@Component({
  selector: 'app-transaction-history-component',
  imports: [CommonModule, FormsModule],
  templateUrl: './transaction-history-component.html',
  styleUrl: './transaction-history-component.css',
})
export class TransactionHistoryComponent {
  account: Account | null = null;
  payments: Payment[] = [];
  salaryDisbursements: SalaryDisbursement[] = [];

  loading = true;
  accountError: string | null = null;
  historyError: string | null = null;

  // Payment Filters
  paymentSearchTerm = '';
  paymentStatusFilter = 'all';
  paymentDateFilter = '';
  filteredPayments: Payment[] = [];
  paymentCurrentPage = 1;
  paymentItemsPerPage = 5;

  // Salary Filters
  salaryStatusFilter = 'all';
  salaryDateFilter = '';
  filteredSalaryDisbursements: SalaryDisbursement[] = [];
  salaryCurrentPage = 1;
  salaryItemsPerPage = 5;

  constructor(
    private paymentService: PaymentService,
    private salaryService: SalaryDisbursementService,
    private accountService: AccountService
  ) {}

  ngOnInit(): void {
    this.loadAllData();
  }

  loadAllData(): void {
    this.loading = true;
    this.accountError = null;
    this.historyError = null;

    forkJoin({
      account: this.accountService.getMyAccount().pipe(
        catchError(err => {
          console.error('Account error', err);
          this.accountError = "Your account is not yet approved. Balance is not available.";
          return of(null);
        })
      ),
      payments: this.paymentService.getMyPayments().pipe(
        catchError(err => {
          console.error('Payment history error', err);
          this.historyError = "Could not load payment history.";
          return of([]);
        })
      ),
      salaryDisbursements: this.salaryService.getMyDisbursements().pipe(
        catchError(err => {
          console.error('Salary history error', err);
          this.historyError = "Could not load salary history.";
          return of([]);
        })
      )
    }).subscribe({
      next: (results) => {
        this.account = results.account;
        this.payments = results.payments;
        this.salaryDisbursements = results.salaryDisbursements;
        this.filteredPayments = [...this.payments];
        this.filteredSalaryDisbursements = [...this.salaryDisbursements];
        this.loading = false;
      }
    });
  }

  // Payment Filtering
  applyPaymentFilters(): void {
    this.filteredPayments = this.payments.filter(payment => {
      // Search filter
      const matchesSearch = !this.paymentSearchTerm || 
        payment.beneficiaryName.toLowerCase().includes(this.paymentSearchTerm.toLowerCase()) ||
        payment.destinationAccountNumber.includes(this.paymentSearchTerm);

      // Status filter
      const matchesStatus = this.paymentStatusFilter === 'all' || 
        payment.transactionStatus === this.paymentStatusFilter;

      // Date filter
      const matchesDate = !this.paymentDateFilter || 
        new Date(payment.createdAt).toDateString() === new Date(this.paymentDateFilter).toDateString();

      return matchesSearch && matchesStatus && matchesDate;
    });
    this.paymentCurrentPage = 1; // Reset to first page
  }

  // Salary Filtering
  applySalaryFilters(): void {
    this.filteredSalaryDisbursements = this.salaryDisbursements.filter(salary => {
      // Status filter
      const matchesStatus = this.salaryStatusFilter === 'all' || 
        salary.transactionStatus === this.salaryStatusFilter;

      // Date filter
      const matchesDate = !this.salaryDateFilter || 
        new Date(salary.createdAt).toDateString() === new Date(this.salaryDateFilter).toDateString();

      return matchesStatus && matchesDate;
    });
    this.salaryCurrentPage = 1; // Reset to first page
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
  getPaginatedSalary(): SalaryDisbursement[] {
    const start = (this.salaryCurrentPage - 1) * this.salaryItemsPerPage;
    const end = start + this.salaryItemsPerPage;
    return this.filteredSalaryDisbursements.slice(start, end);
  }

  getSalaryStartIndex(): number {
    return (this.salaryCurrentPage - 1) * this.salaryItemsPerPage;
  }

  getSalaryEndIndex(): number {
    return Math.min(this.salaryCurrentPage * this.salaryItemsPerPage, this.filteredSalaryDisbursements.length);
  }

  getSalaryTotalPages(): number {
    return Math.ceil(this.filteredSalaryDisbursements.length / this.salaryItemsPerPage);
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

  // Helper function for styling the status badge
  getStatusClass(status: string): string {
    switch (status?.toUpperCase()) {
      case 'APPROVED':
        return 'bg-green-100 text-green-800';
      case 'REJECTED':
        return 'bg-red-100 text-red-800';
      case 'PENDING':
      default:
        return 'bg-yellow-100 text-yellow-800';
    }
  }

  getCurrentDate(): Date {
    return new Date();
  }
}
