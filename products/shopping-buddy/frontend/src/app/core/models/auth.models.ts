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

export interface JwtPayload {
  sub: string;
  email: string;
  name: string;
  role: UserRole;
  roles?: string;
  exp: number;
  iat: number;
}
