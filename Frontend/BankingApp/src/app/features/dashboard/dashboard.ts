import { Component } from '@angular/core';
import { AuthService } from '../../core/services/auth-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {

  username = localStorage.getItem('userId');
  role = localStorage.getItem('role');

  constructor(private auth: AuthService, private router: Router) {}

  logout() {
    this.auth.logout();
  }

}
