

import { Component } from '@angular/core';
import { Bank } from '../../../../core/models/Bank';
import { BankUser } from '../../../../core/models/BankUser';
import { BankUserService } from '../bank-user-service';
import { BankService } from '../bank-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CreateBankUser } from '../../../../core/models/CreateBankUser';

@Component({
  selector: 'app-bank-user-list',
  standalone: true, // <-- YOU WERE MISSING THIS
  imports: [CommonModule, FormsModule],
  templateUrl: './bank-user-list.html',
  styleUrl: './bank-user-list.css',
})
export class BankUserList {
  bankUsers: BankUser[] = [];
  banks: Bank[] = [];

  showForm = false;
  isEdit = false;
  selectedUser: BankUser | null = null;


// --- FIX 1: Changed type to 'any' ---
  // This allows the object to hold 'CreateBankUser' fields when adding
  // and 'BankUser' fields (like 'isActive' and 'branch') when editing.
  newUser: any = {};

  constructor(
    private bankUserService: BankUserService,
    private bankService: BankService
  ) {}

  ngOnInit() {
    this.loadUsers();
    this.loadBanks();
  }

  loadUsers() {
    this.bankUserService.getAll().subscribe({
      next: res => (this.bankUsers = res),
      error: err => console.error('Error fetching bank users', err),
    });
  }

  loadBanks() {
    this.bankService.getAll().subscribe({
      next: res => (this.banks = res),
      error: err => console.error('Error fetching banks', err),
    });
  }

  addUser() {
    this.isEdit = false;
    this.showForm = true;
    this.newUser = {};
  }

  editUser(user: BankUser) {
    this.isEdit = true;
    this.showForm = true;
    this.selectedUser = user;
    this.newUser = { ...user };
  }

  // saveUser() {
  //   if (this.isEdit && this.selectedUser) {
  //     this.bankUserService.update(this.selectedUser.userId, this.newUser).subscribe({
  //       next: () => this.refresh(),
  //       error: err => console.error('Error updating user', err),
  //     });
  //   } else {
  //     this.bankUserService.create(this.newUser as CreateBankUser).subscribe({
  //       next: () => this.refresh(),
  //       error: err => console.error('Error adding user', err),
  //     });
  //   }
  // }

  saveUser() {
  if (this.isEdit && this.selectedUser) {
    const updatePayload = {
      userFullName: this.newUser.userFullName,
      userEmail: this.newUser.userEmail,
      userPhone: this.newUser.userPhone,
      branch: this.newUser.branch,
    };

    this.bankUserService.update(this.selectedUser.userId, updatePayload).subscribe({
      next: () => this.refresh(),
      error: err => console.error('Error updating user', err),
    });
  } else {
    const createPayload = {
      userFullName: this.newUser.userFullName,
      userName: this.newUser.userName,
      password: this.newUser.password,
      userEmail: this.newUser.userEmail,
      userPhone: this.newUser.userPhone,
      bankId: this.newUser.bankId,
      branch: this.newUser.branch,
    };

    this.bankUserService.create(createPayload).subscribe({
      next: () => this.refresh(),
      error: err => console.error('Error adding user', err),
    });
  }
}





  deleteUser(id: number) {
    if (confirm('Are you sure you want to delete this user?')) {
      this.bankUserService.delete(id).subscribe({
        next: () => this.refresh(),
        error: err => console.error('Error deleting user', err),
      });
    }
  }

  cancelForm() {
    this.showForm = false;
    this.newUser = {};
    this.selectedUser = null;
  }

  private refresh() {
    this.loadUsers();
    this.cancelForm();
  }


  // --- FIX 2: Added the helper function ---
  // This function is for your HTML to safely find the bank name.
  public getBankName(bankId: number | null | undefined): string {
    if (!bankId) {
      return '—';
    }
    const bank = this.banks.find((b) => b.bankId === bankId);
    return bank ? bank.bankName : '—';
  }
}