import { Component, OnInit } from '@angular/core';
import { ShoppingBuddyService } from '../../../core/services/shopping-buddy.service';
import {
  BuddySearchFilters,
  PublicVenue,
  PuneLocality,
  ShoppingBuddyProfile,
} from '../../../core/models/shopping-buddy.models';

@Component({
  selector: 'app-find-buddy',
  templateUrl: './find-buddy.component.html',
  styleUrls: ['./find-buddy.component.css'],
})
export class FindBuddyComponent implements OnInit {
  localities: PuneLocality[] = [];
  venues: PublicVenue[] = [];
  activityTypes: string[] = [];
  buddies: ShoppingBuddyProfile[] = [];
  isLoading = false;

  filters: BuddySearchFilters = {
    localityId: '',
    venueId: '',
    activityType: '',
    verifiedOnly: true,
  };

  constructor(private readonly buddyService: ShoppingBuddyService) {}

  ngOnInit(): void {
    this.buddyService.getLocalities().subscribe((l) => (this.localities = l));
    this.activityTypes = this.buddyService.getActivityTypes();
    this.search();
  }

  onLocalityChange(): void {
    this.filters.venueId = '';
    this.buddyService.getVenues(this.filters.localityId || undefined).subscribe((v) => (this.venues = v));
    this.search();
  }

  search(): void {
    this.isLoading = true;
    this.buddyService.searchBuddies(this.filters).subscribe({
      next: (results) => {
        this.buddies = results;
        this.isLoading = false;
      },
      error: () => (this.isLoading = false),
    });
  }

  localityName(id: string): string {
    return this.buddyService.getLocalityName(id);
  }

  verificationLabel(level: string): string {
    if (level === 'PoliceVerified') {
      return 'Police verified';
    }
    if (level === 'AadhaarVerified') {
      return 'Aadhaar verified';
    }
    return 'Basic';
  }

  verificationClass(level: string): string {
    if (level === 'PoliceVerified') {
      return 'bg-success';
    }
    if (level === 'AadhaarVerified') {
      return 'bg-primary';
    }
    return 'bg-secondary';
  }
}
