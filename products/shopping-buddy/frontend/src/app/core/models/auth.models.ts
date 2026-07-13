export type UserRole = 'Customer' | 'Buddy' | 'Admin';

export interface AuthUser {
  id: string;
  email: string;
  displayName: string;
  role: UserRole;
  roles?: UserRole[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: AuthUser;
}

export interface RegisterRequest {
  email: string;
  password: string;
  displayName: string;
}

export interface RegisterResponse {
  message: string;
  email: string;
  requiresVerification: boolean;
}

export interface VerifyEmailRequest {
  email: string;
  code: string;
}

export interface ResendOtpRequest {
  email: string;
  purpose: 'EmailVerification' | 'PasswordReset' | 'ForgotLoginId';
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  code: string;
  newPassword: string;
}

export interface ForgotLoginIdRequest {
  displayName: string;
}

export interface MessageResponse {
  message: string;
}

export interface JwtPayload {
  sub: string;
  email: string;
  name: string;
  role: UserRole;
  roles?: string;
  exp: number;
  iat: number;
}
