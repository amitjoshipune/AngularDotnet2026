import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  email = '';
  password = '';
  isSubmitting = false;
  errorMessage = '';
  returnUrl = '';

  readonly demoAccounts = [
    { label: 'Customer (Sunita)', email: 'customer@demo.com', password: 'Customer@123' },
    { label: 'Customer (Senior)', email: 'senior@demo.com', password: 'Senior@123' },
    { label: 'Buddy (Meera)', email: 'meera@demo.com', password: 'Buddy@123' },
    { label: 'Admin', email: 'admin@example.com', password: 'Admin@123' },
  ];

  constructor(
    private readonly auth: AuthService,
    private readonly router: Router,
    private readonly route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '';
  }

  fillDemo(account: { email: string; password: string }): void {
    this.email = account.email;
    this.password = account.password;
    this.errorMessage = '';
  }

  onSubmit(): void {
    this.errorMessage = '';

    if (!this.email.trim() || !this.password) {
      this.errorMessage = 'Email and password are required.';
      return;
    }

    this.isSubmitting = true;

    this.auth.login({ email: this.email.trim(), password: this.password }).subscribe({
      next: () => {
        this.isSubmitting = false;
        const target = this.resolveReturnUrl();
        this.router.navigateByUrl(target);
      },
      error: (err) => {
        this.isSubmitting = false;
        const message = err?.error?.message || err?.message || '';
        if (err?.status === 0) {
          this.errorMessage =
            'The authentication API is currently unavailable. Start AuthApi (dotnet run) and ensure the database scripts have been run.';
        } else if (err?.status === 503) {
          this.errorMessage = message || 'Database is unavailable. Run products/shopping-buddy/database/run-all.bat first.';
        } else if (message) {
          this.errorMessage = message;
        } else {
          this.errorMessage = 'Login failed. Check your credentials and try again.';
        }
      },
    });
  }

  private resolveReturnUrl(): string {
    if (this.returnUrl && this.returnUrl !== '/login') {
      return this.returnUrl;
    }
    return this.auth.getHomeRoute();
  }
}
