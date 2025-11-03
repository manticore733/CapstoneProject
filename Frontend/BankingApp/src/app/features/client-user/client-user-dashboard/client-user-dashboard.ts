import { Component } from '@angular/core';
import { AuthService } from '../../../core/services/auth-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-client-user-dashboard',
  imports: [],
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
