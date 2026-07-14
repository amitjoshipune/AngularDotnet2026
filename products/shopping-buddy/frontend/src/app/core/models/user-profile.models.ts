export interface UserAddress {
  addressId?: number;
  label: string;
  line1: string;
  line2?: string;
  city: string;
  state: string;
  pincode: string;
  isDefault: boolean;
}

export interface UserMe {
  id: string;
  email: string;
  displayName: string;
  roles: string[];
  phone?: string;
  dateOfBirth?: string;
  gender?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  bio?: string;
  profilePhotoUrl?: string;
  buddyApplicationStatus: string;
  buddyApplicationNotes?: string;
  buddyLocalityId?: string;
  addresses: UserAddress[];
}

export interface UpdateUserMeRequest {
  updateScope?: 'basic' | 'advanced' | 'all';
  displayName: string;
  phone?: string;
  dateOfBirth?: string;
  gender?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  bio?: string;
  profilePhotoUrl?: string;
  addresses?: UserAddress[];
}

export interface BuddyApplyRequest {
  localityId: string;
  bio: string;
  notes?: string;
}

export interface UserDocument {
  documentId: number;
  documentType: string;
  fileName: string;
  status: string;
  uploadedAt: string;
  verifiedAt?: string;
  rejectionReason?: string;
}

export interface VerificationStatus {
  canAcceptBookings: boolean;
  missingDocuments: string[];
  documents: UserDocument[];
  note?: string;
}

export interface ProfilePhotoUploadResponse {
  message: string;
  profilePhotoUrl: string;
  profile: UserMe;
}
