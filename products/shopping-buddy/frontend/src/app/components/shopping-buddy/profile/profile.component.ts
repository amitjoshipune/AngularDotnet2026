import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';
import { UserProfileService } from '../../../core/services/user-profile.service';
import { ShoppingBuddyService } from '../../../core/services/shopping-buddy.service';
import {
  BuddyApplyRequest,
  UpdateUserMeRequest,
  UserAddress,
  UserMe,
  VerificationStatus,
} from '../../../core/models/user-profile.models';
import { PuneLocality } from '../../../core/models/shopping-buddy.models';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
})
export class ProfileComponent implements OnInit {
  profile: UserMe | null = null;
  verification: VerificationStatus | null = null;
  localities: PuneLocality[] = [];

  isLoading = true;
  isSaving = false;
  isApplying = false;
  isUploading = false;
  errorMessage = '';
  successMessage = '';

  displayName = '';
  phone = '';
  dateOfBirth = '';
  gender = '';
  emergencyContactName = '';
  emergencyContactPhone = '';
  bio = '';
  addresses: UserAddress[] = [];

  buddyLocalityId = '';
  buddyBio = '';
  buddyNotes = '';

  selectedDocType = 'Aadhaar';
  readonly documentTypes = ['Aadhaar', 'AddressProof', 'PAN'];

  constructor(
    readonly auth: AuthService,
    private readonly profileService: UserProfileService,
    private readonly buddyService: ShoppingBuddyService
  ) {}

  ngOnInit(): void {
    this.buddyService.getLocalities().subscribe({
      next: (items) => (this.localities = items),
    });
    this.loadAll();
  }

  loadAll(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.profileService.getMe().subscribe({
      next: (profile) => {
        this.profile = profile;
        this.bindForm(profile);
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage =
          err?.error?.message ||
          'Could not load profile. Run SQL migrations 010 and 011, then restart the API.';
      },
    });

    this.profileService.getVerificationStatus().subscribe({
      next: (status) => (this.verification = status),
    });
  }

  private bindForm(profile: UserMe): void {
    this.displayName = profile.displayName;
    this.phone = profile.phone || '';
    this.dateOfBirth = profile.dateOfBirth || '';
    this.gender = profile.gender || '';
    this.emergencyContactName = profile.emergencyContactName || '';
    this.emergencyContactPhone = profile.emergencyContactPhone || '';
    this.bio = profile.bio || '';
    this.addresses = profile.addresses?.length
      ? [...profile.addresses]
      : [{ label: 'Home', line1: '', city: 'Pune', state: 'Maharashtra', pincode: '', isDefault: true }];
    this.buddyLocalityId = profile.buddyLocalityId || '';
    this.buddyBio = profile.bio || '';
  }

  addAddress(): void {
    this.addresses.push({
      label: 'Other',
      line1: '',
      city: 'Pune',
      state: 'Maharashtra',
      pincode: '',
      isDefault: false,
    });
  }

  removeAddress(index: number): void {
    if (this.addresses.length > 1) {
      this.addresses.splice(index, 1);
    }
  }

  saveProfile(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.isSaving = true;

    const payload: UpdateUserMeRequest = {
      displayName: this.displayName.trim(),
      phone: this.phone.trim() || undefined,
      dateOfBirth: this.dateOfBirth || undefined,
      gender: this.gender || undefined,
      emergencyContactName: this.emergencyContactName.trim() || undefined,
      emergencyContactPhone: this.emergencyContactPhone.trim() || undefined,
      bio: this.bio.trim() || undefined,
      addresses: this.addresses.filter((a) => a.line1.trim()),
    };

    this.profileService.updateMe(payload).subscribe({
      next: (profile) => {
        this.profile = profile;
        this.isSaving = false;
        this.successMessage = 'Profile saved.';
      },
      error: (err) => {
        this.isSaving = false;
        this.errorMessage = err?.error?.message || 'Could not save profile.';
      },
    });
  }

  applyBuddy(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.isApplying = true;

    const payload: BuddyApplyRequest = {
      localityId: this.buddyLocalityId,
      bio: this.buddyBio.trim(),
      notes: this.buddyNotes.trim() || undefined,
    };

    this.profileService.applyBuddy(payload).subscribe({
      next: (response) => {
        this.isApplying = false;
        this.successMessage = response.message;
        this.loadAll();
      },
      error: (err) => {
        this.isApplying = false;
        this.errorMessage = err?.error?.message || 'Could not submit application.';
      },
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) {
      return;
    }

    this.isUploading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.profileService.uploadDocument(this.selectedDocType, file).subscribe({
      next: (response) => {
        this.isUploading = false;
        this.successMessage = response.message;
        input.value = '';
        this.profileService.getVerificationStatus().subscribe({
          next: (status) => (this.verification = status),
        });
      },
      error: (err) => {
        this.isUploading = false;
        this.errorMessage = err?.error?.message || 'Upload failed.';
        input.value = '';
      },
    });
  }

  docStatusClass(status: string): string {
    if (status === 'Verified') {
      return 'bg-success';
    }
    if (status === 'Rejected') {
      return 'bg-danger';
    }
    return 'bg-warning text-dark';
  }
}
