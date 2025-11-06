// import { CanActivateFn, Router } from '@angular/router';
// import { AuthService } from '../services/auth-service';
// import { inject } from '@angular/core';

// export const roleGuard: CanActivateFn = (route, state) => {
//   const auth = inject(AuthService);
//   const router = inject(Router);
//   const expectedRole = route.data['role'];

//   const userRole = auth.getUserRole();

//   if (userRole === expectedRole) {
//     return true;
//   } else {
//     router.navigate(['/unauthorized']);
//     return false;
//   }
// };




import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth-service';
import { inject } from '@angular/core';

export const roleGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const expectedRole = route.data['role'] as string;
  const userRole = auth.getUserRole();

  // 🔍 DEBUG: Log everything
  console.log('========== ROLE GUARD CHECK ==========');
  console.log('Current URL:', state.url);
  console.log('Expected Role:', expectedRole);
  console.log('User Role from localStorage:', userRole);
  console.log('Token:', auth.getToken() ? '✅ Token exists' : '❌ No token');
  console.log('Is Logged In:', auth.isLoggedIn());
  console.log('Role Match:', userRole === expectedRole);
  console.log('========== END CHECK ==========');

  if (userRole === expectedRole) {
    return true;
  } else {
    console.warn('❌ ROLE GUARD REJECTED - Redirecting to /unauthorized');
    router.navigate(['/unauthorized']);
    return false;
  }
};
