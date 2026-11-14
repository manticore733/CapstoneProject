
import { Component } from '@angular/core';
import { Beneficiary } from '../../../../core/models/Beneficiary';
import { BeneficiaryService } from '../../services/beneficiary-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BeneficiaryFormComponent } from '../beneficiary-form-component/beneficiary-form-component';

@Component({
  selector: 'app-beneficiary-list-component',
  imports: [CommonModule, FormsModule, BeneficiaryFormComponent],
  templateUrl: './beneficiary-list-component.html',
  styleUrl: './beneficiary-list-component.css',
})
export class BeneficiaryListComponent {
  beneficiaries: Beneficiary[] = [];
  filteredBeneficiaries: Beneficiary[] = [];
  loading = false;
  error: string | null = null;

  // Search and Filter
  searchTerm = '';

  // Pagination
  currentPage = 1;
  itemsPerPage = 10;

  showForm = false;
  selectedBeneficiary: Beneficiary | null = null;

  constructor(private beneficiaryService: BeneficiaryService) {}

  ngOnInit(): void {
    this.loadBeneficiaries();
  }

  loadBeneficiaries(): void {
    this.loading = true;
    this.beneficiaryService.getMyBeneficiaries().subscribe({
      next: (data) => {
        this.beneficiaries = data;
        this.filteredBeneficiaries = [...data];
        this.loading = false;
        this.error = null;
      },
      error: (err) => {
        console.error('Error fetching beneficiaries', err);
        if (err.status === 404 || err.status === 403) {
          this.error = "Your account is not active or approved. Please upload documents to enable this feature.";
        } else {
          this.error = 'Failed to load beneficiaries.';
        }
        this.loading = false;
      },
    });
  }

  // Search and Filter
  applyFilters(): void {
    this.filteredBeneficiaries = this.beneficiaries.filter(b => {
      const matchesSearch = !this.searchTerm ||
        b.beneficiaryName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        b.accountNumber.includes(this.searchTerm);

      return matchesSearch;
    });
    this.currentPage = 1;
  }

  // Pagination
  getPaginatedBeneficiaries(): Beneficiary[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.filteredBeneficiaries.slice(start, end);
  }

  getStartIndex(): number {
    return (this.currentPage - 1) * this.itemsPerPage;
  }

  getEndIndex(): number {
    return Math.min(this.currentPage * this.itemsPerPage, this.filteredBeneficiaries.length);
  }

  getTotalPages(): number {
    return Math.ceil(this.filteredBeneficiaries.length / this.itemsPerPage);
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

  // Form Controls
  openAddForm(): void {
    this.selectedBeneficiary = null;
    this.showForm = true;
  }

  openEditForm(beneficiary: Beneficiary): void {
    this.selectedBeneficiary = { ...beneficiary };
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
    this.selectedBeneficiary = null;
  }

  onFormSaved(): void {
    this.loadBeneficiaries();
    this.closeForm();
  }

  // CRUD Actions
  deleteBeneficiary(id: number): void {
    if (confirm('Are you sure you want to delete this beneficiary?')) {
      this.beneficiaryService.deleteBeneficiary(id).subscribe({
        next: () => {
          this.loadBeneficiaries();
        },
        error: (err) => {
          console.error('Error deleting beneficiary', err);
          alert('Failed to delete beneficiary.');
        },
      });
    }
  }
}
