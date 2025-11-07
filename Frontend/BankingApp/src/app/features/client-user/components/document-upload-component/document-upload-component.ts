// import { Component } from '@angular/core';
// import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
// import { DocumentUploadService } from '../../services/document-upload-service';
// import { CommonModule } from '@angular/common';
// import { ReactiveFormsModule } from '@angular/forms';
// import { Document } from '../../../../core/models/Document';

// @Component({
//   selector: 'app-document-upload-component',
//   imports: [CommonModule, ReactiveFormsModule,FormsModule],
//   templateUrl: './document-upload-component.html',
//   styleUrl: './document-upload-component.css',
// })
// export class DocumentUploadComponent {
//   form!: FormGroup;
//   selectedFile: File | null = null;
//   myDocuments: Document[] = [];
  
//   loading = false;
//   loadingList = false;

//   accountStatus: number | null = null;

  
//   // Toast notifications
//   showSuccessToast = false;
//   showErrorToast = false;
//   successMessage = '';
//   errorMessage = '';

//   proofTypes = [
//     { id: 0, name: 'Business Registration' },
//     { id: 1, name: 'Tax ID Proof' },
//     { id: 2, name: 'Proof of Address' },
//     { id: 3, name: 'Other' }
//   ];

//   constructor(
//     private fb: FormBuilder,
//     private documentService: DocumentUploadService
//   ) {}

//   ngOnInit(): void {
//     this.form = this.fb.group({
//       proofTypeId: [null, [Validators.required]],
//       file: [null, [Validators.required]],
//     });

//     const token = localStorage.getItem('token');
//   if (token) {
//     try {
//       // if it's a normal object, not JWT
//       const parsed = JSON.parse(atob(token.split('.')[1] || '{}'));
//       this.accountStatus = parsed?.Status ?? null;
//     } catch {
//       try {
//         const parsed = JSON.parse(token);
//         this.accountStatus = parsed?.status ?? null;
//       } catch {
//         console.warn('Could not parse token');
//       }
//     }
//   }

//     this.loadMyDocuments();
//   }

//   loadMyDocuments(): void {
//     this.loadingList = true;
//     this.documentService.getMyDocuments().subscribe({
//       next: (data) => {
//         this.myDocuments = data;
//         this.loadingList = false;
//       },
//       error: (err) => {
//         console.error('Error fetching documents', err);
//         this.loadingList = false;
//       },
//     });
//   }

//   onFileChange(event: any): void {
//     const file = event.target.files?.[0];
//     if (file) {
//       this.selectedFile = file;
//       this.form.patchValue({ file: file });
//     }
//   }

//   submitUpload(): void {
//     if (this.form.invalid || !this.selectedFile) {
//       this.form.markAllAsTouched();
//       this.errorMessage = "Please select a document type and a file.";
//       this.showErrorToast = true;
//       setTimeout(() => this.showErrorToast = false, 5000);
//       return;
//     }

//     this.loading = true;

//     const proofTypeId = +this.form.value.proofTypeId;

//     this.documentService.uploadDocument(proofTypeId, this.selectedFile).subscribe({
//       next: (response) => {
//         this.loading = false;
//         this.successMessage = `Successfully uploaded: ${response.documentName}`;
//         this.showSuccessToast = true;
//         this.resetForm();
//         this.loadMyDocuments();
        
//         setTimeout(() => this.showSuccessToast = false, 5000);
//       },
//       error: (err) => {
//         console.error('Error uploading document', err);
//         this.loading = false;
//         this.errorMessage = err.error?.message || 'An unknown error occurred during upload.';
//         this.showErrorToast = true;
        
//         setTimeout(() => this.showErrorToast = false, 5000);
//       },
//     });
//   }

//   replaceDocument(doc: Document): void {
//     // Pre-fill the form with the document type
//     this.form.patchValue({ proofTypeId: this.getProofTypeId(doc.proofTypeName) });
//     this.successMessage = `Ready to replace: ${doc.documentName}. Select a new file and click Upload.`;
//     this.showSuccessToast = true;
//     setTimeout(() => this.showSuccessToast = false, 3000);
    
//     // Scroll to upload form
//     window.scrollTo({ top: 0, behavior: 'smooth' });
//   }

//   getProofTypeId(proofTypeName: string): number {
//     const proof = this.proofTypes.find(p => p.name === proofTypeName);
//     return proof ? proof.id : 0;
//   }

//   resetForm(): void {
//     this.form.reset();
//     this.selectedFile = null;
//     (document.getElementById('file-input') as HTMLInputElement).value = "";
//   }

//   closeSuccessToast(): void {
//     this.showSuccessToast = false;
//   }

//   closeErrorToast(): void {
//     this.showErrorToast = false;
//   }
// }





import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { DocumentUploadService } from '../../services/document-upload-service';
import { ClientUserService } from '../../services/client-user-service';  // ← NEW
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Document } from '../../../../core/models/Document';
import { ClientUser } from '../../../../core/models/ClientUser';  // ← NEW

@Component({
  selector: 'app-document-upload-component',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './document-upload-component.html',
  styleUrl: './document-upload-component.css',
})
export class DocumentUploadComponent {
  form!: FormGroup;
  selectedFile: File | null = null;
  myDocuments: Document[] = [];
  clientUser: ClientUser | null = null;  // ← NEW: Store client user data
  
  loading = false;
  loadingList = false;
  loadingProfile = false;  // ← NEW
  
  showSuccessToast = false;
  showErrorToast = false;
  successMessage = '';
  errorMessage = '';

  proofTypes = [
    { id: 0, name: 'Business Registration' },
    { id: 1, name: 'Tax ID Proof' },
    { id: 2, name: 'Proof of Address' },
    { id: 3, name: 'Other' }
  ];

  constructor(
    private fb: FormBuilder,
    private documentService: DocumentUploadService,
    private clientUserService: ClientUserService  // ← NEW
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      proofTypeId: [null, [Validators.required]],
      file: [null, [Validators.required]],
    });

    this.loadClientProfile();  // ← NEW: Load profile on init
    this.loadMyDocuments();
  }

  // ← NEW: Load client user profile
  loadClientProfile(): void {
    this.loadingProfile = true;
    this.clientUserService.getMyProfile().subscribe({
      next: (data) => {
        this.clientUser = data;
        console.log('Client User Profile:', this.clientUser);  // Debug
        this.loadingProfile = false;
      },
      error: (err) => {
        console.error('Error fetching profile', err);
        this.loadingProfile = false;
      },
    });
  }

  loadMyDocuments(): void {
    this.loadingList = true;
    this.documentService.getMyDocuments().subscribe({
      next: (data) => {
        this.myDocuments = data;
        this.loadingList = false;
      },
      error: (err) => {
        console.error('Error fetching documents', err);
        this.loadingList = false;
      },
    });
  }

  onFileChange(event: any): void {
    const file = event.target.files?.[0];
    if (file) {
      this.selectedFile = file;
      this.form.patchValue({ file: file });
    }
  }

  submitUpload(): void {
    if (this.form.invalid || !this.selectedFile) {
      this.form.markAllAsTouched();
      this.errorMessage = "Please select a document type and a file.";
      this.showErrorToast = true;
      setTimeout(() => this.showErrorToast = false, 5000);
      return;
    }

    this.loading = true;
    const proofTypeId = +this.form.value.proofTypeId;

    this.documentService.uploadDocument(proofTypeId, this.selectedFile).subscribe({
      next: (response) => {
        this.loading = false;
        this.successMessage = `Successfully uploaded: ${response.documentName}`;
        this.showSuccessToast = true;
        this.resetForm();
        this.loadMyDocuments();
        
        setTimeout(() => this.showSuccessToast = false, 5000);
      },
      error: (err) => {
        console.error('Error uploading document', err);
        this.loading = false;
        this.errorMessage = err.error?.message || 'An unknown error occurred during upload.';
        this.showErrorToast = true;
        
        setTimeout(() => this.showErrorToast = false, 5000);
      },
    });
  }

  replaceDocument(doc: Document): void {
    this.form.patchValue({ proofTypeId: this.getProofTypeId(doc.proofTypeName) });
    this.successMessage = `Ready to replace: ${doc.documentName}. Select a new file and click Upload.`;
    this.showSuccessToast = true;
    setTimeout(() => this.showSuccessToast = false, 3000);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  getProofTypeId(proofTypeName: string): number {
    const proof = this.proofTypes.find(p => p.name === proofTypeName);
    return proof ? proof.id : 0;
  }

  resetForm(): void {
    this.form.reset();
    this.selectedFile = null;
    (document.getElementById('file-input') as HTMLInputElement).value = "";
  }

  closeSuccessToast(): void {
    this.showSuccessToast = false;
  }

  closeErrorToast(): void {
    this.showErrorToast = false;
  }
}
