import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userId: number;
  role: string;
  message?: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = 'https://localhost:7141/api/Auth';

  constructor(private http: HttpClient, private router: Router) {}

  /** Handles login and stores JWT + user data in localStorage */
  login(data: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, data).pipe(
      tap((res) => {
        if (res.token) {
          localStorage.setItem('token', res.token);
          localStorage.setItem('role', res.role);
          localStorage.setItem('userId', res.userId.toString());
        }
      })
    );
  }

  /** Clears localStorage and navigates to login */
  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('userId');
    this.router.navigate(['/login']);
  }

  /** Retrieves token from localStorage */
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  /** Checks if user is logged in */
  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  /** Gets user role from localStorage */
  getUserRole(): string | null {
    return localStorage.getItem('role');
  }

  /** Gets user ID as a number (if exists) */
  getUserId(): number | null {
    const id = localStorage.getItem('userId');
    return id ? Number(id) : null;
  }
}
