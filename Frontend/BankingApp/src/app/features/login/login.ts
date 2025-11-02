import { Component } from '@angular/core';
import { AuthService } from '../../core/services/auth-service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  imports: [CommonModule,FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  username = '';
  password = '';
  errorMessage = '';

  constructor(private auth: AuthService, private router: Router) {}

  onSubmit() {

      this.auth.login({ username: this.username, password: this.password }).subscribe({
    next: res => {
      const role = res.role;
      if (role === 'SUPER_ADMIN') this.router.navigate(['/super-admin/dashboard']);
      else if (role === 'BANK_USER') this.router.navigate(['/bank/dashboard']);
      else if (role === 'CLIENT_USER') this.router.navigate(['/client/dashboard']);
      else this.router.navigate(['/unauthorized']);
    },
    error: err => {
      this.errorMessage = err.error?.message || 'Invalid login';
    }
  });


  }

}
