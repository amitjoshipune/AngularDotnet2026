import { Component, OnInit } from '@angular/core';
import {
  BuddyIncomingBooking,
  RejectionReason,
  ShoppingBuddyService,
} from '../../../core/services/shopping-buddy.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-buddy-incoming',
  templateUrl: './buddy-incoming.component.html',
  styleUrls: ['./buddy-incoming.component.css'],
})
export class BuddyIncomingComponent implements OnInit {
  bookings: BuddyIncomingBooking[] = [];
  rejectionReasons: RejectionReason[] = [];
  isLoading = false;
  errorMessage = '';
  infoMessage = '';

  rejectBookingId: string | null = null;
  rejectReasonCode = '';
  rejectReasonText = '';
  isSubmitting = false;

  constructor(
    private readonly buddyService: ShoppingBuddyService,
    readonly auth: AuthService
  ) {}

  ngOnInit(): void {
    if (!this.auth.hasRole('Buddy')) {
      this.infoMessage = 'Buddy role required. Log in as meera@demo.com or anjali@demo.com.';
      return;
    }

    this.buddyService.getRejectionReasons().subscribe({
      next: (reasons) => (this.rejectionReasons = reasons),
    });
    this.loadBookings();
  }

  loadBookings(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.buddyService.getBuddyIncomingBookings().subscribe({
      next: (items) => {
        this.bookings = items;
        this.isLoading = false;
      },
      error: (err: { error?: { message?: string } }) => {
        this.isLoading = false;
        this.errorMessage = err?.error?.message || 'Could not load incoming requests.';
      },
    });
  }

  confirm(bookingId: string): void {
    this.isSubmitting = true;
    this.buddyService.confirmBooking(bookingId).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.loadBookings();
      },
      error: (err: { error?: { message?: string } }) => {
        this.isSubmitting = false;
        this.errorMessage = err?.error?.message || 'Could not confirm booking.';
      },
    });
  }

  openReject(bookingId: string): void {
    this.rejectBookingId = bookingId;
    this.rejectReasonCode = '';
    this.rejectReasonText = '';
  }

  cancelReject(): void {
    this.rejectBookingId = null;
  }

  submitReject(): void {
    if (!this.rejectBookingId || !this.rejectReasonCode) {
      return;
    }

    const selected = this.rejectionReasons.find((r) => r.code === this.rejectReasonCode);
    if (selected?.requiresText && !this.rejectReasonText.trim()) {
      this.errorMessage = 'Please type a reason for Other.';
      return;
    }

    this.isSubmitting = true;
    this.buddyService
      .rejectBooking(this.rejectBookingId, {
        reasonCode: this.rejectReasonCode,
        reasonText: this.rejectReasonText.trim() || undefined,
      })
      .subscribe({
        next: () => {
          this.isSubmitting = false;
          this.rejectBookingId = null;
          this.loadBookings();
        },
        error: (err: { error?: { message?: string } }) => {
          this.isSubmitting = false;
          this.errorMessage = err?.error?.message || 'Could not reject booking.';
        },
      });
  }

  statusClass(status: string): string {
    if (status === 'PendingBuddy') {
      return 'bg-warning text-dark';
    }
    if (status === 'Confirmed') {
      return 'bg-success';
    }
    if (status === 'RejectedByBuddy') {
      return 'bg-secondary';
    }
    return 'bg-primary';
  }

  statusLabel(status: string): string {
    if (status === 'PendingBuddy') {
      return 'Awaiting your response';
    }
    if (status === 'RejectedByBuddy') {
      return 'Rejected';
    }
    return status;
  }
}
