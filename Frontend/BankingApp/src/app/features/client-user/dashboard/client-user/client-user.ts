import { Component } from '@angular/core';
import { AuthService } from '../../../../core/services/auth-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-client-user',
  imports: [],
  templateUrl: './client-user.html',
  styleUrl: './client-user.css',
})
export class ClientUser {
  userId = localStorage.getItem('userId');
  role = localStorage.getItem('role');

  constructor(private auth: AuthService, private router: Router) {}

  logout() {
    this.auth.logout();
  }

}
