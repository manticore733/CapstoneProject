import { Component } from '@angular/core';
import { ClientUser } from '../../../../core/models/ClientUser';
import { ClientService } from '../../services/client-service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ClientForm } from "../client-form/client-form";

@Component({
  selector: 'app-client-list',
  imports: [FormsModule, CommonModule, ClientForm],
  templateUrl: './client-list.html',
  styleUrl: './client-list.css',
})
export class ClientList {
  clients: ClientUser[] = [];
  loading = false;
  error: string | null = null;

  constructor(private clientService: ClientService, private router: Router) {}
  showModal = false;
  selectedClient: ClientUser | null = null;

  openAddForm() {
    this.selectedClient = null;
    this.showModal = true;
  }

  openEditForm(client: ClientUser) {
    this.selectedClient = client;
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  onSaved() {
    this.loadClients();
  }

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients(): void {
    this.loading = true;
    this.clientService.getMyClients().subscribe({
      next: (res) => {
        this.clients = res;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching clients', err);
        this.error = 'Failed to load clients.';
        this.loading = false;
      },
    });
  }

  // Navigate to client details page
  viewClientDetails(clientId: number): void {
    this.router.navigate([`/bank/dashboard/clients/${clientId}`]);
  }

  // Delete client
  deleteClient(clientId: number): void {
    if (confirm('Are you sure you want to delete this client?')) {
      this.clientService.deleteClient(clientId).subscribe({
        next: () => this.loadClients(),
        error: (err) => console.error('Error deleting client', err),
      });
    }
  }

  getStatusBadgeClass(statusName: string): string {
    switch (statusName.toUpperCase()) {
      case 'APPROVED':
        return 'bg-green-200 text-green-800';
      case 'REJECTED':
        return 'bg-red-200 text-red-800';
      case 'PENDING':
      default:
        return 'bg-yellow-200 text-yellow-800';
    }
  }

  showForm = false;


openAddClientForm() {
  this.selectedClient = null;
  this.showForm = true;
}

openEditClientForm(client: ClientUser) {
  this.selectedClient = client;
  this.showForm = true;
}

closeForm() {
  this.showForm = false;
}

onClientSaved() {
  this.loadClients(); // refresh list after save
}

}
