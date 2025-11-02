import { Component } from '@angular/core';
import { AuthService } from '../../../../core/services/auth-service';
import { Router, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { BankList } from "../../Banks/bank-list/bank-list";

@Component({
  selector: 'app-super-admin',
  standalone: true,
  imports: [CommonModule,RouterOutlet],
  templateUrl: './super-admin.html',
  styleUrl: './super-admin.css',
})
export class SuperAdmin {
 

  sidebarOpen = true;

  constructor(private router: Router, private auth: AuthService) {}

  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  logout() {
    this.auth.logout();
  }

  navigateTo(path: string) {
    this.router.navigate([path]);
  }

}
