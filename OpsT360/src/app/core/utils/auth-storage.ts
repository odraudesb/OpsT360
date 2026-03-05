
import { STORAGE_KEYS } from '../constants/storage-keys';
import type { AuthContext } from '../../interfaces/auth/auth-context.interface';
import type { LoginResponse } from '../../interfaces/auth/login-response.interface';

const TOKEN_KEY = STORAGE_KEYS.TOKEN;

/**
 * Intenta obtener el token desde varias estructuras posibles del LoginResponse
 * (por compatibilidad con distintas respuestas del backend).
 */
export function extractTokenFromLoginResponse(response: LoginResponse): string | null {
  const tokenFromData = typeof response.data === 'string' ? response.data : response.data?.token;

  const token = tokenFromData ?? response.token;
  return typeof token === 'string' && token.trim().length ? token.trim() : null;
}

export function saveAuthToken(token: string): void {
  localStorage.setItem(TOKEN_KEY, token);
}

export function readAuthToken(): string | null {
  const raw = localStorage.getItem(TOKEN_KEY);
  if (!raw) return null;

  try {
    const parsed = JSON.parse(raw);

    // A veces guardan directo el string, o un objeto {token: "..."}
    if (typeof parsed === 'string') return parsed;
    if (parsed && typeof (parsed as { token?: unknown }).token === 'string') {
      return (parsed as { token: string }).token;
    }
  } catch {
    // si no era JSON, devolvemos el raw
  }

  return raw;
}

export function clearAuthToken(): void {
  localStorage.removeItem(TOKEN_KEY);
}

export function getAuthContextFromToken(token: string | null): AuthContext | null {
  if (!token) return null;

  const payload = decodeJwtPayload(token);
  if (!payload) return null;

  const rawUserId = readNumberClaim(payload, ['userId', 'userID', 'sub']);
  const rawCompanyId = readNumberClaim(payload, ['companyId', 'companyID', 'company']);

  if (rawUserId == null || rawCompanyId == null) return null;

  const userId = Number(rawUserId);
  const companyId = Number(rawCompanyId);

  if (!Number.isFinite(userId) || !Number.isFinite(companyId) || userId <= 0 || companyId <= 0) {
    return null;
  }

  const name = readStringClaim(payload, ['name']);
  return { userId, companyId, name: name ?? undefined };
}

export function getAuthContextFromStorage(): AuthContext | null {
  return getAuthContextFromToken(readAuthToken());
}

/**
 * Decodifica el payload de un JWT (sin verificar firma).
 */
function decodeJwtPayload(token: string): Record<string, unknown> | null {
  try {
    const part = token.split('.')[1];
    if (!part) return null;

    const json = decodeBase64Url(part);
    const parsed = JSON.parse(json);
    return typeof parsed === 'object' && parsed !== null ? (parsed as Record<string, unknown>) : null;
  } catch {
    return null;
  }
}

function decodeBase64Url(value: string): string {
  const base64 = value.replace(/-/g, '+').replace(/_/g, '/');
  const pad = base64.length % 4 === 0 ? '' : '='.repeat(4 - (base64.length % 4));
  const raw = atob(base64 + pad);
  const bytes = Uint8Array.from(raw, (char) => char.charCodeAt(0));
  if (typeof TextDecoder !== 'undefined') {
    return new TextDecoder().decode(bytes);
  }
  return decodeURIComponent(
    Array.from(bytes)
      .map((byte) => `%${byte.toString(16).padStart(2, '0')}`)
      .join(''),
  );
}

function readNumberClaim(payload: Record<string, unknown>, keys: string[]): number | null {
  for (const key of keys) {
    const value = payload[key];
    if (typeof value === 'number' && Number.isFinite(value)) return value;

    if (typeof value === 'string') {
      const num = Number(value);
      if (Number.isFinite(num)) return num;
    }
  }
  return null;
}

function readStringClaim(payload: Record<string, unknown>, keys: string[]): string | null {
  for (const key of keys) {
    const value = payload[key];
    if (typeof value === 'string' && value.trim().length) return value.trim();
  }
  return null;
}
