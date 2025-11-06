

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
  filteredClients: ClientUser[] = [];
  loading = false;
  error: string | null = null;

  // Search and Filter
  searchTerm = '';
  statusFilter = 'all';

  // Pagination
  currentPage = 1;
  itemsPerPage = 10;

  // Form
  showForm = false;
  selectedClient: ClientUser | null = null;

  constructor(private clientService: ClientService, private router: Router) {}

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients(): void {
    this.loading = true;
    this.error = null;
    this.clientService.getMyClients().subscribe({
      next: (res) => {
        this.clients = res;
        this.filteredClients = [...res];
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching clients', err);
        this.error = 'Failed to load clients.';
        this.loading = false;
      },
    });
  }

  // Search and Filter Logic
  applyFilters(): void {
    this.filteredClients = this.clients.filter(client => {
      const matchesSearch = !this.searchTerm || 
        client.userFullName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        client.userName.toLowerCase().includes(this.searchTerm.toLowerCase());

      const matchesStatus = this.statusFilter === 'all' || 
        client.statusName === this.statusFilter;

      return matchesSearch && matchesStatus;
    });
    this.currentPage = 1; // Reset to first page
  }

  // Pagination Logic
  getPaginatedClients(): ClientUser[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.filteredClients.slice(start, end);
  }

  getStartIndex(): number {
    return (this.currentPage - 1) * this.itemsPerPage;
  }

  getEndIndex(): number {
    return Math.min(this.currentPage * this.itemsPerPage, this.filteredClients.length);
  }

  getTotalPages(): number {
    return Math.ceil(this.filteredClients.length / this.itemsPerPage);
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

  // Form Actions
  openAddClientForm(): void {
    this.selectedClient = null;
    this.showForm = true;
  }

  openEditClientForm(client: ClientUser): void {
    this.selectedClient = client;
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
    this.selectedClient = null;
  }

  onClientSaved(): void {
    this.loadClients();
  }

  // Navigation
  viewClientDetails(clientId: number): void {
    this.router.navigate([`/bank/dashboard/clients/${clientId}`]);
  }

  // Delete
  deleteClient(clientId: number): void {
    if (confirm('Are you sure you want to delete this client? This action cannot be undone.')) {
      this.clientService.deleteClient(clientId).subscribe({
        next: () => {
          this.loadClients();
          alert('Client deleted successfully!');
        },
        error: (err) => {
          console.error('Error deleting client', err);
          alert('Failed to delete client. Please try again.');
        },
      });
    }
  }
}
