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
    const role = this.auth.getUserRole();
    if (role && role !== 'Customer') {
      this.infoMessage =
        'You are signed in as ' +
        role +
        '. Bookings belong to Customer accounts. Use customer@demo.com or senior@demo.com to view trips.';
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
          this.infoMessage = err?.error?.message || 'Customer account required.';
          return;
        }
        this.errorMessage =
          err?.error?.message ||
          'Could not load bookings. Ensure AuthApi is running and you are logged in as a customer.';
      },
    });
  }

  statusClass(status: string): string {
    if (status === 'Completed') {
      return 'bg-success';
    }
    if (status === 'Cancelled') {
      return 'bg-secondary';
    }
    return 'bg-primary';
  }
}
