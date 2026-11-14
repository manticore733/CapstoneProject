import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ClientUserReportDto, ReportRecord } from '../../../../core/models/ClientUserReportDto';
import { ReportService } from '../../../../core/services/report-service';
import { CommonModule } from '@angular/common';
import { catchError, of } from 'rxjs';

@Component({
  selector: 'app-client-report-component',
  standalone: true, 
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './client-report-component.html',
  styleUrl: './client-report-component.css',
})
export class ClientReportComponent implements OnInit { 
  filterForm!: FormGroup;
  reportData: ClientUserReportDto | null = null;
  reportHistory: ReportRecord[] = [];

  loadingReport = false;
  loadingHistory = false;
  error: string | null = null;

  // Payment Filters
  paymentSearchTerm = '';
  paymentStatusFilter = 'all';
  filteredPayments: any[] = [];
  paymentCurrentPage = 1;
  paymentItemsPerPage = 10;

  // Salary Filters
  salaryStatusFilter = 'all';
  filteredSalary: any[] = [];
  salaryCurrentPage = 1;
  salaryItemsPerPage = 10;

  constructor(
    private fb: FormBuilder,
    private reportService: ReportService
  ) {}

  ngOnInit(): void {
    this.filterForm = this.fb.group({
      startDate: [''],
      endDate: ['']
    });

    console.log('Component initialized. Running default report...');
    this.runReport();
    this.loadHistory();
  }

  runReport(): void {
    console.log('--- runReport() function CALLED. ---');

    this.loadingReport = true;
    this.error = null;
    this.reportData = null;

    const { startDate, endDate } = this.filterForm.value;
    
    console.log('Filters being sent to API:', { startDate, endDate });

    // Convert to ISO format if dates exist
    const formattedStartDate = startDate ? new Date(startDate).toISOString() : undefined;
    const formattedEndDate = endDate ? new Date(endDate).toISOString() : undefined;

    console.log('Calling reportService.getClientReport...');

    this.reportService.getClientReport(formattedStartDate, formattedEndDate).subscribe({
      next: (data) => {
        console.log('API call successful. Data received:', data);
        this.reportData = data;
        this.filteredPayments = [...(data.payments || [])];
        this.filteredSalary = [...(data.salaryDisbursements || [])];
        this.loadingReport = false;
      },
      error: (err) => {
        console.error('API call FAILED.', err);
        this.error = 'Failed to load report data. Your account may be pending approval.';
        this.loadingReport = false;
      }
    });
  }

  loadHistory(): void {
    console.log('loadHistory() CALLED.');
    this.loadingHistory = true;
    this.reportService.getReportHistory().pipe(
      catchError(err => {
        if (err.status === 404) {
          console.log('History API returned 404 (No history found). This is OK.');
          return of([]);
        }
        console.error('Error fetching report history', err);
        return of([]);
      })
    ).subscribe({
      next: (data) => {
        console.log('History API successful. Data received:', data);
        this.reportHistory = data.filter(r => r.reportName === 'ClientUserReport');
        this.loadingHistory = false;
      },
      error: (err) => {
        console.error('Unhandled error fetching report history', err);
        this.loadingHistory = false;
      }
    });
  }

  exportToExcel(): void {
    console.log('--- exportToExcel() function CALLED. ---');
    
    this.loadingReport = true;
    this.error = null;
    const { startDate, endDate } = this.filterForm.value;

    const formattedStartDate = startDate ? new Date(startDate).toISOString() : undefined;
    const formattedEndDate = endDate ? new Date(endDate).toISOString() : undefined;

    console.log('Filters being sent to API for EXCEL:', { formattedStartDate, formattedEndDate });

    this.reportService.generateClientReportExcel(formattedStartDate, formattedEndDate).subscribe({
      next: (result) => {
        console.log('Excel API successful. Result:', result);
        window.open(result.fileUrl, '_blank');
        this.loadHistory();
        this.loadingReport = false;
      },
      error: (err) => {
        console.error('Error generating Excel report', err);
        this.error = 'Failed to generate Excel report.';
        this.loadingReport = false;
      }
    });
  }

  // Payment Filtering
  applyPaymentFilters(): void {
    if (!this.reportData) return;

    this.filteredPayments = this.reportData.payments.filter(payment => {
      const matchesSearch = !this.paymentSearchTerm || 
        payment.beneficiaryName.toLowerCase().includes(this.paymentSearchTerm.toLowerCase());

      const matchesStatus = this.paymentStatusFilter === 'all' || 
        payment.status === this.paymentStatusFilter;

      return matchesSearch && matchesStatus;
    });
    this.paymentCurrentPage = 1;
  }

  // Salary Filtering
  applySalaryFilters(): void {
    if (!this.reportData) return;

    this.filteredSalary = this.reportData.salaryDisbursements.filter(salary => {
      const matchesStatus = this.salaryStatusFilter === 'all' || 
        salary.status === this.salaryStatusFilter;

      return matchesStatus;
    });
    this.salaryCurrentPage = 1;
  }

  // Payment Pagination
  getPaginatedPayments(): any[] {
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
  getPaginatedSalary(): any[] {
    const start = (this.salaryCurrentPage - 1) * this.salaryItemsPerPage;
    const end = start + this.salaryItemsPerPage;
    return this.filteredSalary.slice(start, end);
  }

  getSalaryStartIndex(): number {
    return (this.salaryCurrentPage - 1) * this.salaryItemsPerPage;
  }

  getSalaryEndIndex(): number {
    return Math.min(this.salaryCurrentPage * this.salaryItemsPerPage, this.filteredSalary.length);
  }

  getSalaryTotalPages(): number {
    return Math.ceil(this.filteredSalary.length / this.salaryItemsPerPage);
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

  // Helper for status styling
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
