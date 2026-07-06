import { Component, OnInit } from '@angular/core';
import { BookingListItem, ShoppingBuddyService } from '../../../core/services/shopping-buddy.service';

@Component({
  selector: 'app-my-bookings',
  templateUrl: './my-bookings.component.html',
  styleUrls: ['./my-bookings.component.css'],
})
export class MyBookingsComponent implements OnInit {
  bookings: BookingListItem[] = [];
  isLoading = false;
  errorMessage = '';

  constructor(private readonly buddyService: ShoppingBuddyService) {}

  ngOnInit(): void {
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
      error: (err: { error?: { message?: string } }) => {
        this.isLoading = false;
        this.errorMessage =
          err?.error?.message ||
          'Could not load bookings. Sign in as a customer and ensure the API database is set up.';
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
