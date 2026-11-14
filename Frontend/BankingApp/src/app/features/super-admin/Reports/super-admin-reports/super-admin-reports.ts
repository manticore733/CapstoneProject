import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../../core/services/api-service';

@Component({
  selector: 'app-super-admin-reports',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './super-admin-reports.html',
  styleUrl: './super-admin-reports.css',
})
export class SuperAdminReports implements OnInit {
  isLoading = false;
  systemSummary: any = null;
  reportHistory: any[] = [];
  isExporting = false;

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.fetchSystemSummary();
    this.fetchReportHistory();
  }

  fetchSystemSummary() {
    this.isLoading = true;
    this.api.get<any>('Reports/system-summary').subscribe({
      next: (res) => {
        console.log(res);
        this.systemSummary = res;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching system summary', err);
        this.isLoading = false;
      },
    });
  }

  fetchReportHistory() {
    this.api.get<any[]>('Reports/history').subscribe({
      next: (res) => {
        this.reportHistory = res;
      },
      error: (err) => {
        console.error('Error fetching report history', err);
      },
    });
  }

  downloadExcel() {
    this.isExporting = true;
    this.api.get<any>('Reports/system-summary?export=excel').subscribe({
      next: (res) => {
        this.isExporting = false;
        const link = document.createElement('a');
        link.href = res.fileUrl;
        link.download = res.fileName;
        link.click();
      },
      error: (err) => {
        console.error('Error exporting report', err);
        this.isExporting = false;
      },
    });
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleString();
  }
}
