import { Component } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-forgot-login-id',
  templateUrl: './forgot-login-id.component.html',
  styleUrls: ['../auth-shared.css', './forgot-login-id.component.css'],
})
export class ForgotLoginIdComponent {
  displayName = '';
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  constructor(private readonly auth: AuthService) {}

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.displayName.trim()) {
      this.errorMessage = 'Display name is required.';
      return;
    }

    this.isSubmitting = true;

    this.auth.forgotLoginId({ displayName: this.displayName.trim() }).subscribe({
      next: (response) => {
        this.isSubmitting = false;
        this.successMessage = response.message;
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = err?.error?.message || 'Request failed.';
      },
    });
  }
}
