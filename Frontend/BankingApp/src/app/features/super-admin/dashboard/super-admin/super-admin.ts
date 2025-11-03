// import { Component } from '@angular/core';
// import { AuthService } from '../../../../core/services/auth-service';
// import { Router, RouterOutlet } from '@angular/router';
// import { CommonModule } from '@angular/common';
// import { BankList } from "../../Banks/bank-list/bank-list";

// @Component({
//   selector: 'app-super-admin',
//   standalone: true,
//   imports: [CommonModule,RouterOutlet],
//   templateUrl: './super-admin.html',
//   styleUrl: './super-admin.css',
// })
// export class SuperAdmin {
 

//   sidebarOpen = true;

//   constructor(private router: Router, private auth: AuthService) {}

//   toggleSidebar() {
//     this.sidebarOpen = !this.sidebarOpen;
//   }

//   logout() {
//     this.auth.logout();
//   }

//   navigateTo(path: string) {
//     this.router.navigate([path]);
//   }

// }





import { Component } from '@angular/core';
import { AuthService } from '../../../../core/services/auth-service';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-super-admin',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './super-admin.html',
  styleUrl: './super-admin.css',
})
export class SuperAdmin {
  sidebarOpen = true;
  currentRoute = '';

  constructor(private router: Router, private auth: AuthService) {
    // Track current route for active state
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.currentRoute = event.url;
      });
  }

  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  logout() {
    this.auth.logout();
  }

  navigateTo(path: string) {
    this.router.navigate([path]);
  }

  // isActive(route: string): boolean {
  //   return this.currentRoute.includes(route);
  // }

  isActive(route: string): boolean {
  return this.currentRoute.includes(route);
}



}