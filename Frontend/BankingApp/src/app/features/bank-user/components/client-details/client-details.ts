


import { Component } from '@angular/core';
import { ClientUser } from '../../../../core/models/ClientUser';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ClientService } from '../../services/client-service';
import { DocumentService } from '../../services/document-service';

@Component({
  selector: 'app-client-details',
  imports: [FormsModule, CommonModule],
  templateUrl: './client-details.html',
  styleUrl: './client-details.css',
})
export class ClientDetails {
  clientId!: number;
  client: ClientUser | null = null;
  documents: any[] = [];
  filteredDocuments: any[] = [];
  loading = false;
  error: string | null = null;
  remark: string = '';

  // For modals
  showApproveModal = false;
  showRejectModal = false;
  initialBalance: number = 0;

  // Search and Filter
  searchTerm = '';
  proofTypeFilter = 'all';
  proofTypes: string[] = [];

  // Pagination
  currentPage = 1;
  itemsPerPage = 10;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private clientService: ClientService,
    private documentService: DocumentService
  ) {}

  ngOnInit(): void {
    this.clientId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadClient();
    this.loadDocuments();
  }

  loadClient(): void {
    this.loading = true;
    this.clientService.getClientById(this.clientId).subscribe({
      next: (res: ClientUser | null) => {
        this.client = res;
        this.loading = false;
      },
      error: (err: any) => {
        console.error('Error fetching client', err);
        this.error = 'Failed to load client details.';
        this.loading = false;
      },
    });
  }

  loadDocuments(): void {
    this.documentService.getDocumentsForClient(this.clientId).subscribe({
      next: (res) => {
        this.documents = res;
        this.filteredDocuments = [...res];
        this.extractProofTypes();
      },
      error: (err) => console.error('Error fetching documents', err),
    });
  }

  // Extract unique proof types
  extractProofTypes(): void {
    const types = new Set(this.documents.map(doc => doc.proofTypeName));
    this.proofTypes = Array.from(types).sort();
  }

  // Filter documents
  applyFilters(): void {
    this.filteredDocuments = this.documents.filter(doc => {
      const matchesSearch = !this.searchTerm || 
        doc.documentName.toLowerCase().includes(this.searchTerm.toLowerCase());

      const matchesType = this.proofTypeFilter === 'all' || 
        doc.proofTypeName === this.proofTypeFilter;

      return matchesSearch && matchesType;
    });
    this.currentPage = 1;
  }

  // Pagination
  getPaginatedDocuments(): any[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.filteredDocuments.slice(start, end);
  }

  getStartIndex(): number {
    return (this.currentPage - 1) * this.itemsPerPage;
  }

  getEndIndex(): number {
    return Math.min(this.currentPage * this.itemsPerPage, this.filteredDocuments.length);
  }

  getTotalPages(): number {
    return Math.ceil(this.filteredDocuments.length / this.itemsPerPage);
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

  // Modal methods
  openApproveModal(): void {
    this.showApproveModal = true;
    this.initialBalance = 0;
    this.remark = '';
  }

  closeApproveModal(): void {
    this.showApproveModal = false;
  }

  openRejectModal(): void {
    this.showRejectModal = true;
    this.remark = '';
  }

  closeRejectModal(): void {
    this.showRejectModal = false;
  }

  submitApproval(): void {
    const approvalData = {
      isApproved: true,
      initialBalance: this.initialBalance,
      remark: this.remark || ''
    };

    this.clientService.approveClient(this.clientId, approvalData).subscribe({
      next: () => {
        alert('Client approved successfully!');
        this.closeApproveModal();
        this.loadClient();
        this.remark = '';
      },
      error: (err) => {
        console.error('Error approving client', err);
        alert('Something went wrong while approving.');
      },
    });
  }

  submitRejection(): void {
    const approvalData = {
      isApproved: false,
      initialBalance: 0,
      remark: this.remark || ''
    };

    this.clientService.approveClient(this.clientId, approvalData).subscribe({
      next: () => {
        alert('Client rejected successfully!');
        this.closeRejectModal();
        this.loadClient();
        this.remark = '';
      },
      error: (err) => {
        console.error('Error rejecting client', err);
        alert('Something went wrong while rejecting.');
      },
    });
  }

  goBack(): void {
    this.router.navigate(['/bank/dashboard/clients']);
  }

  viewDocuments(): void {
    this.router.navigate(['/bank/dashboard/documents', this.clientId]);
  }
}
