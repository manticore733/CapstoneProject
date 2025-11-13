
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClientService } from '../../services/client-service';
import { DocumentService } from '../../services/document-service';
import { forkJoin, switchMap, map } from 'rxjs';
import { Router } from '@angular/router';

interface DocumentWithClient {
  documentId: number;
  documentName: string;
  documentURL: string;
  proofTypeName: string;
  clientId: number;
  clientName: string;
  clientEmail: string;
}

@Component({
  selector: 'app-all-documents',
  imports: [CommonModule, FormsModule],
  templateUrl: './all-documents.html',
  styleUrl: './all-documents.css',
})
export class AllDocuments implements OnInit {
  allDocuments: DocumentWithClient[] = [];
  filteredDocuments: DocumentWithClient[] = [];
  loading = false;
  error: string | null = null;

  // Search and Filter
  searchTerm = '';
  documentTypeFilter = 'all';
  documentTypes: string[] = [];

  // Pagination
  currentPage = 1;
  itemsPerPage = 10;

  constructor(
    private clientService: ClientService,
    private documentService: DocumentService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAllDocuments();
  }

  loadAllDocuments(): void {
    this.loading = true;
    this.error = null;

    // Get all clients
    this.clientService.getMyClients()
      .pipe(
        switchMap(clients => {
          if (clients.length === 0) {
            return [];
          }

          // For each client, fetch their documents
          const documentRequests = clients.map(client =>
            this.documentService.getDocumentsForClient(client.userId).pipe(
              map(documents =>
                documents.map(doc => ({
                  documentId: doc.documentId,
                  documentName: doc.documentName,
                  documentURL: doc.documentURL,
                  proofTypeName: doc.proofTypeName,
                  clientId: client.userId,
                  clientName: client.userFullName,
                  clientEmail: client.userEmail,
                }))
              )
            )
          );

          // Wait for all document requests to complete
          return forkJoin(documentRequests);
        }),
        map(documentsArrays => {
          // Flatten the array of arrays into a single array
          return documentsArrays.flat();
        })
      )
      .subscribe({
        next: (documents) => {
          this.allDocuments = documents;
          this.filteredDocuments = [...documents];
          this.extractDocumentTypes();
          this.loading = false;
        },
        error: (err) => {
          console.error('Error fetching all documents', err);
          this.error = 'Failed to load documents.';
          this.loading = false;
        },
      });
  }

  // Extract unique document types for filter
  extractDocumentTypes(): void {
    const types = new Set(this.allDocuments.map(doc => doc.proofTypeName));
    this.documentTypes = Array.from(types).sort();
  }

  // Search and Filter Logic
  applyFilters(): void {
    this.filteredDocuments = this.allDocuments.filter(doc => {
      const matchesSearch = !this.searchTerm || 
        doc.documentName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        doc.clientName.toLowerCase().includes(this.searchTerm.toLowerCase());

      const matchesType = this.documentTypeFilter === 'all' || 
        doc.proofTypeName === this.documentTypeFilter;

      return matchesSearch && matchesType;
    });
    this.currentPage = 1; // Reset to first page
  }

  // Pagination Logic
  getPaginatedDocuments(): DocumentWithClient[] {
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

  // Navigation
  viewClientDetails(clientId: number): void {
    this.router.navigate(['/bank/dashboard/clients', clientId]);
  }

  openDocument(url: string): void {
    window.open(url, '_blank');
  }

  isImageFile(url: string): boolean {
    return /\.(jpg|jpeg|png|gif|webp)$/i.test(url);
  }

  getFileExtension(url: string): string {
    const extension = url.split('.').pop()?.toUpperCase();
    return extension || 'FILE';
  }
}
