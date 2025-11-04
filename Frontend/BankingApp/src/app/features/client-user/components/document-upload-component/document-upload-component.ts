import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DocumentUploadService } from '../../services/document-upload-service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Document } from '../../../../core/models/Document';


@Component({
  selector: 'app-document-upload-component',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './document-upload-component.html',
  styleUrl: './document-upload-component.css',
})
export class DocumentUploadComponent {

  form!: FormGroup;
  selectedFile: File | null = null;
  
  myDocuments: Document[] = [];
  
  loading = false;
  loadingList = false;
  error: string | null = null;
  success: string | null = null;

  // From your ProofType.cs enum (0, 1, 2, 3)
  proofTypes = [
    { id: 0, name: 'Business Registration' },
    { id: 1, name: 'Tax ID Proof' },
    { id: 2, name: 'Proof of Address' },
    { id: 3, name: 'Other' }
  ];

  constructor(
    private fb: FormBuilder,
    private documentService: DocumentUploadService // Using the new service name
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      proofTypeId: [null, [Validators.required]],
      file: [null, [Validators.required]],
    });

    this.loadMyDocuments();
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
        // Don't show an error, just an empty list
      },
    });
  }

  onFileChange(event: any): void {
    const file = event.target.files?.[0];
    if (file) {
      this.selectedFile = file;
    }
  }

  submitUpload(): void {
    if (this.form.invalid || !this.selectedFile) {
      this.form.markAllAsTouched();
      this.error = "Please select a document type and a file.";
      return;
    }

    this.loading = true;
    this.error = null;
    this.success = null;

    const proofTypeId = +this.form.value.proofTypeId;

    this.documentService.uploadDocument(proofTypeId, this.selectedFile).subscribe({
      next: (response) => {
        this.loading = false;
        this.success = `Successfully uploaded: ${response.documentName}`;
        this.form.reset();
        this.selectedFile = null;
        // Reset the file input visually
        (document.getElementById('file-input') as HTMLInputElement).value = "";
        
        // Refresh the list
        this.loadMyDocuments();
      },
      error: (err) => {
        console.error('Error uploading document', err);
        this.loading = false;
        this.error = err.error?.message || 'An unknown error occurred during upload.';
      },
    });
  }

}
