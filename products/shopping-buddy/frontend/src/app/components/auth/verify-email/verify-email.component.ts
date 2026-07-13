import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-verify-email',
  templateUrl: './verify-email.component.html',
  styleUrls: ['../auth-shared.css', './verify-email.component.css'],
})
export class VerifyEmailComponent implements OnInit {
  email = '';
  code = '';
  isSubmitting = false;
  isResending = false;
  errorMessage = '';
  infoMessage = 'In local dev, copy the 6-digit code from the API console ([DEV OTP]).';

  constructor(
    private readonly auth: AuthService,
    private readonly router: Router,
    private readonly route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.email = this.route.snapshot.queryParams['email'] || '';
  }

  onSubmit(): void {
    this.errorMessage = '';

    if (!this.email.trim() || !this.code.trim()) {
      this.errorMessage = 'Email and verification code are required.';
      return;
    }

    this.isSubmitting = true;

    this.auth.verifyEmail({ email: this.email.trim(), code: this.code.trim() }).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigateByUrl(this.auth.getHomeRoute());
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage =
          err?.error?.message || 'Invalid or expired code. Check the API console and try again.';
      },
    });
  }

  resendCode(): void {
    if (!this.email.trim()) {
      this.errorMessage = 'Enter your email first.';
      return;
    }

    this.isResending = true;
    this.errorMessage = '';

    this.auth.resendOtp({ email: this.email.trim(), purpose: 'EmailVerification' }).subscribe({
      next: (response) => {
        this.isResending = false;
        this.infoMessage = response.message;
      },
      error: (err) => {
        this.isResending = false;
        this.errorMessage = err?.error?.message || 'Could not resend code.';
      },
    });
  }
}
