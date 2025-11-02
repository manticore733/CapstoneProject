import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth-service';

@Component({
  selector: 'app-bank-user-dashboard',
  imports: [],
  templateUrl: './bank-user-dashboard.html',
  styleUrl: './bank-user-dashboard.css',
})
export class BankUserDashboard {


     userId = localStorage.getItem('userId');
  role = localStorage.getItem('role');

  constructor(private auth: AuthService, private router: Router) {}

  logout() {
    this.auth.logout();
  }


}
