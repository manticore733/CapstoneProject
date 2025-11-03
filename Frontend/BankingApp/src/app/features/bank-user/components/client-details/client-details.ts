import { Component } from '@angular/core';
import { ClientUser } from '../../../../core/models/ClientUser';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ClientService } from '../../services/client-service';
import { DocumentService } from '../../services/document-service';

@Component({
  selector: 'app-client-details',
  imports: [FormsModule,CommonModule],
  templateUrl: './client-details.html',
  styleUrl: './client-details.css',
})
export class ClientDetails {
  clientId!: number;
  client: ClientUser | null = null;
  documents: any[] = [];
  loading = false;
  error: string | null = null;

  // For approve modal
  showApprovalModal = false;
  isApproved = false;
  initialBalance: number = 0;

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

  loadClient() {
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

  loadDocuments() {
    this.documentService.getDocumentsForClient(this.clientId).subscribe({
      next: (res) => (this.documents = res),
      error: (err) => console.error('Error fetching documents', err),
    });
  }

  approveOrRejectClient() {
    this.showApprovalModal = true;
    this.isApproved = false;
    this.initialBalance = 0;
  }

  submitApproval() {
    const approvalData = {
      isApproved: this.isApproved,
      initialBalance: this.initialBalance,
    };

    this.clientService.approveClient(this.clientId, approvalData).subscribe({
      next: () => {
        alert('Client status updated successfully!');
        this.showApprovalModal = false;
        this.loadClient();
      },
      error: (err) => {
        console.error('Error approving client', err);
        alert('Something went wrong while processing.');
      },
    });
  }

  goBack() {
    this.router.navigate(['/bank/dashboard/clients']);
  }

viewDocuments() {
  this.router.navigate(['/bank/dashboard/documents', this.clientId]);
}


}
