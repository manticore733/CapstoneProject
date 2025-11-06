// import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
// import { inject } from '@angular/core';
// import { Router } from '@angular/router';
// import { AuthService } from './auth-service';
// import { catchError, throwError } from 'rxjs';

// export const errorInterceptor: HttpInterceptorFn = (req, next) => {
//   const router = inject(Router);
//   const authService = inject(AuthService);

//   return next(req).pipe(
//     catchError((error: HttpErrorResponse) => {
//       console.error('HTTP Error:', error);

//       // Handle 401 Unauthorized (Token Expired)
//       if (error.status === 401) {
//         console.warn('Token expired or unauthorized');
//         authService.logout(); // This will clear localStorage and navigate to login
//         return throwError(() => new Error('Session expired. Please login again.'));
//       }

//       // Handle 403 Forbidden (Access Denied)
//       if (error.status === 403) {
//         console.warn('Access forbidden');
//         router.navigate(['/unauthorized']);
//         return throwError(() => new Error('You do not have permission to access this resource.'));
//       }

//       // Handle 404 Not Found
//       if (error.status === 404) {
//         console.warn('Resource not found');
//         return throwError(() => new Error('Resource not found.'));
//       }

//       // Handle 500 Server Error
//       if (error.status === 500) {
//         console.error('Server error');
//         return throwError(() => new Error('Server error. Please try again later.'));
//       }

//       // Return other errors as is
//       return throwError(() => error);
//     })
//   );
// };




import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './auth-service';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      console.error('HTTP Error:', error);
      console.error('Error Status:', error.status);
      console.error('Error Message:', error.message);

      // Handle 401 Unauthorized (Token Expired)
      if (error.status === 401) {
        console.warn('🚨 Token expired or unauthorized');
        authService.logout(); // This will clear localStorage and navigate to login
        return throwError(() => new Error('Session expired. Please login again.'));
      }

      // Handle 403 Forbidden (Access Denied)
      if (error.status === 403) {
        console.warn('🚨 Access forbidden - you don\'t have permission');
        router.navigate(['/unauthorized']);
        return throwError(() => new Error('You do not have permission to access this resource.'));
      }

      // Handle 404 Not Found
      if (error.status === 404) {
        console.warn('⚠️ Resource not found');
        return throwError(() => new Error('Resource not found.'));
      }

      // Handle 500 Server Error
      if (error.status === 500) {
        console.error('🚨 Server error');
        return throwError(() => new Error('Server error. Please try again later.'));
      }

      // Return other errors as is
      return throwError(() => error);
    })
  );
};
