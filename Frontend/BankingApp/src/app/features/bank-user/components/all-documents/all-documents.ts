import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
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
  imports: [CommonModule],
  templateUrl: './all-documents.html',
  styleUrl: './all-documents.css',
})
export class AllDocuments implements OnInit {
  allDocuments: DocumentWithClient[] = [];
  loading = false;
  error: string | null = null;

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

    // Step 1: Get all clients
    this.clientService.getMyClients()
      .pipe(
        switchMap(clients => {
          if (clients.length === 0) {
            return [];
          }

          // Step 2: For each client, fetch their documents
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

          // Step 3: Wait for all document requests to complete
          return forkJoin(documentRequests);
        }),
        map(documentsArrays => {
          // Step 4: Flatten the array of arrays into a single array
          return documentsArrays.flat();
        })
      )
      .subscribe({
        next: (documents) => {
          this.allDocuments = documents;
          this.loading = false;
        },
        error: (err) => {
          console.error('Error fetching all documents', err);
          this.error = 'Failed to load documents.';
          this.loading = false;
        },
      });
  }

  openDocument(url: string): void {
    window.open(url, '_blank');
  }

  viewClientDetails(clientId: number): void {
    this.router.navigate(['/bank/dashboard/clients', clientId]);
  }

  isImageFile(url: string): boolean {
    return /\.(jpg|jpeg|png|gif|webp)$/i.test(url);
  }

  getFileExtension(url: string): string {
    const extension = url.split('.').pop()?.toUpperCase();
    return extension || 'FILE';
  }
}
