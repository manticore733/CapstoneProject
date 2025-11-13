

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
  standalone: true, 
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

  submitting = false;
formError: string | null = null;




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
    next: res => {
      this.banks = res.filter((bank: any) => bank.isActive === true);
    },
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

 
 saveUser() {
  this.submitting = true;
  this.formError = null;

  if (this.isEdit && this.selectedUser) {
    const updatePayload = {
      userFullName: this.newUser.userFullName.trim(),
      userEmail: this.newUser.userEmail.trim(),
      userPhone: this.newUser.userPhone.trim(),
      branch: this.newUser.branch.trim(),
    };

    this.bankUserService.update(this.selectedUser.userId, updatePayload).subscribe({
      next: () => {
        this.refresh();
        alert('User updated successfully!');
      },
      error: err => {
        console.error('Error updating user', err);
        this.formError = err.error?.message || 'Failed to update user. Please try again.';
        this.submitting = false;
      },
    });
  } else {
    const createPayload = {
      userFullName: this.newUser.userFullName.trim(),
      userName: this.newUser.userName.trim(),
      password: this.newUser.password,
      userEmail: this.newUser.userEmail.trim(),
      userPhone: this.newUser.userPhone.trim(),
      bankId: this.newUser.bankId,
      branch: this.newUser.branch.trim(),
    };

    this.bankUserService.create(createPayload).subscribe({
      next: () => {
        this.refresh();
        alert('User created successfully!');
      },
      error: err => {
        console.error('Error adding user', err);
        this.formError = err.error?.message || 'Failed to create user. Username may already exist.';
        this.submitting = false;
      },
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
  this.formError = null;
  this.submitting = false;
}

  private refresh() {
    this.loadUsers();
    this.cancelForm();
  }


  
  public getBankName(bankId: number | null | undefined): string {
    if (!bankId) {
      return '—';
    }
    const bank = this.banks.find((b) => b.bankId === bankId);
    return bank ? bank.bankName : '—';
  }
}