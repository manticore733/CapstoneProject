import { Routes } from '@angular/router';
import { Login } from './features/login/login';
import { Dashboard} from './features/dashboard/dashboard';
import { authGuard } from './core/guards/auth-guard';
import { SuperAdmin } from './features/super-admin/dashboard/super-admin/super-admin';  
import { roleGuard } from './core/guards/role-guard';
import { BankUserDashboard } from './features/bank-user/dashboard/bank-user-dashboard/bank-user-dashboard';
import { ClientUser } from './features/client-user/dashboard/client-user/client-user';
import { Unauthorized } from './pages/unauthorized/unauthorized/unauthorized';
import { BankList } from './features/super-admin/Banks/bank-list/bank-list';
import { BankUserList } from './features/super-admin/Banks/bank-user-list/bank-user-list';


export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },

  
 // Super Admin dashboard
  // {
  //   path: 'super-admin/dashboard',
  //   component: SuperAdmin,
  //   canActivate: [authGuard, roleGuard],
  //   data: { role: 'SUPER_ADMIN' },
  // },

  {
  path: 'super-admin/dashboard',
  component: SuperAdmin,
  canActivate: [authGuard, roleGuard],
  data: { role: 'SUPER_ADMIN' },
  children: [
    { path: '', redirectTo: 'banks', pathMatch: 'full' },
    {
      path: 'banks',
     component: BankList,
    },
  ],
},





  // Bank User dashboard
  {
    path: 'bank/dashboard',
     component: BankUserDashboard,
    canActivate: [authGuard, roleGuard],
    data: { role: 'BANK_USER' },
  },

  // Client User dashboard
  {
    path: 'client/dashboard',
   component:ClientUser,
    canActivate: [authGuard, roleGuard],
    data: { role: 'CLIENT_USER' },
  },

  {
  path: 'super-admin/bank-users',
  component: BankUserList,
  canActivate: [authGuard, roleGuard],
  data: { role: 'SUPER_ADMIN' },
},




  // Unauthorized page
  {
    path: 'unauthorized',
    component: Unauthorized
  },
];
