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
  buddyApplicationStatus: string;
  buddyApplicationNotes?: string;
  buddyLocalityId?: string;
  addresses: UserAddress[];
}

export interface UpdateUserMeRequest {
  displayName: string;
  phone?: string;
  dateOfBirth?: string;
  gender?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  bio?: string;
  addresses: UserAddress[];
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
}
