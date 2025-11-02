import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from './auth-service';
import { inject } from '@angular/core';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getToken();

 // Skip adding the token for auth endpoints (avoid recursion on login)
  if (req.url.includes('/api/Auth/login')) {
    return next(req);
  }

  if (token) {
    const clonedRequest = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
    return next(clonedRequest);
  }

  return next(req);
};
