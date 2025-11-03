import { Routes } from '@angular/router';
import { Login } from './features/login/login';
import { Dashboard} from './features/dashboard/dashboard';
import { authGuard } from './core/guards/auth-guard';
import { SuperAdmin } from './features/super-admin/dashboard/super-admin/super-admin';  
import { roleGuard } from './core/guards/role-guard';
import { BankUserDashboard } from './features/bank-user/dashboard/bank-user-dashboard/bank-user-dashboard';
import { ClientUserDashboard } from './features/client-user/client-user-dashboard/client-user-dashboard';
import { Unauthorized } from './pages/unauthorized/unauthorized/unauthorized';
import { BankList } from './features/super-admin/Banks/bank-list/bank-list';
import { BankUserList } from './features/super-admin/Banks/bank-user-list/bank-user-list';
import { ClientList } from './features/bank-user/components/client-list/client-list';
import { ClientDetails } from './features/bank-user/components/client-details/client-details';
import { DocumentViewer } from './features/bank-user/components/document-viewer/document-viewer';
import { ClientForm } from './features/bank-user/components/client-form/client-form';


export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },

  
 
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
    //added route for bank users
     { path: 'bank-users', component: BankUserList }, 
  ],
},





  // Bank User dashboard
  {
    path: 'bank/dashboard',
     component: BankUserDashboard,
    canActivate: [authGuard, roleGuard],
    data: { role: 'BANK_USER' },
      children: [
      { path: '', redirectTo: 'clients', pathMatch: 'full' },
      { path: 'clients', component: ClientList },
     { path: 'clients/:id', component: ClientDetails },

      { path: 'documents/:clientId', component: DocumentViewer },
          { path: 'client/add', component: ClientForm },
    { path: 'client/edit/:id', component: ClientForm },
    ],
  },

  // Client User dashboard
  {
    path: 'client/dashboard',
   component:ClientUserDashboard,
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










