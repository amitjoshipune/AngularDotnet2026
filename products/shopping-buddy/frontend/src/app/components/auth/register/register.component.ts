import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['../auth-shared.css', './register.component.css'],
})
export class RegisterComponent {
  email = '';
  password = '';
  confirmPassword = '';
  displayName = '';
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private readonly auth: AuthService,
    private readonly router: Router
  ) {}

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.email.trim() || !this.password || !this.displayName.trim()) {
      this.errorMessage = 'All fields are required.';
      return;
    }

    if (this.password.length < 8) {
      this.errorMessage = 'Password must be at least 8 characters.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    this.isSubmitting = true;

    this.auth
      .register({
        email: this.email.trim(),
        password: this.password,
        displayName: this.displayName.trim(),
      })
      .subscribe({
        next: (response) => {
          this.isSubmitting = false;
          this.successMessage = response.message;
          this.router.navigate(['/verify-email'], {
            queryParams: { email: response.email },
          });
        },
        error: (err) => this.handleError(err),
      });
  }

  private handleError(err: { status?: number; error?: { message?: string }; message?: string }): void {
    this.isSubmitting = false;
    const message = err?.error?.message || err?.message || '';
    if (err?.status === 0) {
      this.errorMessage = 'The authentication API is unavailable. Start AuthApi (dotnet run) first.';
    } else if (err?.status === 503) {
      this.errorMessage = message || 'Database is unavailable. Run database/run-all.bat first.';
    } else if (message) {
      this.errorMessage = message;
    } else {
      this.errorMessage = 'Registration failed. Please try again.';
    }
  }
}
