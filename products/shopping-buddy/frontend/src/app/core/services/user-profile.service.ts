import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  BuddyApplyRequest,
  UpdateUserMeRequest,
  UserDocument,
  UserMe,
  VerificationStatus,
} from '../models/user-profile.models';

@Injectable({ providedIn: 'root' })
export class UserProfileService {
  private readonly baseUrl = `${environment.apiUrl}/users`;

  constructor(private readonly http: HttpClient) {}

  getMe(): Observable<UserMe> {
    return this.http.get<UserMe>(`${this.baseUrl}/me`);
  }

  updateMe(payload: UpdateUserMeRequest): Observable<UserMe> {
    return this.http.put<UserMe>(`${this.baseUrl}/me`, payload);
  }

  applyBuddy(payload: BuddyApplyRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/me/buddy-apply`, payload);
  }

  getVerificationStatus(): Observable<VerificationStatus> {
    return this.http.get<VerificationStatus>(`${this.baseUrl}/me/verification-status`);
  }

  getDocuments(): Observable<UserDocument[]> {
    return this.http.get<UserDocument[]>(`${this.baseUrl}/me/documents`);
  }

  uploadDocument(documentType: string, file: File): Observable<{ message: string; document: UserDocument }> {
    const formData = new FormData();
    formData.append('documentType', documentType);
    formData.append('file', file);
    return this.http.post<{ message: string; document: UserDocument }>(
      `${this.baseUrl}/me/documents`,
      formData
    );
  }
}
