import { Component } from '@angular/core';
import { AuthService } from '../../../core/services/auth-service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-client-user-dashboard',
  imports: [CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive],
  templateUrl: './client-user-dashboard.html',
  styleUrl: './client-user-dashboard.css',
})
export class ClientUserDashboard {
    userId = localStorage.getItem('userId');
  role = localStorage.getItem('role');

  constructor(private auth: AuthService, private router: Router) {}

  logout() {
    this.auth.logout();
  }

}
