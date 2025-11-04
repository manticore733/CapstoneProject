import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Beneficiary } from '../../../../core/models/Beneficiary';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BeneficiaryService } from '../../services/beneficiary-service';
import { UpdateBeneficiaryDto } from '../../../../core/models/UpdateBeneficiaryDto';
import { CreateBeneficiaryDto } from '../../../../core/models/CreateBeneficiaryDto';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
@Component({
  selector: 'app-beneficiary-form-component',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './beneficiary-form-component.html',
  styleUrl: './beneficiary-form-component.css',
})
export class BeneficiaryFormComponent {
  @Input() beneficiary: Beneficiary | null = null; // for edit mode
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  form!: FormGroup;
  loading = false;
  title = '';

  constructor(
    private fb: FormBuilder,
    private beneficiaryService: BeneficiaryService
  ) {}

  ngOnInit(): void {
    this.title = this.beneficiary ? 'Edit Beneficiary' : 'Add New Beneficiary';

    // Initialize the form based on backend DTOs
    this.form = this.fb.group({
      beneficiaryName: [this.beneficiary?.beneficiaryName || '', [Validators.required]],
      accountNumber: [this.beneficiary?.accountNumber || '', [Validators.required, Validators.pattern(/^[0-9]+$/)]],
      bankName: [this.beneficiary?.bankName || '', [Validators.required]],
      ifsc: [this.beneficiary?.ifsc || '', [Validators.required]],
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched(); // Show validation errors
      return;
    }

    this.loading = true;
    const formValue = this.form.value;

    if (this.beneficiary) {
      // --- UPDATE (Edit) ---
      const dto: UpdateBeneficiaryDto = {
        beneficiaryName: formValue.beneficiaryName,
        accountNumber: formValue.accountNumber,
        bankName: formValue.bankName,
        ifsc: formValue.ifsc,
      };
      
      this.beneficiaryService.updateBeneficiary(this.beneficiary.beneficiaryId, dto).subscribe({
        next: () => this.handleSuccess(),
        error: (err) => this.handleError(err),
      });

    } else {
      // --- CREATE (Add) ---
      const dto: CreateBeneficiaryDto = {
        beneficiaryName: formValue.beneficiaryName,
        accountNumber: formValue.accountNumber,
        bankName: formValue.bankName,
        ifsc: formValue.ifsc,
      };

      this.beneficiaryService.createBeneficiary(dto).subscribe({
        next: () => this.handleSuccess(),
        error: (err) => this.handleError(err),
      });
    }
  }

  private handleSuccess(): void {
    this.loading = false;
    this.saved.emit(); // Tell parent to refresh
  }

  private handleError(err: any): void {
    console.error('Error saving beneficiary', err);
    this.loading = false;
    // You could add a specific error message to the form here
    alert('An error occurred. Please try again.');
  }

  cancel(): void {
    this.close.emit(); // Tell parent to close
  }

}
