// import { Component, EventEmitter, Input, Output } from '@angular/core';
// import { ClientUser } from '../../../../core/models/ClientUser';
// import { FormBuilder, FormGroup, Validators } from '@angular/forms';
// import { ClientService } from '../../services/client-service';
// import { CommonModule } from '@angular/common';
// import { ReactiveFormsModule } from '@angular/forms';

// @Component({
//   selector: 'app-client-form',
//   imports: [CommonModule, ReactiveFormsModule],
//   templateUrl: './client-form.html',
//   styleUrl: './client-form.css',
// })
// export class ClientForm {

//    @Input() client: ClientUser | null = null; // for edit
//   @Output() close = new EventEmitter<void>();
//   @Output() saved = new EventEmitter<void>();

//   clientForm!: FormGroup;
//   loading = false;
//   title = '';

//   constructor(private fb: FormBuilder, private clientService: ClientService) {}

//   ngOnInit(): void {
//     this.title = this.client ? 'Edit Client' : 'Add New Client';

//     this.clientForm = this.fb.group({
//       userFullName: [this.client?.userFullName || '', Validators.required],
//       userName: [this.client?.userName || '', Validators.required],
//       password: ['', this.client ? [] : [Validators.required, Validators.minLength(6)]],
//       userEmail: [this.client?.userEmail || '', [Validators.required, Validators.email]],
//       userPhone: [this.client?.userPhone || '', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
//       address: [this.client?.address || '', Validators.required],
//       establishmentDate: [null], // only used for creation
//     });
//   }

//   save() {
//     if (this.clientForm.invalid) return;
//     this.loading = true;

//     const formValue = this.clientForm.value;

//     const payload = this.client
//       ? {
//           userFullName: formValue.userFullName,
//           userEmail: formValue.userEmail,
//           userPhone: formValue.userPhone,
//           address: formValue.address,
//           statusId: this.client.statusId
//         }
//       : {
//           ...formValue,
//           establishmentDate: formValue.establishmentDate,
//         };

//     const request$ = this.client
//       ? this.clientService.updateClient(this.client.userId, payload)
//       : this.clientService.createClient(payload);

//     request$.subscribe({
//       next: () => {
//         this.loading = false;
//         this.saved.emit();
//         this.close.emit();
//       },
//       error: (err: any) => {
//         console.error('Error saving client', err);
//         this.loading = false;
//         alert('Something went wrong while saving.');
//       },
//     });
//   }

//   cancel() {
//     this.close.emit();
//   }

// }







import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ClientUser } from '../../../../core/models/ClientUser';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClientService } from '../../services/client-service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

@Component({
  selector: 'app-client-form',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './client-form.html',
  styleUrl: './client-form.css',
})
export class ClientForm {
  @Input() client: ClientUser | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  clientForm!: FormGroup;
  loading = false;
  formError: string | null = null;
  title = '';
  maxDate: string = '';

  constructor(private fb: FormBuilder, private clientService: ClientService) {
    // Set max date to today
    const today = new Date();
    this.maxDate = today.toISOString().split('T')[0];
  }

  ngOnInit(): void {
    this.title = this.client ? 'Edit Client' : 'Add New Client';

    this.clientForm = this.fb.group({
      userFullName: [
        this.client?.userFullName || '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(100),
        ],
      ],
      userName: [
        this.client?.userName || '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(20),
        ],
      ],
      password: [
        '',
        this.client
          ? []
          : [
              Validators.required,
              Validators.minLength(6),
              Validators.maxLength(20),
            ],
      ],
      userEmail: [
        this.client?.userEmail || '',
        [Validators.required, Validators.email],
      ],
      userPhone: [
        this.client?.userPhone || '',
        [
          Validators.required,
          Validators.pattern(/^[1-9][0-9]{9}$/),
        ],
      ],
      address: [
        this.client?.address || '',
        [
          Validators.required,
          Validators.minLength(5),
          Validators.maxLength(100),
        ],
      ],
      establishmentDate: [null, this.client ? [] : [Validators.required]],
    });

    // Disable username when editing
    if (this.client) {
      this.clientForm.get('userName')?.disable();
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.clientForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  getErrorMessage(fieldName: string): string {
    const field = this.clientForm.get(fieldName);
    if (!field) return '';

    if (field.errors?.['required']) return `${this.getFieldLabel(fieldName)} is required`;
    if (field.errors?.['minlength']) return `Minimum ${field.errors['minlength'].requiredLength} characters`;
    if (field.errors?.['maxlength']) return `Maximum ${field.errors['maxlength'].requiredLength} characters`;
    if (field.errors?.['email']) return 'Invalid email address';
    if (field.errors?.['pattern']) {
      if (fieldName === 'userPhone') return 'Phone must be 10 digits starting with 1-9';
      return 'Invalid format';
    }
    return 'Invalid input';
  }

  getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      userFullName: 'Company name',
      userName: 'Username',
      password: 'Password',
      userEmail: 'Email',
      userPhone: 'Phone',
      address: 'Address',
      establishmentDate: 'Establishment date',
    };
    return labels[fieldName] || fieldName;
  }

  save() {
    if (this.clientForm.invalid) return;

    this.loading = true;
    this.formError = null;

    const formValue = this.clientForm.getRawValue();

    const payload = this.client
      ? {
          userFullName: formValue.userFullName.trim(),
          userEmail: formValue.userEmail.trim(),
          userPhone: formValue.userPhone.trim(),
          address: formValue.address.trim(),
          statusId: this.client.statusId,
        }
      : {
          userFullName: formValue.userFullName.trim(),
          userName: formValue.userName.trim(),
          password: formValue.password.trim(),
          userEmail: formValue.userEmail.trim(),
          userPhone: formValue.userPhone.trim(),
          address: formValue.address.trim(),
          establishmentDate: formValue.establishmentDate,
        };

    const request$ = this.client
      ? this.clientService.updateClient(this.client.userId, payload)
      : this.clientService.createClient(payload);

    request$.subscribe({
      next: () => {
        this.loading = false;
        this.saved.emit();
        this.close.emit();
      },
      error: (err: any) => {
        console.error('Error saving client', err);
        this.formError = err.error?.message || 'Failed to save client. Please try again.';
        this.loading = false;
      },
    });
  }

  cancel() {
    this.close.emit();
  }
}
