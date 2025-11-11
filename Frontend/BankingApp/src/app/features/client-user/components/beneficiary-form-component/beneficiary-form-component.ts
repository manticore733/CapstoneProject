
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
  @Input() beneficiary: Beneficiary | null = null;
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

    // Initialize form with enhanced validations
    this.form = this.fb.group({
      beneficiaryName: [
        this.beneficiary?.beneficiaryName || '',
        [
          Validators.required,
          Validators.minLength(2) // Minimum 2 characters
        ]
      ],
      accountNumber: [
        this.beneficiary?.accountNumber || '',
        [
          Validators.required,
          Validators.pattern(/^[0-9]+$/) // Only digits
        ]
      ],
      bankName: [
        this.beneficiary?.bankName || '',
        [
          Validators.required,
          Validators.minLength(2) // Minimum 2 characters
        ]
      ],
      ifsc: [
        this.beneficiary?.ifsc || '',
        [
          Validators.required,
          Validators.minLength(4), // IFSC is typically 11 chars, but at least 4
          Validators.pattern(/^[A-Z0-9]+$/) // Only uppercase letters and digits
        ]
      ],
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    const formValue = this.form.value;

    if (this.beneficiary) {
      // UPDATE (Edit)
      const dto: UpdateBeneficiaryDto = {
        beneficiaryName: formValue.beneficiaryName.trim(),
        accountNumber: formValue.accountNumber.trim(),
        bankName: formValue.bankName.trim(),
        ifsc: formValue.ifsc.trim().toUpperCase(),
      };

      this.beneficiaryService.updateBeneficiary(this.beneficiary.beneficiaryId, dto).subscribe({
        next: () => this.handleSuccess(),
        error: (err) => this.handleError(err),
      });
    } else {
      // CREATE (Add)
      const dto: CreateBeneficiaryDto = {
        beneficiaryName: formValue.beneficiaryName.trim(),
        accountNumber: formValue.accountNumber.trim(),
        bankName: formValue.bankName.trim(),
        ifsc: formValue.ifsc.trim().toUpperCase(),
      };

      this.beneficiaryService.createBeneficiary(dto).subscribe({
        next: () => this.handleSuccess(),
        error: (err) => this.handleError(err),
      });
    }
  }

  private handleSuccess(): void {
    this.loading = false;
    this.saved.emit();
  }

  private handleError(err: any): void {
    console.error('Error saving beneficiary', err);
    this.loading = false;
    alert('An error occurred. Please try again.');
  }

  cancel(): void {
    this.close.emit();
  }

  // Helper method to get error messages
  getErrorMessage(fieldName: string): string {
    const control = this.form.get(fieldName);
    
    if (!control || !control.errors || !control.touched) {
      return '';
    }

    if (control.errors['required']) {
      return `${this.formatFieldName(fieldName)} is required.`;
    }
    
    if (control.errors['minLength']) {
      const minLen = control.errors['minLength'].requiredLength;
      return `${this.formatFieldName(fieldName)} must be at least ${minLen} characters.`;
    }
    
    if (control.errors['pattern']) {
      if (fieldName === 'accountNumber') {
        return 'Account number can only contain digits (0-9).';
      }
      if (fieldName === 'ifsc') {
        return 'IFSC code can only contain uppercase letters and digits.';
      }
    }

    return 'Invalid input.';
  }

  private formatFieldName(fieldName: string): string {
    // Convert camelCase to Title Case
    return fieldName
      .replace(/([A-Z])/g, ' $1')
      .replace(/^./, str => str.toUpperCase())
      .trim();
  }

  // Check if field is invalid and touched
  isFieldInvalid(fieldName: string): boolean {
    const control = this.form.get(fieldName);
    return !!(control && control.invalid && control.touched);
  }
}
