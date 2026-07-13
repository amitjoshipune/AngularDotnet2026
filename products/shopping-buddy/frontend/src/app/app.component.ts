import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  constructor(
    readonly auth: AuthService,
    private readonly router: Router
  ) {}

  isLoginRoute(): boolean {
    return this.router.url.startsWith('/login');
  }

  showNavbar(): boolean {
    const publicAuthRoutes = ['/login', '/register', '/verify-email', '/forgot-password', '/reset-password', '/forgot-login-id'];
    return !publicAuthRoutes.some((route) => this.router.url.startsWith(route));
  }

  isAdmin(): boolean {
    return this.auth.hasRole('Admin');
  }

  isCustomer(): boolean {
    return this.auth.hasRole('Customer');
  }

  isBuddy(): boolean {
    return this.auth.hasRole('Buddy');
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
