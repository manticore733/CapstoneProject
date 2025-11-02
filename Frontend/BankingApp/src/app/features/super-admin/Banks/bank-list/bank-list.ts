import { Component, OnInit } from '@angular/core';
import { BankService } from '../bank-service';
import { Bank } from '../../../../core/models/Bank';
import { CommonModule } from '@angular/common';

import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-bank-list',
  imports: [CommonModule,FormsModule],
  templateUrl: './bank-list.html',
  styleUrl: './bank-list.css',
})
export class BankList implements OnInit {


 banks: Bank[] = [];
  showForm = false;
  isEdit = false;
  selectedBank: Bank | null = null;
  newBank: any = {};

  constructor(private bankService: BankService) {}

  ngOnInit() {
    this.loadBanks();
  }

  loadBanks() {
    this.bankService.getAll().subscribe({
      next: res => (this.banks = res),
      error: err => console.error('Error fetching banks', err),
    });
  }

  addBank() {
    this.isEdit = false;
    this.showForm = true;
    this.newBank = {};
  }

  editBank(bank: Bank) {
    this.isEdit = true;
    this.showForm = true;
    this.selectedBank = bank;
    this.newBank = {
      bankName: bank.bankName,
      ifsc: bank.ifsc,
    };
  }

  saveBank() {
    if (this.isEdit && this.selectedBank) {
      const updatePayload = {
        bankName: this.newBank.bankName,
        ifsc: this.newBank.ifsc,
      };
      this.bankService.update(this.selectedBank.bankId, updatePayload).subscribe({
        next: () => this.refresh(),
        error: err => console.error('Error updating bank', err),
      });
    } else {
      const createPayload = {
        bankName: this.newBank.bankName,
        ifsc: this.newBank.ifsc,
        establishmentDate: this.newBank.establishmentDate,
      };
      this.bankService.create(createPayload).subscribe({
        next: () => this.refresh(),
        error: err => console.error('Error adding bank', err),
      });
    }
  }

  deleteBank(id: number) {
    if (confirm('Are you sure you want to delete this bank?')) {
      this.bankService.delete(id).subscribe({
        next: () => this.refresh(),
        error: err => console.error('Error deleting bank', err),
      });
    }
  }

  cancelForm() {
    this.showForm = false;
    this.isEdit = false;
    this.selectedBank = null;
    this.newBank = {};
  }

  private refresh() {
    this.loadBanks();
    this.cancelForm();
  }




}
