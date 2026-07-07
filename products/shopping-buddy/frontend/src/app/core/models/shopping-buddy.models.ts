export type VerificationLevel = 'Basic' | 'AadhaarVerified' | 'PoliceVerified';

export type BuddyActivityType = 'Shopping' | 'MallVisit' | 'MarketVisit' | 'GroceryRun';

export interface PuneLocality {
  id: string;
  name: string;
  zone: 'Pune City' | 'Pune Metro' | 'PCMC';
}

export interface PublicVenue {
  id: string;
  name: string;
  localityId: string;
  type: 'Mall' | 'Market' | 'Supermarket';
}

export interface ShoppingBuddyProfile {
  id: string;
  displayName: string;
  age: number;
  localityId: string;
  languages: string[];
  verificationLevel: VerificationLevel;
  rating: number;
  completedTrips: number;
  bio: string;
  activityTypes: BuddyActivityType[];
  availableToday: boolean;
  preferredVenueIds: string[];
  /** Same-gender companion only — enforced at platform level. */
  gender: 'Female';
  avatarUrl?: string;
}

export interface BuddySearchFilters {
  localityId: string;
  venueId: string;
  activityType: BuddyActivityType | '';
  verifiedOnly: boolean;
}

export interface BuddyBookingRequest {
  buddyId: string;
  venueId: string;
  date: string;
  timeSlot: string;
  activityType: BuddyActivityType;
  notes: string;
  acceptedSafetyRules: boolean;
  shareLiveLocation: boolean;
}

export interface BuddyBookingConfirmation {
  bookingId: string;
  buddyName: string;
  venueName: string;
  localityName: string;
  date: string;
  timeSlot: string;
  safetyPin: string;
  status?: string;
  emergencyContact: string;
}
