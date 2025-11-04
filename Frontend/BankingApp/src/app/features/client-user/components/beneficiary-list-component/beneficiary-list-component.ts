import { Component } from '@angular/core';
import { Beneficiary } from '../../../../core/models/Beneficiary';
import { BeneficiaryService } from '../../services/beneficiary-service';
import { CommonModule } from '@angular/common';
import { BeneficiaryFormComponent } from '../beneficiary-form-component/beneficiary-form-component';

@Component({
  selector: 'app-beneficiary-list-component',
  imports: [CommonModule, BeneficiaryFormComponent],
  templateUrl: './beneficiary-list-component.html',
  styleUrl: './beneficiary-list-component.css',
})
export class BeneficiaryListComponent {
  beneficiaries: Beneficiary[] = [];
  loading = false;
  error: string | null = null;
  
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
        this.loading = false;
        this.error = null;
      },
      error: (err) => {
        console.error('Error fetching beneficiaries', err);
        // Handle the "Pending Approval" error from the API
        if (err.status === 404 || err.status === 403) {
          this.error = "Your account is not active or approved. Please upload documents to enable this feature.";
        } else {
          this.error = 'Failed to load beneficiaries.';
        }
        this.loading = false;
      },
    });
  }

  // --- Form/Modal Controls ---

  openAddForm(): void {
    this.selectedBeneficiary = null;
    this.showForm = true;
  }

  openEditForm(beneficiary: Beneficiary): void {
    this.selectedBeneficiary = { ...beneficiary }; // Pass a copy
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
    this.selectedBeneficiary = null;
  }

  onFormSaved(): void {
    this.loadBeneficiaries(); // Refresh the list
    this.closeForm();
  }

  // --- CRUD Actions ---

  deleteBeneficiary(id: number): void {
    if (confirm('Are you sure you want to delete this beneficiary?')) {
      this.beneficiaryService.deleteBeneficiary(id).subscribe({
        next: () => {
          this.loadBeneficiaries(); // Refresh on success
        },
        error: (err) => {
          console.error('Error deleting beneficiary', err);
          alert('Failed to delete beneficiary.');
        },
      });
    }
  }

}
