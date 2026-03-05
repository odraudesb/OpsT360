import { Injectable } from '@angular/core';
import axios, {
  AxiosError,
  AxiosHeaders,
  AxiosInstance,
  AxiosRequestConfig,
  AxiosResponse,
  InternalAxiosRequestConfig,
} from 'axios';
import { from, Observable } from 'rxjs';
import { environment } from '../../../enviroments/enviroment';
import { STORAGE_KEYS } from '../../core/constants/storage-keys';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly client: AxiosInstance;

  private readonly publicEndpoints: RegExp[] = [
    /(^|\/)(api\/)?auth\/login(\?|$)/i,
    /(^|\/)(api\/)?auth\/register(\?|$)/i,
    /(^|\/)(api\/)?auth\/forgot(\?|$)/i,
    /(^|\/)(api\/)?auth\/refresh(\?|$)/i,
  ];

  constructor() {
    this.client = axios.create({
      baseURL: environment.apiBaseUrl,
      timeout: 15000,
    });

    this.client.interceptors.request.use(
      (config: InternalAxiosRequestConfig): InternalAxiosRequestConfig => {
        const fullUrl = this.buildFullUrl(config);

        if (this.isPublicEndpoint(fullUrl)) {
          this.removeAuthHeader(config);
          return config;
        }

        const token = this.readTokenFromStorage();
        if (token) this.setAuthHeader(config, token);
        else this.removeAuthHeader(config);

        return config;
      },
      (error: AxiosError) => Promise.reject(error),
    );

    this.client.interceptors.response.use(
      (response: AxiosResponse) => response,
      (error: AxiosError) => Promise.reject(error),
    );
  }

  private buildFullUrl(config: InternalAxiosRequestConfig): string {
    const base = String(config.baseURL ?? this.client.defaults.baseURL ?? '');
    const url = String(config.url ?? '');
    if (!base) return url;
    if (!url) return base;
    return `${base.replace(/\/+$/, '')}/${url.replace(/^\/+/, '')}`;
  }

  private isPublicEndpoint(url: string): boolean {
    const u = (url || '').toLowerCase();
    return this.publicEndpoints.some((re) => re.test(u));
  }

  private readTokenFromStorage(): string | null {
    const raw = localStorage.getItem(STORAGE_KEYS.TOKEN);
    if (!raw) return null;

    let token: string | null = raw;

    try {
      const parsed = JSON.parse(raw);
      if (typeof parsed === 'string') token = parsed;
      else if (parsed && typeof parsed.token === 'string') token = parsed.token;
    } catch { }

    token = token?.trim() ?? null;
    return token && token.length ? token : null;
  }

  private setAuthHeader(config: InternalAxiosRequestConfig, token: string): void {
    if (!config.headers) config.headers = new AxiosHeaders();

    if (config.headers instanceof AxiosHeaders) {
      config.headers.set('Authorization', `Bearer ${token}`);
    } else {
      (config.headers as any).Authorization = `Bearer ${token}`;
    }
  }

  private removeAuthHeader(config: InternalAxiosRequestConfig): void {
    if (!config.headers) return;

    if (config.headers instanceof AxiosHeaders) {
      config.headers.delete('Authorization');
    } else {
      delete (config.headers as any).Authorization;
    }
  }

  get<TResponse>(url: string, config?: AxiosRequestConfig): Observable<TResponse> {
    return from(this.client.get<TResponse>(url, config).then((res) => res.data));
  }

  post<TRequest, TResponse>(url: string, body?: TRequest, config?: AxiosRequestConfig): Observable<TResponse> {
    return from(this.client.post<TResponse>(url, body, config).then((res) => res.data));
  }

  put<TRequest, TResponse>(url: string, body?: TRequest, config?: AxiosRequestConfig): Observable<TResponse> {
    return from(this.client.put<TResponse>(url, body, config).then((res) => res.data));
  }

  delete<TResponse>(url: string, config?: AxiosRequestConfig): Observable<TResponse> {
    return from(this.client.delete<TResponse>(url, config).then((res) => res.data));
  }
}
