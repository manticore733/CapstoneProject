


import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http'; // <-- Import HttpClient
import { Observable } from 'rxjs';
// Make sure this path points to your new 'report.model.ts' file
import { ClientTransactionReportDto, ClientUserReportDto, ReportRecord, ReportResultDto, SystemSummaryReportDto } from '../models/ClientUserReportDto'; 
import { environment } from '../../environments/environment'; // <-- Import environment

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  private readonly endpoint = 'Reports';
  private baseUrl = environment.apiUrl; // <-- Get base URL

  // --- FIX: We inject HttpClient directly, bypassing ApiService ---
  constructor(private http: HttpClient) {}

  // Helper to build date parameters
  private createParams(startDate?: string, endDate?: string, reportType?: string): HttpParams {
    let params = new HttpParams();
    if (startDate) {
      params = params.set('startDate', startDate);
    }
    if (endDate) {
      params = params.set('endDate', endDate);
    }
    if (reportType) {
      params = params.set('reportType', reportType);
    }
    return params;
  }

  // === CLIENT USER ===
  getClientReport(startDate?: string, endDate?: string): Observable<ClientUserReportDto> {
    const params = this.createParams(startDate, endDate);
    // --- FIX: Use http.get ---
    return this.http.get<ClientUserReportDto>(`${this.baseUrl}/${this.endpoint}/client`, { params });
  }

  generateClientReportExcel(startDate?: string, endDate?: string): Observable<ReportResultDto> {
    let params = this.createParams(startDate, endDate);
    params = params.set('export', 'excel'); 
    // --- FIX: Use http.get ---
    return this.http.get<ReportResultDto>(`${this.baseUrl}/${this.endpoint}/client`, { params });
  }

  // === BANK USER ===
  getBankUserReport(reportType: string, startDate?: string, endDate?: string): Observable<ClientTransactionReportDto[]> {
    const params = this.createParams(startDate, endDate, reportType);
    // --- FIX: Use http.get ---
    return this.http.get<ClientTransactionReportDto[]>(`${this.baseUrl}/${this.endpoint}/bank/transactions`, { params });
  }

  generateBankUserReportExcel(reportType: string, startDate?: string, endDate?: string): Observable<ReportResultDto> {
    let params = this.createParams(startDate, endDate, reportType);
    params = params.set('export', 'excel');
    // --- FIX: Use http.get ---
    return this.http.get<ReportResultDto>(`${this.baseUrl}/${this.endpoint}/bank/transactions`, { params });
  }

  // === SUPER ADMIN ===
  getSystemSummary(startDate?: string, endDate?: string): Observable<SystemSummaryReportDto> {
    const params = this.createParams(startDate, endDate);
    // --- FIX: Use http.get ---
    return this.http.get<SystemSummaryReportDto>(`${this.baseUrl}/${this.endpoint}/system-summary`, { params });
  }
  
  // === COMMON ===
  getReportHistory(): Observable<ReportRecord[]> {
    // --- FIX: Use http.get ---
    return this.http.get<ReportRecord[]>(`${this.baseUrl}/${this.endpoint}/history`);
  }
}