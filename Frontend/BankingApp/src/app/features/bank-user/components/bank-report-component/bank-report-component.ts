// import { CommonModule } from '@angular/common';
// import { Component } from '@angular/core';
// import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
// import { ClientTransactionReportDto, ReportRecord, ReportResultDto } from '../../../../core/models/ClientUserReportDto';
// import { ReportService } from '../../../../core/services/report-service';
// import { catchError, of } from 'rxjs';

// @Component({
//   selector: 'app-bank-report-component',
//   imports: [CommonModule, ReactiveFormsModule],
//   templateUrl: './bank-report-component.html',
//   styleUrl: './bank-report-component.css',
// })
// export class BankReportComponent {

//   filterForm!: FormGroup;
//   reportData: ClientTransactionReportDto[] = []; // This is the flat list from the API
//   reportHistory: ReportRecord[] = [];

//   loadingReport = false;
//   loadingHistory = false;
//   error: string | null = null;

//   constructor(
//     private fb: FormBuilder,
//     private reportService: ReportService
//   ) {}

//   ngOnInit(): void {
//     // This form includes the 'reportType' dropdown
//     this.filterForm = this.fb.group({
//       startDate: [''],
//       endDate: [''],
//       reportType: ['both'] // Default to 'both'
//     });

//     this.runReport(); // Load default report for all time
//     this.loadHistory(); // Load report history
//   }

//   runReport(): void {
//     this.loadingReport = true;
//     this.error = null;
//     this.reportData = [];

//     const { startDate, endDate, reportType } = this.filterForm.value;

//     this.reportService.getBankUserReport(reportType, startDate || undefined, endDate || undefined).subscribe({
//       next: (data) => {
//         this.reportData = data;
//         this.loadingReport = false;
//       },
//       error: (err) => {
//         console.error('Error fetching bank report', err);
//         this.error = 'Failed to load report data.';
//         this.loadingReport = false;
//       }
//     });
//   }

//   loadHistory(): void {
//     this.loadingHistory = true;
//     this.reportService.getReportHistory().pipe(
//       // This catches the 404 error if history is empty
//       catchError(err => {
//         if (err.status === 404) {
//           return of([]); // Return an empty array
//         }
//         console.error('Error fetching report history', err);
//         return of([]); 
//       })
//     ).subscribe({
//       next: (data) => {
//         // Filter for this user's specific report type
//         this.reportHistory = data.filter(r => r.reportName === 'BankUserReport');
//         this.loadingHistory = false;
//       },
//       error: (err) => {
//         console.error('Unhandled error fetching report history', err);
//         this.loadingHistory = false;
//       }
//     });
//   }

//   exportToExcel(): void {
//     this.loadingReport = true; 
//     this.error = null;
//     const { startDate, endDate, reportType } = this.filterForm.value;

//     this.reportService.generateBankUserReportExcel(reportType, startDate || undefined, endDate || undefined).subscribe({
//       next: (result: ReportResultDto) => {
//         window.open(result.fileUrl, '_blank'); // Open the Cloudinary URL
//         this.loadHistory(); // Refresh the history list
//         this.loadingReport = false;
//       },
//       error: (err) => {
//         console.error('Error generating Excel report', err);
//         this.error = 'Failed to generate Excel report.';
//         this.loadingReport = false;
//       }
//     });
//   }

//   // Helper for status styling
//   getStatusClass(status: string): string {
//     switch (status?.toUpperCase()) {
//       case 'APPROVED':
//         return 'bg-green-100 text-green-800';
//       case 'REJECTED':
//         return 'bg-red-100 text-red-800';
//       case 'PENDING':
//       default:
//         return 'bg-yellow-100 text-yellow-800';
//     }
//   }

// }
























// import { CommonModule } from '@angular/common';
// import { Component } from '@angular/core';
// import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';
// import { ClientTransactionReportDto, ReportRecord, ReportResultDto } from '../../../../core/models/ClientUserReportDto';
// import { ReportService } from '../../../../core/services/report-service';
// import { catchError, of } from 'rxjs';

// @Component({
//   selector: 'app-bank-report-component',
//   imports: [CommonModule, ReactiveFormsModule, FormsModule],
//   templateUrl: './bank-report-component.html',
//   styleUrl: './bank-report-component.css',
// })
// export class BankReportComponent {
//   filterForm!: FormGroup;
//   reportData: ClientTransactionReportDto[] = [];
//   reportHistory: ReportRecord[] = [];

//   loadingReport = false;
//   loadingHistory = false;
//   error: string | null = null;

//   // Filters
//   searchTerm = '';
//   typeFilter = 'all';
//   statusFilter = 'all';
//   filteredData: ClientTransactionReportDto[] = [];
//   currentPage = 1;
//   itemsPerPage = 10;

//   constructor(
//     private fb: FormBuilder,
//     private reportService: ReportService
//   ) {}

//   ngOnInit(): void {
//     this.filterForm = this.fb.group({
//       startDate: [''],
//       endDate: [''],
//       reportType: ['both']
//     });

//     this.runReport();
//     this.loadHistory();
//   }

//   runReport(): void {
//     this.loadingReport = true;
//     this.error = null;
//     this.reportData = [];

//     const { startDate, endDate, reportType } = this.filterForm.value;

//     // Convert to ISO format if dates exist
//     const formattedStartDate = startDate ? new Date(startDate).toISOString() : undefined;
//     const formattedEndDate = endDate ? new Date(endDate).toISOString() : undefined;

//     this.reportService.getBankUserReport(reportType, formattedStartDate, formattedEndDate).subscribe({
//       next: (data) => {
//         this.reportData = data;
//         this.filteredData = [...data];
//         this.loadingReport = false;
//       },
//       error: (err) => {
//         console.error('Error fetching bank report', err);
//         this.error = 'Failed to load report data.';
//         this.loadingReport = false;
//       }
//     });
//   }

//   loadHistory(): void {
//     this.loadingHistory = true;
//     this.reportService.getReportHistory().pipe(
//       catchError(err => {
//         if (err.status === 404) {
//           return of([]);
//         }
//         console.error('Error fetching report history', err);
//         return of([]);
//       })
//     ).subscribe({
//       next: (data) => {
//         this.reportHistory = data.filter(r => r.reportName === 'BankUserReport');
//         this.loadingHistory = false;
//       },
//       error: (err) => {
//         console.error('Unhandled error fetching report history', err);
//         this.loadingHistory = false;
//       }
//     });
//   }

//   exportToExcel(): void {
//     this.loadingReport = true;
//     this.error = null;
//     const { startDate, endDate, reportType } = this.filterForm.value;

//     const formattedStartDate = startDate ? new Date(startDate).toISOString() : undefined;
//     const formattedEndDate = endDate ? new Date(endDate).toISOString() : undefined;

//     this.reportService.generateBankUserReportExcel(reportType, formattedStartDate, formattedEndDate).subscribe({
//       next: (result: ReportResultDto) => {
//         window.open(result.fileUrl, '_blank');
//         this.loadHistory();
//         this.loadingReport = false;
//       },
//       error: (err) => {
//         console.error('Error generating Excel report', err);
//         this.error = 'Failed to generate Excel report.';
//         this.loadingReport = false;
//       }
//     });
//   }

//   // Apply Filters
//   applyFilters(): void {
//   this.filteredData = this.reportData.filter(row => {
//     // Search filter (with null checks)
//     const matchesSearch = !this.searchTerm || 
//       row.clientName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
//       (row.beneficiaryOrEmployee?.toLowerCase().includes(this.searchTerm.toLowerCase()) ?? false);

//     // Type filter
//     const matchesType = this.typeFilter === 'all' || 
//       row.type === this.typeFilter;

//     // Status filter
//     const matchesStatus = this.statusFilter === 'all' || 
//       row.status === this.statusFilter;

//     return matchesSearch && matchesType && matchesStatus;
//   });
//   this.currentPage = 1; // Reset to first page
// }

//   // Pagination
//   getPaginatedData(): ClientTransactionReportDto[] {
//     const start = (this.currentPage - 1) * this.itemsPerPage;
//     const end = start + this.itemsPerPage;
//     return this.filteredData.slice(start, end);
//   }

//   getStartIndex(): number {
//     return (this.currentPage - 1) * this.itemsPerPage;
//   }

//   getEndIndex(): number {
//     return Math.min(this.currentPage * this.itemsPerPage, this.filteredData.length);
//   }

//   getTotalPages(): number {
//     return Math.ceil(this.filteredData.length / this.itemsPerPage);
//   }

//   previousPage(): void {
//     if (this.currentPage > 1) {
//       this.currentPage--;
//     }
//   }

//   nextPage(): void {
//     if (this.currentPage < this.getTotalPages()) {
//       this.currentPage++;
//     }
//   }

//   // Helper for status styling
//   getStatusClass(status: string): string {
//     switch (status?.toUpperCase()) {
//       case 'APPROVED':
//         return 'bg-green-100 text-green-800';
//       case 'REJECTED':
//         return 'bg-red-100 text-red-800';
//       case 'PENDING':
//       default:
//         return 'bg-yellow-100 text-yellow-800';
//     }
//   }
// }



import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ClientTransactionReportDto, ReportRecord, ReportResultDto } from '../../../../core/models/ClientUserReportDto';
import { ReportService } from '../../../../core/services/report-service';
import { catchError, of } from 'rxjs';

@Component({
  selector: 'app-bank-report-component',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './bank-report-component.html',
  styleUrl: './bank-report-component.css',
})
export class BankReportComponent {
  filterForm!: FormGroup;
  reportData: ClientTransactionReportDto[] = [];
  reportHistory: ReportRecord[] = [];

  loadingReport = false;
  loadingHistory = false;
  error: string | null = null;

  // Transaction Table Filters
  searchTerm = '';
  typeFilter = 'all';
  statusFilter = 'all';
  filteredData: ClientTransactionReportDto[] = [];
  currentPage = 1;
  itemsPerPage = 10;

  // Report History Filters
  historySearchTerm = '';
  filteredHistory: ReportRecord[] = [];
  historyCurrentPage = 1;
  historyItemsPerPage = 10;

  constructor(
    private fb: FormBuilder,
    private reportService: ReportService
  ) {}

  ngOnInit(): void {
    this.filterForm = this.fb.group({
      startDate: [''],
      endDate: [''],
      reportType: ['both']
    });

    this.runReport();
    this.loadHistory();
  }

  runReport(): void {
    this.loadingReport = true;
    this.error = null;
    this.reportData = [];

    const { startDate, endDate, reportType } = this.filterForm.value;

    const formattedStartDate = startDate ? new Date(startDate).toISOString() : undefined;
    const formattedEndDate = endDate ? new Date(endDate).toISOString() : undefined;

    this.reportService.getBankUserReport(reportType, formattedStartDate, formattedEndDate).subscribe({
      next: (data) => {
        this.reportData = data;
        this.filteredData = [...data];
        this.loadingReport = false;
      },
      error: (err) => {
        console.error('Error fetching bank report', err);
        this.error = 'Failed to load report data.';
        this.loadingReport = false;
      }
    });
  }

  loadHistory(): void {
    this.loadingHistory = true;
    this.reportService.getReportHistory().pipe(
      catchError(err => {
        if (err.status === 404) {
          return of([]);
        }
        console.error('Error fetching report history', err);
        return of([]);
      })
    ).subscribe({
      next: (data) => {
        this.reportHistory = data.filter(r => r.reportName === 'BankUserReport');
        this.filteredHistory = [...this.reportHistory];
        this.loadingHistory = false;
      },
      error: (err) => {
        console.error('Unhandled error fetching report history', err);
        this.loadingHistory = false;
      }
    });
  }

  exportToExcel(): void {
    this.loadingReport = true;
    this.error = null;
    const { startDate, endDate, reportType } = this.filterForm.value;

    const formattedStartDate = startDate ? new Date(startDate).toISOString() : undefined;
    const formattedEndDate = endDate ? new Date(endDate).toISOString() : undefined;

    this.reportService.generateBankUserReportExcel(reportType, formattedStartDate, formattedEndDate).subscribe({
      next: (result: ReportResultDto) => {
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

  // Transaction Table Filters
  applyFilters(): void {
    this.filteredData = this.reportData.filter(row => {
      const matchesSearch = !this.searchTerm || 
        row.clientName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        (row.beneficiaryOrEmployee?.toLowerCase().includes(this.searchTerm.toLowerCase()) ?? false);

      const matchesType = this.typeFilter === 'all' || 
        row.type === this.typeFilter;

      const matchesStatus = this.statusFilter === 'all' || 
        row.status === this.statusFilter;

      return matchesSearch && matchesType && matchesStatus;
    });
    this.currentPage = 1;
  }

  // Transaction Table Pagination
  getPaginatedData(): ClientTransactionReportDto[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.filteredData.slice(start, end);
  }

  getStartIndex(): number {
    return (this.currentPage - 1) * this.itemsPerPage;
  }

  getEndIndex(): number {
    return Math.min(this.currentPage * this.itemsPerPage, this.filteredData.length);
  }

  getTotalPages(): number {
    return Math.ceil(this.filteredData.length / this.itemsPerPage);
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  nextPage(): void {
    if (this.currentPage < this.getTotalPages()) {
      this.currentPage++;
    }
  }

  // Report History Filters
  applyHistoryFilters(): void {
    this.filteredHistory = this.reportHistory.filter(report => {
      return !this.historySearchTerm || 
        report.fileName.toLowerCase().includes(this.historySearchTerm.toLowerCase());
    });
    this.historyCurrentPage = 1;
  }

  // Report History Pagination
  getPaginatedHistory(): ReportRecord[] {
    const start = (this.historyCurrentPage - 1) * this.historyItemsPerPage;
    const end = start + this.historyItemsPerPage;
    return this.filteredHistory.slice(start, end);
  }

  getHistoryStartIndex(): number {
    return (this.historyCurrentPage - 1) * this.historyItemsPerPage;
  }

  getHistoryEndIndex(): number {
    return Math.min(this.historyCurrentPage * this.historyItemsPerPage, this.filteredHistory.length);
  }

  getHistoryTotalPages(): number {
    return Math.ceil(this.filteredHistory.length / this.historyItemsPerPage);
  }

  previousHistoryPage(): void {
    if (this.historyCurrentPage > 1) {
      this.historyCurrentPage--;
    }
  }

  nextHistoryPage(): void {
    if (this.historyCurrentPage < this.getHistoryTotalPages()) {
      this.historyCurrentPage++;
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
