import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ShoppingBuddyService } from '../../../core/services/shopping-buddy.service';
import {
  BuddyBookingConfirmation,
  PublicVenue,
  ShoppingBuddyProfile,
} from '../../../core/models/shopping-buddy.models';

@Component({
  selector: 'app-book-buddy',
  templateUrl: './book-buddy.component.html',
  styleUrls: ['./book-buddy.component.css'],
})
export class BookBuddyComponent implements OnInit {
  buddy: ShoppingBuddyProfile | undefined;
  venues: PublicVenue[] = [];
  safetyRules: string[] = [];
  form: FormGroup;
  isSubmitting = false;
  confirmation: BuddyBookingConfirmation | null = null;

  readonly timeSlots = ['10:00 AM – 12:00 PM', '12:00 PM – 2:00 PM', '3:00 PM – 5:00 PM', '5:00 PM – 7:00 PM'];

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly buddyService: ShoppingBuddyService,
    private readonly fb: FormBuilder
  ) {
    this.form = this.fb.group({
      venueId: ['', Validators.required],
      date: ['', Validators.required],
      timeSlot: ['', Validators.required],
      activityType: ['Shopping', Validators.required],
      notes: [''],
      acceptedSafetyRules: [false, Validators.requiredTrue],
      shareLiveLocation: [true],
    });
  }

  ngOnInit(): void {
    const buddyId = this.route.snapshot.paramMap.get('id');
    if (!buddyId) {
      this.router.navigate(['/shopping/find']);
      return;
    }

    this.safetyRules = this.buddyService.getSafetyRules();

    this.buddyService.getBuddyById(buddyId).subscribe((buddy) => {
      if (!buddy) {
        this.router.navigate(['/shopping/find']);
        return;
      }
      this.buddy = buddy;
      this.buddyService.getVenues(buddy.localityId).subscribe((v) => {
        this.venues = v.filter((venue) => buddy.preferredVenueIds.includes(venue.id));
        if (this.venues.length === 1) {
          this.form.patchValue({ venueId: this.venues[0].id });
        }
      });
    });
  }

  submit(): void {
    if (this.form.invalid || !this.buddy) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const value = this.form.value;

    this.buddyService
      .bookBuddy({
        buddyId: this.buddy.id,
        venueId: value.venueId,
        date: value.date,
        timeSlot: value.timeSlot,
        activityType: value.activityType,
        notes: value.notes,
        acceptedSafetyRules: value.acceptedSafetyRules,
        shareLiveLocation: value.shareLiveLocation,
      })
      .subscribe({
        next: (result) => {
          this.confirmation = result;
          this.isSubmitting = false;
        },
        error: () => (this.isSubmitting = false),
      });
  }

  localityName(id: string): string {
    return this.buddyService.getLocalityName(id);
  }
}
