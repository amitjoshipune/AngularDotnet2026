import { JwtPayload } from '../models/auth.models';

function base64UrlDecode(value: string): string {
  const base64 = value.replace(/-/g, '+').replace(/_/g, '/');
  const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), '=');
  return decodeURIComponent(
    atob(padded)
      .split('')
      .map((char) => `%${`00${char.charCodeAt(0).toString(16)}`.slice(-2)}`)
      .join('')
  );
}

export function parseJwt(token: string): JwtPayload | null {
  try {
    const payload = token.split('.')[1];
    if (!payload) {
      return null;
    }
    return JSON.parse(base64UrlDecode(payload)) as JwtPayload;
  } catch {
    return null;
  }
}

export function isTokenExpired(token: string, skewSeconds = 30): boolean {
  const payload = parseJwt(token);
  if (!payload?.exp) {
    return true;
  }
  return payload.exp * 1000 <= Date.now() + skewSeconds * 1000;
}

export function createMockJwt(payload: Omit<JwtPayload, 'iat'>): string {
  const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
  const body = btoa(
    JSON.stringify({
      ...payload,
      iat: Math.floor(Date.now() / 1000),
    })
  )
    .replace(/\+/g, '-')
    .replace(/\//g, '_')
    .replace(/=+$/, '');
  const encodedHeader = header.replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
  return `${encodedHeader}.${body}.mock-signature`;
}
