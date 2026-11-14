// import { Component, OnInit } from '@angular/core';
// import { ActivatedRoute, Router } from '@angular/router';
// import { DocumentService } from '../../services/document-service';
// import { Document } from '../../../../core/models/Document';
// import { CommonModule } from '@angular/common';

// @Component({
//   selector: 'app-document-viewer',
//   imports: [CommonModule],
//   templateUrl: './document-viewer.html',
//   styleUrl: './document-viewer.css',
// })
// export class DocumentViewer implements OnInit {
//    clientId!: number;
//   documents: Document[] = [];
//   loading = false;
//   error: string | null = null;

//   constructor(
//     private route: ActivatedRoute,
//     private router: Router,
//     private documentService: DocumentService
//   ) {}

//   ngOnInit(): void {
//     this.clientId = Number(this.route.snapshot.paramMap.get('clientId'));
//     this.loadDocuments();
//   }

//   loadDocuments(): void {
//     this.loading = true;
//     this.documentService.getDocumentsForClient(this.clientId).subscribe({
//       next: (res) => {
//         this.documents = res;
//         this.loading = false;
//       },
//       error: (err) => {
//         console.error('Error fetching documents', err);
//         this.error = 'Failed to load documents.';
//         this.loading = false;
//       },
//     });
//   }

//   openDocument(url: string): void {
//     window.open(url, '_blank');
//   }

//   goBack(): void {
//     this.router.navigate(['/bank/dashboard/clients', this.clientId]);
//   }

//   isImageFile(url: string): boolean {
//   return /\.(jpg|jpeg|png)$/i.test(url);
// }

// }










import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DocumentService } from '../../services/document-service';
import { Document } from '../../../../core/models/Document';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-document-viewer',
  imports: [CommonModule],
  templateUrl: './document-viewer.html',
  styleUrl: './document-viewer.css',
})
export class DocumentViewer implements OnInit {
  clientId!: number;
  documents: Document[] = [];
  loading = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private documentService: DocumentService
  ) {}

  ngOnInit(): void {
    this.clientId = Number(this.route.snapshot.paramMap.get('clientId'));
    this.loadDocuments();
  }

  loadDocuments(): void {
    this.loading = true;
    this.documentService.getDocumentsForClient(this.clientId).subscribe({
      next: (res) => {
        this.documents = res;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching documents', err);
        this.error = 'Failed to load documents.';
        this.loading = false;
      },
    });
  }

  openDocument(url: string): void {
    window.open(url, '_blank');
  }

  goBack(): void {
    this.router.navigate(['/bank/dashboard/clients', this.clientId]);
  }

  isImageFile(url: string): boolean {
    return /\.(jpg|jpeg|png|gif|webp)$/i.test(url);
  }

  getFileExtension(url: string): string {
    const extension = url.split('.').pop()?.toUpperCase();
    return extension || 'FILE';
  }

  getUniqueDocumentTypes(): number {
    const types = new Set(this.documents.map(doc => doc.proofTypeName));
    return types.size;
  }
}



