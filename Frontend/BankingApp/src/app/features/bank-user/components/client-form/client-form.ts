import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ClientUser } from '../../../../core/models/ClientUser';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClientService } from '../../services/client-service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-client-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './client-form.html',
  styleUrl: './client-form.css',
})
export class ClientForm {

   @Input() client: ClientUser | null = null; // for edit
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  clientForm!: FormGroup;
  loading = false;
  title = '';

  constructor(private fb: FormBuilder, private clientService: ClientService) {}

  ngOnInit(): void {
    this.title = this.client ? 'Edit Client' : 'Add New Client';

    this.clientForm = this.fb.group({
      userFullName: [this.client?.userFullName || '', Validators.required],
      userName: [this.client?.userName || '', Validators.required],
      password: ['', this.client ? [] : [Validators.required, Validators.minLength(6)]],
      userEmail: [this.client?.userEmail || '', [Validators.required, Validators.email]],
      userPhone: [this.client?.userPhone || '', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
      address: [this.client?.address || '', Validators.required],
      establishmentDate: [null], // only used for creation
    });
  }

  save() {
    if (this.clientForm.invalid) return;
    this.loading = true;

    const formValue = this.clientForm.value;

    const payload = this.client
      ? {
          userFullName: formValue.userFullName,
          userEmail: formValue.userEmail,
          userPhone: formValue.userPhone,
          address: formValue.address,
        }
      : {
          ...formValue,
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
        this.loading = false;
        alert('Something went wrong while saving.');
      },
    });
  }

  cancel() {
    this.close.emit();
  }

}
