import { AuthUser, LoginRequest, LoginResponse } from '../models/auth.models';
import { createMockJwt } from '../utils/jwt.util';

interface MockAccount extends AuthUser {
  password: string;
}

/** Demo accounts — mirrors what a .NET Auth API would validate. */
export const MOCK_ACCOUNTS: MockAccount[] = [
  {
    id: '1',
    email: 'admin@example.com',
    password: 'Admin@123',
    displayName: 'Admin User',
    role: 'Admin',
  },
  {
    id: '2',
    email: 'tester@example.com',
    password: 'Tester@123',
    displayName: 'Tester User',
    role: 'Tester',
  },
];

export function buildMockLoginResponse(credentials: LoginRequest): LoginResponse | null {
  const account = MOCK_ACCOUNTS.find(
    (entry) =>
      entry.email.toLowerCase() === credentials.email.trim().toLowerCase() &&
      entry.password === credentials.password
  );

  if (!account) {
    return null;
  }

  const expiresIn = 3600;
  const exp = Math.floor(Date.now() / 1000) + expiresIn;
  const user: AuthUser = {
    id: account.id,
    email: account.email,
    displayName: account.displayName,
    role: account.role,
  };

  return {
    accessToken: createMockJwt({
      sub: user.id,
      email: user.email,
      name: user.displayName,
      role: user.role,
      exp,
    }),
    refreshToken: `refresh-${user.id}-${Date.now()}`,
    expiresIn,
    user,
  };
}
