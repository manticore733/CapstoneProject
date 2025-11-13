import { Component, ViewChild } from '@angular/core'; // Added ViewChild
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RecaptchaModule, RecaptchaComponent } from 'ng-recaptcha'; //Added RecaptchaComponent
import { AuthService } from '../../core/services/auth-service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RecaptchaModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  username = '';
  password = '';
  captchaToken: string | null = null;
  errorMessage = '';

  // Add a reference to the reCAPTCHA component
  @ViewChild(RecaptchaComponent) captchaRef!: RecaptchaComponent;

  constructor(private auth: AuthService, private router: Router) {}

  onCaptchaResolved(token: string | null) {
  this.captchaToken = token;
}


  onSubmit() {
    if (!this.captchaToken) {
      this.errorMessage = 'Please verify the captcha before logging in.';
      return;
    }

    this.auth.login({
      username: this.username,
      password: this.password,
      captchaToken: this.captchaToken,
    }).subscribe({
      next: res => {
        const role = res.role;
        if (role === 'SUPER_ADMIN') this.router.navigate(['/super-admin/dashboard']);
        else if (role === 'BANK_USER') this.router.navigate(['/bank/dashboard']);
        else if (role === 'CLIENT_USER') this.router.navigate(['/client/dashboard']);
        else this.router.navigate(['/unauthorized']);
      },
      error: err => {
        this.errorMessage = err.error?.message || 'Invalid login';

        // Reset CAPTCHA if login fails
        if (this.captchaRef) {
          this.captchaRef.reset();
        }
        this.captchaToken = null;
      }
    });
  }
}
