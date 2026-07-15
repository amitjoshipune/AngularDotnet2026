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
  isSavingBasic = false;
  isSavingAdvanced = false;
  isApplying = false;
  isUploadingPhoto = false;
  isUploadingDoc = false;
  showAdvanced = false;
  errorMessage = '';
  successMessage = '';

  displayName = '';
  phone = '';
  dateOfBirth = '';
  gender = '';
  emergencyContactName = '';
  emergencyContactPhone = '';
  bio = '';
  profilePhotoUrl = '';
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
          'Could not load profile. Run SQL migrations 010–012 in SSMS, then restart the API.';
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
    this.profilePhotoUrl = profile.profilePhotoUrl || '';
    this.addresses = profile.addresses?.length ? [...profile.addresses] : [];
    this.buddyLocalityId = profile.buddyLocalityId || '';
    this.buddyBio = profile.bio || '';
    this.showAdvanced =
      !!profile.dateOfBirth ||
      !!profile.gender ||
      !!profile.emergencyContactName ||
      !!profile.emergencyContactPhone ||
      (profile.addresses?.length ?? 0) > 0;
  }

  photoSrc(): string | null {
    if (!this.profilePhotoUrl) {
      return null;
    }
    if (this.profilePhotoUrl.startsWith('http')) {
      return this.profilePhotoUrl;
    }
    return this.profilePhotoUrl;
  }

  addAddress(): void {
    this.addresses.push({
      label: 'Home',
      line1: '',
      city: 'Pune',
      state: 'Maharashtra',
      pincode: '',
      isDefault: false,
    });
  }

  removeAddress(index: number): void {
    this.addresses.splice(index, 1);
  }

  canSaveBasic(): boolean {
    return this.displayName.trim().length > 0;
  }

  saveBasic(): void {
    this.clearMessages();
    this.isSavingBasic = true;

    const payload: UpdateUserMeRequest = {
      updateScope: 'basic',
      displayName: this.displayName.trim(),
      phone: this.phone.trim() || undefined,
      bio: this.bio.trim() || undefined,
    };

    this.profileService.updateMe(payload).subscribe({
      next: (profile) => {
        this.profile = profile;
        this.displayName = profile.displayName;
        this.isSavingBasic = false;
        this.successMessage = 'Basic profile saved.';
      },
      error: (err) => {
        this.isSavingBasic = false;
        this.errorMessage = err?.error?.message || 'Could not save basic profile.';
      },
    });
  }

  saveAdvanced(): void {
    this.clearMessages();
    this.isSavingAdvanced = true;

    const payload: UpdateUserMeRequest = {
      updateScope: 'advanced',
      displayName: this.displayName.trim() || 'User',
      dateOfBirth: this.dateOfBirth || undefined,
      gender: this.gender || undefined,
      emergencyContactName: this.emergencyContactName.trim() || undefined,
      emergencyContactPhone: this.emergencyContactPhone.trim() || undefined,
      addresses: this.addresses.filter((a) => a.line1.trim()),
    };

    this.profileService.updateMe(payload).subscribe({
      next: (profile) => {
        this.profile = profile;
        this.bindForm(profile);
        this.isSavingAdvanced = false;
        this.successMessage = 'Advanced details saved.';
      },
      error: (err) => {
        this.isSavingAdvanced = false;
        this.errorMessage = err?.error?.message || 'Could not save advanced details.';
      },
    });
  }

  onPhotoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) {
      return;
    }

    this.clearMessages();
    this.isUploadingPhoto = true;

    this.profileService.uploadProfilePhoto(file).subscribe({
      next: (response) => {
        this.isUploadingPhoto = false;
        this.profile = response.profile;
        this.profilePhotoUrl = response.profilePhotoUrl;
        this.successMessage = response.message;
        input.value = '';
      },
      error: (err) => {
        this.isUploadingPhoto = false;
        this.errorMessage = err?.error?.message || 'Photo upload failed.';
        input.value = '';
      },
    });
  }

  applyBuddy(): void {
    this.clearMessages();
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

    this.clearMessages();
    this.isUploadingDoc = true;

    this.profileService.uploadDocument(this.selectedDocType, file).subscribe({
      next: (response) => {
        this.isUploadingDoc = false;
        this.successMessage = response.message;
        input.value = '';
        this.profileService.getVerificationStatus().subscribe({
          next: (status) => (this.verification = status),
        });
      },
      error: (err) => {
        this.isUploadingDoc = false;
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

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}
