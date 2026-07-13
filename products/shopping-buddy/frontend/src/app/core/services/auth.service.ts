import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AuthUser,
  ForgotLoginIdRequest,
  ForgotPasswordRequest,
  LoginRequest,
  LoginResponse,
  MessageResponse,
  RegisterRequest,
  RegisterResponse,
  ResendOtpRequest,
  ResetPasswordRequest,
  UserRole,
  VerifyEmailRequest,
} from '../models/auth.models';
import { isTokenExpired } from '../utils/jwt.util';
import { TokenStorageService } from './token-storage.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly currentUserSubject = new BehaviorSubject<AuthUser | null>(null);
  readonly currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private readonly http: HttpClient,
    private readonly tokenStorage: TokenStorageService
  ) {
    this.restoreSession();
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, credentials).pipe(
      tap((response) => this.applySession(response))
    );
  }

  register(payload: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${environment.apiUrl}/auth/register`, payload);
  }

  verifyEmail(payload: VerifyEmailRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/verify-email`, payload).pipe(
      tap((response) => this.applySession(response))
    );
  }

  resendOtp(payload: ResendOtpRequest): Observable<MessageResponse> {
    return this.http.post<MessageResponse>(`${environment.apiUrl}/auth/resend-otp`, payload);
  }

  forgotPassword(payload: ForgotPasswordRequest): Observable<MessageResponse> {
    return this.http.post<MessageResponse>(`${environment.apiUrl}/auth/forgot-password`, payload);
  }

  resetPassword(payload: ResetPasswordRequest): Observable<MessageResponse> {
    return this.http.post<MessageResponse>(`${environment.apiUrl}/auth/reset-password`, payload);
  }

  forgotLoginId(payload: ForgotLoginIdRequest): Observable<MessageResponse> {
    return this.http.post<MessageResponse>(`${environment.apiUrl}/auth/forgot-login-id`, payload);
  }

  logout(): void {
    this.tokenStorage.clear();
    this.currentUserSubject.next(null);
  }

  isAuthenticated(): boolean {
    const token = this.tokenStorage.getAccessToken();
    return !!token && !isTokenExpired(token);
  }

  getAccessToken(): string | null {
    return this.tokenStorage.getAccessToken();
  }

  getCurrentUser(): AuthUser | null {
    return this.currentUserSubject.value;
  }

  getRoles(): UserRole[] {
    const user = this.getCurrentUser();
    if (!user) {
      return [];
    }
    if (user.roles?.length) {
      return user.roles;
    }
    return user.role ? [user.role] : [];
  }

  hasRole(role: UserRole): boolean {
    return this.getRoles().includes(role);
  }

  getUserRole(): UserRole | null {
    return this.getCurrentUser()?.role ?? null;
  }

  getRolesLabel(): string {
    return this.getRoles().join(', ');
  }

  getHomeRoute(): string {
    if (this.hasRole('Admin')) {
      return '/admin';
    }
    return '/shopping';
  }

  private restoreSession(): void {
    const token = this.tokenStorage.getAccessToken();
    const user = this.tokenStorage.getUser();

    if (token && user && !isTokenExpired(token)) {
      this.normalizeUser(user);
      this.currentUserSubject.next(user);
      return;
    }

    this.tokenStorage.clear();
    this.currentUserSubject.next(null);
  }

  private applySession(response: LoginResponse): void {
    if (!response.accessToken || !response.user) {
      return;
    }

    this.normalizeUser(response.user);
    this.tokenStorage.saveSession(
      response.accessToken,
      response.refreshToken,
      response.user
    );
    this.currentUserSubject.next(response.user);
  }

  private normalizeUser(user: AuthUser): void {
    if (!user.roles?.length && user.role) {
      user.roles = [user.role];
    }
  }
}
