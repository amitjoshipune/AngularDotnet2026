import { Component, OnInit } from '@angular/core';
import { BookingListItem, ShoppingBuddyService } from '../../../core/services/shopping-buddy.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-my-bookings',
  templateUrl: './my-bookings.component.html',
  styleUrls: ['./my-bookings.component.css'],
})
export class MyBookingsComponent implements OnInit {
  bookings: BookingListItem[] = [];
  isLoading = false;
  errorMessage = '';
  infoMessage = '';

  constructor(
    private readonly buddyService: ShoppingBuddyService,
    readonly auth: AuthService
  ) {}

  ngOnInit(): void {
    if (!this.auth.hasRole('Customer')) {
      this.infoMessage =
        'Customer role required to view your trips. Use customer@demo.com or a buddy account with Customer role.';
      return;
    }

    this.loadBookings();
  }

  loadBookings(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.buddyService.getMyBookings().subscribe({
      next: (items: BookingListItem[]) => {
        this.bookings = items;
        this.isLoading = false;
      },
      error: (err: { error?: { message?: string }; status?: number }) => {
        this.isLoading = false;
        if (err?.status === 403) {
          this.infoMessage = err?.error?.message || 'Customer role required.';
          return;
        }
        this.errorMessage =
          err?.error?.message ||
          'Could not load bookings. Ensure AuthApi is running and migration 007 is applied.';
      },
    });
  }

  statusClass(status: string): string {
    if (status === 'PendingBuddy') {
      return 'bg-warning text-dark';
    }
    if (status === 'Completed' || status === 'Confirmed') {
      return 'bg-success';
    }
    if (status === 'RejectedByBuddy' || status === 'Cancelled') {
      return 'bg-secondary';
    }
    return 'bg-primary';
  }

  statusLabel(status: string): string {
    if (status === 'PendingBuddy') {
      return 'Waiting for buddy';
    }
    if (status === 'RejectedByBuddy') {
      return 'Rejected by buddy';
    }
    if (status === 'Cancelled') {
      return 'Cancelled';
    }
    return status;
  }

  canCancel(status: string): boolean {
    return status === 'PendingBuddy' || status === 'Confirmed';
  }

  cancel(bookingId: string): void {
    if (!confirm('Cancel this booking?')) {
      return;
    }

    this.buddyService.cancelBooking(bookingId).subscribe({
      next: () => this.loadBookings(),
      error: (err: { error?: { message?: string } }) => {
        this.errorMessage = err?.error?.message || 'Could not cancel booking.';
      },
    });
  }
}
