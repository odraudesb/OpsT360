import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ApiService } from '../api/api.service';

export type PurchaseRequestApiItem = {
  SealPurchaseRequestId: number;
  RequestDescription: string;
  RequesterUserId: number | null;
  RequestDate: string;
  sealTypeDescription: string;
  Quantity: number;
  currentStatusName: string;
  CreatedBy: string | null;
};

export type PurchaseRequestApiResponse = {
  data: PurchaseRequestApiItem[];
  errors: unknown[];
};

export type CreatePurchaseRequestPayload = {
  requestDescription: string;
  requesterUserId?: number | null;
  justification: string;
  notes: string;
  sealTypeId: number;
  quantity: number;
  purchaseUrgencyLevelId: number;
  initialStatusName: string;
  comments: string;
  createdBy: string;
};

export type UpdatePurchaseRequestStatusPayload = {
  statusName: string;
  comments: string;
  rejectionReason: string | null;
  updatedBy: string;
  actionByUserId: number | null;
};

@Injectable({ providedIn: 'root' })
export class PurchaseRequestService {
  private readonly cache$ = new BehaviorSubject<PurchaseRequestApiResponse | null>(null);

  constructor(private readonly api: ApiService) {}

  getAll(): Observable<PurchaseRequestApiResponse> {
    return this.api.get<PurchaseRequestApiResponse>('seals/purchase-request');
  }

  getAllCached(): Observable<PurchaseRequestApiResponse> {
    const cached = this.cache$.value;
    if (cached) {
      return of(cached);
    }
    return this.getAll().pipe(tap((response) => this.cache$.next(response)));
  }

  refresh(): Observable<PurchaseRequestApiResponse> {
    return this.getAll().pipe(tap((response) => this.cache$.next(response)));
  }

  preload(): void {
    if (this.cache$.value) {
      return;
    }
    this.getAll().pipe(tap((response) => this.cache$.next(response))).subscribe({
      error: () => {
        this.cache$.next(null);
      },
    });
  }

  create(payload: CreatePurchaseRequestPayload): Observable<PurchaseRequestApiResponse> {
    return this.api.post<CreatePurchaseRequestPayload, PurchaseRequestApiResponse>(
      'seals/purchase-request',
      payload,
    );
  }

  updateStatus(
    requestId: string,
    payload: UpdatePurchaseRequestStatusPayload,
  ): Observable<PurchaseRequestApiResponse> {
    return this.api.put<UpdatePurchaseRequestStatusPayload, PurchaseRequestApiResponse>(
      `seals/purchase-request/${requestId}/status`,
      payload,
    );
  }
}
