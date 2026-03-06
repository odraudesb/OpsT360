import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

import { ApiService } from '../api/api.service';
import { DEFAULT_DEVICE_INFO } from '../../core/constants/device-info';
import { getAuthContextFromStorage } from '../../core/utils/auth-storage';

import type { ApiResponse } from '../../interfaces/transactions/transaction.interface';
import type {
    TransactionDetailsWithFileItemDto,
    TransactionDetailsWithFileRequestDto,
} from '../../interfaces/transactions/transaction-details.interface';

@Injectable({ providedIn: 'root' })
export class ShipmentDocumentsService {
    constructor(private readonly api: ApiService) { }

    private buildAuthBody(extra: Record<string, unknown> = {}): Record<string, unknown> {
        const ctx = getAuthContextFromStorage();
        if (!ctx) throw new Error('Missing auth context');

        return {
            companyId: ctx.companyId,
            userId: ctx.userId,
            ip: DEFAULT_DEVICE_INFO.ip,
            device: DEFAULT_DEVICE_INFO.device,
            ...extra,
        };
    }

    getTransactionDetailsWithFile(
        payload: TransactionDetailsWithFileRequestDto,
    ): Observable<TransactionDetailsWithFileItemDto[]> {
        const body = this.buildAuthBody(payload as unknown as Record<string, unknown>);

        return this.api
            .post<typeof body, ApiResponse<TransactionDetailsWithFileItemDto[]>>('transactions/details-with-file', body)
            .pipe(map((res) => res?.data ?? []));
    }
}
