import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay, map } from 'rxjs/operators';
import {
  MOCK_SHOPPING_BUDDIES,
  PUBLIC_VENUES,
  PUNE_LOCALITIES,
  ACTIVITY_TYPES,
  SAFETY_RULES,
  VERIFICATION_STEPS,
} from '../data/mock-shopping-buddy.data';
import {
  BuddyBookingConfirmation,
  BuddyBookingRequest,
  BuddySearchFilters,
  PublicVenue,
  PuneLocality,
  ShoppingBuddyProfile,
} from '../models/shopping-buddy.models';

@Injectable({ providedIn: 'root' })
export class ShoppingBuddyService {
  getLocalities(): Observable<PuneLocality[]> {
    return of(PUNE_LOCALITIES);
  }

  getVenues(localityId?: string): Observable<PublicVenue[]> {
    const venues = localityId
      ? PUBLIC_VENUES.filter((v) => v.localityId === localityId)
      : PUBLIC_VENUES;
    return of(venues);
  }

  getActivityTypes(): string[] {
    return ACTIVITY_TYPES;
  }

  getSafetyRules(): string[] {
    return SAFETY_RULES;
  }

  getVerificationSteps(): { step: number; title: string; detail: string }[] {
    return VERIFICATION_STEPS;
  }

  searchBuddies(filters: BuddySearchFilters): Observable<ShoppingBuddyProfile[]> {
    return of(MOCK_SHOPPING_BUDDIES).pipe(
      delay(350),
      map((buddies) => {
        let results = [...buddies];

        if (filters.localityId) {
          results = results.filter((b) => b.localityId === filters.localityId);
        }

        if (filters.activityType) {
          results = results.filter((b) => b.activityTypes.includes(filters.activityType as never));
        }

        if (filters.verifiedOnly) {
          results = results.filter((b) => b.verificationLevel !== 'Basic');
        }

        if (filters.venueId) {
          results = results.filter(
            (b) => b.preferredVenueIds.includes(filters.venueId) || !filters.localityId
          );
        }

        return results.sort((a, b) => b.rating - a.rating);
      })
    );
  }

  getBuddyById(id: string): Observable<ShoppingBuddyProfile | undefined> {
    return of(MOCK_SHOPPING_BUDDIES.find((b) => b.id === id));
  }

  getLocalityName(id: string): string {
    return PUNE_LOCALITIES.find((l) => l.id === id)?.name ?? id;
  }

  getVenueById(id: string): PublicVenue | undefined {
    return PUBLIC_VENUES.find((v) => v.id === id);
  }

  bookBuddy(request: BuddyBookingRequest): Observable<BuddyBookingConfirmation> {
    const buddy = MOCK_SHOPPING_BUDDIES.find((b) => b.id === request.buddyId);
    const venue = PUBLIC_VENUES.find((v) => v.id === request.venueId);

    const confirmation: BuddyBookingConfirmation = {
      bookingId: `BMSB-${Date.now().toString().slice(-6)}`,
      buddyName: buddy?.displayName ?? 'Buddy',
      venueName: venue?.name ?? 'Public venue',
      localityName: this.getLocalityName(venue?.localityId ?? ''),
      date: request.date,
      timeSlot: request.timeSlot,
      safetyPin: Math.floor(1000 + Math.random() * 9000).toString(),
      emergencyContact: '+91 98XX-XXX-XXX (demo)',
    };

    return of(confirmation).pipe(delay(600));
  }
}
