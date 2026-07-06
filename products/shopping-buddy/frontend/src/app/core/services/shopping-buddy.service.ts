import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
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

export interface BookingListItem {
  bookingId: string;
  buddyName: string;
  venueName: string;
  localityName: string;
  date: string;
  timeSlot: string;
  activityType: string;
  status: string;
  safetyPin: string;
}

@Injectable({ providedIn: 'root' })
export class ShoppingBuddyService {
  constructor(private readonly http: HttpClient) {}

  getLocalities(): Observable<PuneLocality[]> {
    if (!environment.useShoppingBuddyApi) {
      return of(PUNE_LOCALITIES);
    }

    return this.http.get<PuneLocality[]>(`${environment.apiUrl}/localities`).pipe(
      catchError(() => of(PUNE_LOCALITIES))
    );
  }

  getVenues(localityId?: string): Observable<PublicVenue[]> {
    if (!environment.useShoppingBuddyApi) {
      const venues = localityId
        ? PUBLIC_VENUES.filter((v) => v.localityId === localityId)
        : PUBLIC_VENUES;
      return of(venues);
    }

    let params = new HttpParams();
    if (localityId) {
      params = params.set('localityId', localityId);
    }

    return this.http.get<PublicVenue[]>(`${environment.apiUrl}/venues`, { params }).pipe(
      catchError(() => {
        const venues = localityId
          ? PUBLIC_VENUES.filter((v) => v.localityId === localityId)
          : PUBLIC_VENUES;
        return of(venues);
      })
    );
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
    if (!environment.useShoppingBuddyApi) {
      return this.searchBuddiesMock(filters);
    }

    let params = new HttpParams().set('verifiedOnly', String(filters.verifiedOnly));
    if (filters.localityId) {
      params = params.set('localityId', filters.localityId);
    }
    if (filters.venueId) {
      params = params.set('venueId', filters.venueId);
    }
    if (filters.activityType) {
      params = params.set('activityType', filters.activityType);
    }

    return this.http.get<ShoppingBuddyProfile[]>(`${environment.apiUrl}/buddies`, { params }).pipe(
      catchError(() => this.searchBuddiesMock(filters))
    );
  }

  getBuddyById(id: string): Observable<ShoppingBuddyProfile | undefined> {
    if (!environment.useShoppingBuddyApi) {
      return of(MOCK_SHOPPING_BUDDIES.find((b) => b.id === id));
    }

    return this.http.get<ShoppingBuddyProfile>(`${environment.apiUrl}/buddies/${id}`).pipe(
      catchError(() => of(MOCK_SHOPPING_BUDDIES.find((b) => b.id === id)))
    );
  }

  bookBuddy(request: BuddyBookingRequest): Observable<BuddyBookingConfirmation> {
    if (!environment.useShoppingBuddyApi) {
      return this.bookBuddyMock(request);
    }

    return this.http.post<BuddyBookingConfirmation>(`${environment.apiUrl}/bookings`, request);
  }

  getMyBookings(): Observable<BookingListItem[]> {
    return this.http.get<BookingListItem[]>(`${environment.apiUrl}/bookings/mine`);
  }

  getLocalityName(id: string): string {
    return PUNE_LOCALITIES.find((l) => l.id === id)?.name ?? id;
  }

  getVenueById(id: string): PublicVenue | undefined {
    return PUBLIC_VENUES.find((v) => v.id === id);
  }

  private searchBuddiesMock(filters: BuddySearchFilters): Observable<ShoppingBuddyProfile[]> {
    let results = [...MOCK_SHOPPING_BUDDIES];

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

    return of(results.sort((a, b) => b.rating - a.rating));
  }

  private bookBuddyMock(request: BuddyBookingRequest): Observable<BuddyBookingConfirmation> {
    const buddy = MOCK_SHOPPING_BUDDIES.find((b) => b.id === request.buddyId);
    const venue = PUBLIC_VENUES.find((v) => v.id === request.venueId);

    return of({
      bookingId: `BMSB-${Date.now().toString().slice(-6)}`,
      buddyName: buddy?.displayName ?? 'Buddy',
      venueName: venue?.name ?? 'Public venue',
      localityName: this.getLocalityName(venue?.localityId ?? ''),
      date: request.date,
      timeSlot: request.timeSlot,
      safetyPin: Math.floor(1000 + Math.random() * 9000).toString(),
      emergencyContact: '+91 98XX-XXX-XXX (demo)',
    });
  }
}
