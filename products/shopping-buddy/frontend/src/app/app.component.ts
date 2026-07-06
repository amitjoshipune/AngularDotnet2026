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
    return !this.isLoginRoute();
  }

  isAdmin(): boolean {
    return this.auth.getUserRole() === 'Admin';
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
