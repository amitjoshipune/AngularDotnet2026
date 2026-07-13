import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['../auth-shared.css', './forgot-password.component.css'],
})
export class ForgotPasswordComponent {
  email = '';
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

    if (!this.email.trim()) {
      this.errorMessage = 'Email is required.';
      return;
    }

    this.isSubmitting = true;

    this.auth.forgotPassword({ email: this.email.trim() }).subscribe({
      next: (response) => {
        this.isSubmitting = false;
        this.successMessage = response.message;
        this.router.navigate(['/reset-password'], {
          queryParams: { email: this.email.trim() },
        });
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = err?.error?.message || 'Could not send reset code.';
      },
    });
  }
}
