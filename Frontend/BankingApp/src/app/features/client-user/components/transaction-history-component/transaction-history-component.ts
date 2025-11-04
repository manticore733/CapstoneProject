import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Account } from '../../../../core/models/ReadAccountDto';
import { Payment } from '../../../../core/models/Payment';
import { SalaryDisbursement } from '../../../../core/models/SalaryDisbursement';
import { PaymentService } from '../../services/payment-service';
import { SalaryDisbursementService } from '../../services/salary-disbursement-service';
import { AccountService } from '../../services/account-service';
import { catchError, forkJoin, of } from 'rxjs';

@Component({
  selector: 'app-transaction-history-component',
  imports: [CommonModule],
  templateUrl: './transaction-history-component.html',
  styleUrl: './transaction-history-component.css',
})
export class TransactionHistoryComponent {
  // New property for account details
  account: Account | null = null; 
  
  payments: Payment[] = [];
  salaryDisbursements: SalaryDisbursement[] = [];

  loading = true;
  accountError: string | null = null;
  historyError: string | null = null;

  constructor(
    private paymentService: PaymentService,
    private salaryService: SalaryDisbursementService,
    private accountService: AccountService // Inject new service
  ) {}

  ngOnInit(): void {
    this.loadAllData();
  }

  loadAllData(): void {
    this.loading = true;
    this.accountError = null;
    this.historyError = null;

    // Use forkJoin to fetch all 3 sets of data in parallel
    forkJoin({
      // We use catchError on each one so if one fails, the others still load
      account: this.accountService.getMyAccount().pipe(
        catchError(err => {
          console.error('Account error', err);
          this.accountError = "Your account is not yet approved. Balance is not available.";
          return of(null); // Return null instead of failing the whole request
        })
      ),
      payments: this.paymentService.getMyPayments().pipe(
        catchError(err => {
          console.error('Payment history error', err);
          this.historyError = "Could not load payment history.";
          return of([]); // Return empty array
        })
      ),
      salaryDisbursements: this.salaryService.getMyDisbursements().pipe(
        catchError(err => {
          console.error('Salary history error', err);
          this.historyError = "Could not load salary history.";
          return of([]); // Return empty array
        })
      )
    }).subscribe({
      next: (results) => {
        this.account = results.account;
        this.payments = results.payments;
        this.salaryDisbursements = results.salaryDisbursements;
        this.loading = false;
      }
      // No top-level error needed, since we handle each one individually
    });
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

}
