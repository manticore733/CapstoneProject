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
  
  // Form validation states
  submitting = false;
  formError: string | null = null;
  maxDate: string = '';

  // Pagination
  currentPage = 1;
  itemsPerPage = 10;
  totalPages = 1;

  // Search and Filter
  searchTerm = '';
  statusFilter = 'all';
  
  // Sorting
  sortColumn: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  constructor(private bankService: BankService) {
    // Set max date to today
    const today = new Date();
    this.maxDate = today.toISOString().split('T')[0];
  }

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

    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(bank =>
        bank.bankName.toLowerCase().includes(term) ||
        bank.ifsc.toLowerCase().includes(term)
      );
    }

    if (this.statusFilter !== 'all') {
      const isActive = this.statusFilter === 'active';
      filtered = filtered.filter(bank => bank.isActive === isActive);
    }

    this.filteredBanks = filtered;
    this.updatePagination();
    this.currentPage = 1;
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
    this.formError = null;
  }

  editBank(bank: Bank) {
    this.isEdit = true;
    this.showForm = true;
    this.selectedBank = bank;
    this.newBank = {
      bankName: bank.bankName,
      ifsc: bank.ifsc,
    };
    this.formError = null;
  }

  saveBank() {
    this.submitting = true;
    this.formError = null;

    if (this.isEdit && this.selectedBank) {
      const updatePayload = {
        bankName: this.newBank.bankName.trim(),
        ifsc: this.newBank.ifsc.trim().toUpperCase(),
      };
      this.bankService.update(this.selectedBank.bankId, updatePayload).subscribe({
        next: () => {
          this.refresh();
          this.showSuccessMessage('Bank updated successfully!');
        },
        error: err => {
          console.error('Error updating bank', err);
          this.formError = err.error?.message || 'Failed to update bank. Please try again.';
          this.submitting = false;
        },
      });
    } else {
      const createPayload = {
        bankName: this.newBank.bankName.trim(),
        ifsc: this.newBank.ifsc.trim().toUpperCase(),
        establishmentDate: this.newBank.establishmentDate,
      };
      this.bankService.create(createPayload).subscribe({
        next: () => {
          this.refresh();
          this.showSuccessMessage('Bank created successfully!');
        },
        error: err => {
          console.error('Error adding bank', err);
          this.formError = err.error?.message || 'Failed to create bank. Please check if IFSC code already exists.';
          this.submitting = false;
        },
      });
    }
  }

  deleteBank(id: number) {
    if (confirm('Are you sure you want to delete this bank? This action cannot be undone.')) {
      this.bankService.delete(id).subscribe({
        next: () => {
          this.refresh();
          this.showSuccessMessage('Bank deleted successfully!');
        },
        error: err => {
          console.error('Error deleting bank', err);
          alert('Failed to delete bank. It may have associated users or accounts.');
        },
      });
    }
  }

  cancelForm() {
    this.showForm = false;
    this.isEdit = false;
    this.selectedBank = null;
    this.newBank = {};
    this.formError = null;
    this.submitting = false;
  }

  private refresh() {
    this.loadBanks();
    this.cancelForm();
  }

  private showSuccessMessage(message: string) {
    alert(message);
  }
}
