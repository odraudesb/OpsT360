const FALLBACK_IP = '127.0.0.1';

function resolveClientIpFromBrowserHost(): string {
  if (typeof window === 'undefined') {
    return FALLBACK_IP;
  }

  const host = window.location.hostname?.trim();
  if (!host) {
    return FALLBACK_IP;
  }

  if (host === 'localhost' || host === '::1' || host === '[::1]') {
    return FALLBACK_IP;
  }

  return host;
}

export const DEFAULT_DEVICE_INFO = {
  get ip(): string {
    return resolveClientIpFromBrowserHost();
  },
  device: 'web' as const,
};

