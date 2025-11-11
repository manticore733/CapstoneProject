import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth-service';
import { jwtDecode } from 'jwt-decode';

export const clientStatusGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const token = auth.getToken();

  if (!token) {
    router.navigate(['/login']);
    return false;
  }

  const decoded: any = jwtDecode(token);

  // handle both formats of role claim
  const role =
    decoded['Role'] ||
    decoded['role'] ||
    decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
  const status = decoded['Status'] || decoded['status'];

  console.log('=== CLIENT STATUS GUARD ===');
  console.log('Decoded:', decoded);
  console.log('Role:', role, 'Status:', status);
  console.log('URL:', state.url);
  console.log('============================');

  //  If rejected client, only allow upload page
  if (role === 'CLIENT_USER' && status === 'REJECTED') {
    if (!state.url.includes('/client/dashboard/upload-documents')) {
      console.warn('🚫 REJECTED CLIENT - Redirecting to upload-documents');
      router.navigate(['/client/dashboard/upload-documents']);
      return false;
    }
  }

  return true;
};
