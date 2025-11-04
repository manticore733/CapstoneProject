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
import { AllDocuments } from './features/bank-user/components/all-documents/all-documents';
import { BeneficiaryListComponent } from './features/client-user/components/beneficiary-list-component/beneficiary-list-component';
import { EmployeeListComponent } from './features/client-user/components/employee-list-component/employee-list-component';
import { MakePaymentComponent } from './features/client-user/components/make-payment-component/make-payment-component';








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
     {
        path: 'all-documents', // ← Add this new route
        component: AllDocuments
      },
    ],
  },

  // Client User dashboard
  {
    path: 'client/dashboard',
   component:ClientUserDashboard,
    canActivate: [authGuard, roleGuard],
    data: { role: 'CLIENT_USER' },
    children: [
      { path: '', redirectTo: 'beneficiaries', pathMatch: 'full' },
      { 
        path: 'beneficiaries', 
        component: BeneficiaryListComponent 
      },
      // We will add routes for employees, payments, etc. here later
      { 
        path: 'employees', 
        component: EmployeeListComponent 
      },
      { 
        path: 'make-payment', 
        component: MakePaymentComponent 
      },
    ]
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










