import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

import { ApiService } from '../api/api.service';
import { DEFAULT_DEVICE_INFO } from '../../core/constants/device-info';
import { getAuthContextFromStorage } from '../../core/utils/auth-storage';
import { environment } from '../../../enviroments/enviroment';

import type { ApiResponse, TransactionDto } from '../../interfaces/transactions/transaction.interface';
import type {
  TransactionDetailsDto,
  TransactionDetailsWithFileRequestDto,
} from '../../interfaces/transactions/transaction-details.interface';
import type { HistoricalTransactionDetailsDto } from '../../interfaces/transactions/historical-transaction-details.interface';
import type { ReeferTransactionDetailsDto } from '../../interfaces/transactions/reefer-transaction-details.interface';
import type { SecurityTransactionDetailsDto } from '../../interfaces/transactions/security-transaction-details.interface';

type AuthBodyBase = {
  companyId: string | number;
  userId: string | number;
  ip: string;
  device: string;
};

type AlertMailPayload = {
  to: string;
  subject: string;
  body: string;
  format: 'HTML';
  cc: string;
  bcc: string;
};

@Injectable({ providedIn: 'root' })
export class TransactionsService {
  private readonly alertMailEndpoint = 'emails';
  private readonly alertMailTo = 'rmurillo@infraportus.com';
  private readonly alertMailCc = 'edu1991e@gmail.com';

  constructor(private readonly api: ApiService) { }

  private logRequestPayload(endpoint: string, payload: unknown): void {
    console.log(`[TransactionsService] payload -> ${endpoint}`, payload);
  }

  private buildAuthBody<TExtra extends Record<string, unknown> = Record<string, never>>(
    extra: TExtra = {} as TExtra,
  ): AuthBodyBase & TExtra {
    const ctx = getAuthContextFromStorage();
    if (!ctx) throw new Error('Missing auth context (token sin companyId/userId o no logueado)');

    return {
      companyId: ctx.companyId,
      userId: ctx.userId,
      ip: DEFAULT_DEVICE_INFO.ip,
      device: DEFAULT_DEVICE_INFO.device,
      ...extra,
    };
  }

  getAllTransactions(extra: Record<string, unknown> = {}): Observable<TransactionDto[]> {
    const body = this.buildAuthBody(extra);
    this.logRequestPayload('transactions', body);

    return this.api
      .post<typeof body, ApiResponse<TransactionDto[]>>('transactions', body)
      .pipe(map((res) => res?.data ?? []));
  }

  getTransactionDetails(transactionId: number, extra: Record<string, unknown> = {}): Observable<TransactionDetailsDto> {
    const body = this.buildAuthBody({ transactionId, ...extra });
    this.logRequestPayload('transactions/details', body);

    return this.api
      .post<typeof body, ApiResponse<TransactionDetailsDto>>('transactions/details', body)
      .pipe(map((res) => res.data));
  }

  getAlertDetails(payload: { entityId: number }, extra: Record<string, unknown> = {}): Observable<unknown[]> {
    const body = this.buildAuthBody({ ...payload, ...extra });
    this.logRequestPayload('transactions/alert-details', body);

    return this.api
      .post<typeof body, ApiResponse<unknown[]>>('transactions/alert-details', body)
      .pipe(map((res) => res?.data ?? []));
  }

  getHistoricalTransactionDetails(
    payload: { entityId: number },
    extra: Record<string, unknown> = {},
  ): Observable<HistoricalTransactionDetailsDto[]> {
    const body = this.buildAuthBody({ ...payload, ...extra });
    this.logRequestPayload('transactions/historical-details', body);

    return this.api
      .post<typeof body, ApiResponse<HistoricalTransactionDetailsDto[]>>('transactions/historical-details', body)
      .pipe(map((res) => res?.data ?? []));
  }

  getSecurityTransactionDetails(
    payload: { entityId: number },
    extra: Record<string, unknown> = {},
  ): Observable<SecurityTransactionDetailsDto[]> {
    const body = this.buildAuthBody({ ...payload, ...extra });
    this.logRequestPayload('transactions/security-details', body);

    return this.api
      .post<typeof body, ApiResponse<SecurityTransactionDetailsDto[]>>('transactions/security-details', body)
      .pipe(map((res) => res?.data ?? []));
  }

  getReeferTransactionDetails(
    payload: { entityId: number },
    extra: Record<string, unknown> = {},
  ): Observable<ReeferTransactionDetailsDto[]> {
    const body = this.buildAuthBody({ ...payload, ...extra });
    this.logRequestPayload('transactions/reefer-details', body);

    return this.api
      .post<typeof body, ApiResponse<ReeferTransactionDetailsDto[]>>('transactions/reefer-details', body)
      .pipe(map((res) => res?.data ?? []));
  }

  // ✅ devuelve ARRAY (como tu API)
  getTransactionDetailsWithFile(
    payload: TransactionDetailsWithFileRequestDto,
    extra: Record<string, unknown> = {},
  ): Observable<TransactionDetailsWithFileRequestDto[]> {
    const body = this.buildAuthBody({ ...payload, ...extra });
    this.logRequestPayload('transactions/details-with-file', body);

    return this.api
      .post<typeof body, ApiResponse<TransactionDetailsWithFileRequestDto[]>>('transactions/details-with-file', body)
      .pipe(map((res) => res?.data ?? []));
  }


  registerTransaction(payload: Record<string, unknown>): Observable<unknown> {
    const body = this.buildAuthBody(payload);
    console.log('[TransactionsService] POST transactions/register payload', body);

    return this.api
      .post<typeof body, ApiResponse<unknown>>('transactions/register', body)
      .pipe(map((res) => res?.data ?? res));
  }

  sendAlertMail(payload: {
    xmlDetails: string;
    containerId: string;
    eventName: string;
    stepLabel: string;
  }): Observable<unknown> {
    const formattedXml = this.escapeHtml(this.formatXmlForHtml(payload.xmlDetails));
    const body: AlertMailPayload = {
      to: this.alertMailTo,
      subject: `Alerta RFID ${payload.stepLabel} - ${payload.containerId} - ${payload.eventName}`,
      body: `<pre style="font-family: monospace; white-space: pre-wrap;">${formattedXml}</pre>`,
      format: 'HTML',
      cc: this.alertMailCc,
      bcc: '',
    };

    console.log('[TransactionsService] POST mail payload', body);

    return this.api
      .post<AlertMailPayload, unknown>(this.alertMailEndpoint, body)
      .pipe(map((res) => res ?? null));
  }

  private formatXmlForHtml(xml: string): string {
    const compactXml = xml.replace(/>\s+</g, '><').trim();
    const parts = compactXml.replace(/></g, '>\n<').split('\n');
    let depth = 0;

    return parts
      .map((part) => {
        const token = part.trim();
        if (!token) {
          return '';
        }

        if (/^<\//.test(token)) {
          depth = Math.max(depth - 1, 0);
        }

        const line = `${'\t'.repeat(depth)}${token}`;

        if (/^<[^!?/][^>]*[^/]?>$/.test(token) && !token.includes('</')) {
          depth += 1;
        }

        return line;
      })
      .filter(Boolean)
      .join('\n');
  }

  private escapeHtml(raw: string): string {
    return raw
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#39;');
  }

  registerTransactionWithFiles(payload: {
    photos: File[];
    eventId: number;
    entityType: string;
    entityId: number;
    status?: number;
    isReefer?: boolean;
    details?: string;
    xmlDetails?: string;
    document?: string;
    recordDate?: string;
    eventDate?: string;
  }): Observable<unknown> {
    const auth = this.buildAuthBody();
    const formData = new FormData();

    payload.photos.forEach((photo) => {
      formData.append('photos', photo, photo.name);
    });

    formData.append('companyId', String(auth.companyId));
    formData.append('userId', String(auth.userId));
    formData.append('ip', String(auth.ip));
    formData.append('device', String(auth.device));

    formData.append('eventId', String(payload.eventId));
    formData.append('entityType', payload.entityType);
    formData.append('entityId', String(payload.entityId));
    formData.append('status', String(payload.status ?? 1));
    formData.append('isReefer', String(payload.isReefer ?? false));
    formData.append('details', payload.details ?? 'RFID validated evidence');
    if (payload.document) {
      formData.append('document', payload.document);
    }
    if (payload.xmlDetails) {
      formData.append('xmlDetails', payload.xmlDetails);
    }
    if (payload.recordDate) {
      formData.append('recordDate', payload.recordDate);
    }
    if (payload.eventDate) {
      formData.append('eventDate', payload.eventDate);
    }

    const formDataPayload = Array.from(formData.entries()).map(([key, value]) => ({
      key,
      value:
        value instanceof File
          ? {
            name: value.name,
            type: value.type,
            size: value.size,
          }
          : value,
    }));
    console.log('[TransactionsService] POST transactions/register-with-files payload', formDataPayload);

    return this.api
      .post<FormData, ApiResponse<unknown>>('transactions/register-with-files', formData)
      .pipe(map((res) => res?.data ?? res));
  }

  validatePhotoWithRoboflow(payload: { imageBase64: string; fileName: string }): Observable<unknown> {
    const { apiKey, workspace, workflow, baseUrl } = environment.roboflow;
    const body = {
      api_key: apiKey,
      inputs: {
        image: {
          type: 'base64',
          value: payload.imageBase64,
        },
      },
      use_cache: true,
    };

    const endpoint = `${baseUrl.replace(/\/+$/, '')}/${workspace}/workflows/${workflow}`;
    console.log('[TransactionsService] POST Roboflow validation endpoint', endpoint);
    console.log('[TransactionsService] POST Roboflow validation payload', body);

    return this.api.post<typeof body, unknown>(endpoint, body);
  }
}
