import { Component, OnInit } from '@angular/core';
import { BankService } from '../bank-service';
import { Bank } from '../../../../core/models/Bank';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-bank-list',
  imports: [CommonModule, FormsModule],
  templateUrl: './bank-list.html',
  styleUrl: './bank-list.css',
})
export class BankList implements OnInit {
  banks: Bank[] = [];
  filteredBanks: Bank[] = [];
  showForm = false;
  isEdit = false;
  selectedBank: Bank | null = null;
  newBank: any = {};

  // Pagination
  currentPage = 1;
  itemsPerPage = 10;
  totalPages = 1;

  // Search and Filter
  searchTerm = '';
  statusFilter = 'all'; // 'all', 'active', 'inactive'
  
  // Sorting
  sortColumn: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  constructor(private bankService: BankService) {}

  ngOnInit() {
    this.loadBanks();
  }

  loadBanks() {
    this.bankService.getAll().subscribe({
      next: res => {
        this.banks = res;
        this.applyFilters();
      },
      error: err => console.error('Error fetching banks', err),
    });
  }

  // Search and Filter Logic
  applyFilters() {
    let filtered = [...this.banks];

    // Apply search filter
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(bank =>
        bank.bankName.toLowerCase().includes(term) ||
        bank.ifsc.toLowerCase().includes(term)
      );
    }

    // Apply status filter
    if (this.statusFilter !== 'all') {
      const isActive = this.statusFilter === 'active';
      filtered = filtered.filter(bank => bank.isActive === isActive);
    }

    this.filteredBanks = filtered;
    this.updatePagination();
    this.currentPage = 1; // Reset to first page when filters change
  }

  clearSearch() {
    this.searchTerm = '';
    this.applyFilters();
  }

  clearStatusFilter() {
    this.statusFilter = 'all';
    this.applyFilters();
  }

  clearAllFilters() {
    this.searchTerm = '';
    this.statusFilter = 'all';
    this.applyFilters();
  }

  // Sorting Logic
  sortBy(column: string) {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }

    this.filteredBanks.sort((a: any, b: any) => {
      const aVal = a[column];
      const bVal = b[column];

      if (aVal < bVal) return this.sortDirection === 'asc' ? -1 : 1;
      if (aVal > bVal) return this.sortDirection === 'asc' ? 1 : -1;
      return 0;
    });
  }

  // Pagination Logic
  updatePagination() {
    this.totalPages = Math.ceil(this.filteredBanks.length / this.itemsPerPage);
  }

  getPaginatedBanks(): Bank[] {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    return this.filteredBanks.slice(startIndex, endIndex);
  }

  getStartIndex(): number {
    return (this.currentPage - 1) * this.itemsPerPage;
  }

  getEndIndex(): number {
    const end = this.currentPage * this.itemsPerPage;
    return end > this.filteredBanks.length ? this.filteredBanks.length : end;
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxPagesToShow = 5;
    
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPagesToShow / 2));
    let endPage = Math.min(this.totalPages, startPage + maxPagesToShow - 1);
    
    if (endPage - startPage < maxPagesToShow - 1) {
      startPage = Math.max(1, endPage - maxPagesToShow + 1);
    }
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    
    return pages;
  }

  goToPage(page: number) {
    this.currentPage = page;
  }

  previousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  onItemsPerPageChange() {
    this.updatePagination();
    this.currentPage = 1;
  }

  // CRUD Operations
  addBank() {
    this.isEdit = false;
    this.showForm = true;
    this.newBank = {};
  }

  editBank(bank: Bank) {
    this.isEdit = true;
    this.showForm = true;
    this.selectedBank = bank;
    this.newBank = {
      bankName: bank.bankName,
      ifsc: bank.ifsc,
    };
  }

  saveBank() {
    if (this.isEdit && this.selectedBank) {
      const updatePayload = {
        bankName: this.newBank.bankName,
        ifsc: this.newBank.ifsc,
      };
      this.bankService.update(this.selectedBank.bankId, updatePayload).subscribe({
        next: () => this.refresh(),
        error: err => console.error('Error updating bank', err),
      });
    } else {
      const createPayload = {
        bankName: this.newBank.bankName,
        ifsc: this.newBank.ifsc,
        establishmentDate: this.newBank.establishmentDate,
      };
      this.bankService.create(createPayload).subscribe({
        next: () => this.refresh(),
        error: err => console.error('Error adding bank', err),
      });
    }
  }

  deleteBank(id: number) {
    if (confirm('Are you sure you want to delete this bank?')) {
      this.bankService.delete(id).subscribe({
        next: () => this.refresh(),
        error: err => console.error('Error deleting bank', err),
      });
    }
  }

  cancelForm() {
    this.showForm = false;
    this.isEdit = false;
    this.selectedBank = null;
    this.newBank = {};
  }

  private refresh() {
    this.loadBanks();
    this.cancelForm();
  }
}