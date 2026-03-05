import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';

import type { ReeferTransactionDetailsDto } from '../../../../interfaces/transactions/reefer-transaction-details.interface';
import { TransactionsService } from '../../../../services/transactions/transactions.service';
import type { Field } from '../../field.type';

type ReeferHistoryDetail = {
  id: string;
  transactionId: number;
  eventId: number | null;
  eventName: string;
  recordDate: Date | null;
  eventDate: Date | null;
  fieldsCols: [Field[], Field[]];
  isOpen: boolean;
};

@Component({
  selector: 't360-reefer-trace',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reefer-trace.component.html',
})
export class ReeferTraceComponent implements OnChanges {
  @Input() entityId: number | null = null;

  reeferDetails: ReeferHistoryDetail[] = [];
  isReeferLoading = false;
  private hasTriedLoading = false;

  constructor(private readonly transactionsService: TransactionsService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if ('entityId' in changes) {
      if (changes['entityId']?.previousValue !== changes['entityId']?.currentValue) {
        this.hasTriedLoading = false;
        this.reeferDetails = [];
      }
      this.fetchReeferDetailsIfPossible();
    }
  }

  onToggle(detail: ReeferHistoryDetail): void {
    detail.isOpen = !detail.isOpen;
  }

  trackByReeferDetailId(_: number, ev: ReeferHistoryDetail): string {
    return ev.id;
  }

  private fetchReeferDetailsIfPossible(): void {
    if (this.hasTriedLoading || this.isReeferLoading) return;
    const id = this.entityId;
    if (!id) return;

    this.isReeferLoading = true;

    this.transactionsService.getReeferTransactionDetails({ entityId: id }).subscribe({
      next: (list: ReeferTransactionDetailsDto[]) => {
        this.reeferDetails = this.mapReeferDetails(list);
        this.isReeferLoading = false;
        this.hasTriedLoading = true;
      },
      error: (err: unknown) => {
        console.error('[ReeferTrace] getReeferTransactionDetails error', err);
        this.reeferDetails = [];
        this.isReeferLoading = false;
        this.hasTriedLoading = true;
      },
    });
  }

  private mapReeferDetails(list: ReeferTransactionDetailsDto[]): ReeferHistoryDetail[] {
    const sorted = [...list].sort(
      (a, b) => this.toTimeIso(b?.eventDate ?? b?.recordDate) - this.toTimeIso(a?.eventDate ?? a?.recordDate),
    );

    return sorted.map((item, index) => {
      const eventName = item?.eventName ?? (item?.eventId != null ? `Evento ${item.eventId}` : 'Evento');
      const txId = Number(item?.transactionId ?? 0);
      const eventId = (item?.eventId ?? null) as number | null;
      const id = txId ? `${txId}-${eventId ?? 'evt'}` : `${eventId ?? 'evt'}-${index}`;

      const fields = this.buildFields(item);
      const fieldsCols = this.splitInto2Columns(fields);

      return {
        id,
        transactionId: txId,
        eventId,
        eventName,
        recordDate: this.toDate(item?.recordDate ?? null),
        eventDate: this.toDate(item?.eventDate ?? null),
        isOpen: false,
        fieldsCols,
      };
    });
  }

  private buildFields(item: ReeferTransactionDetailsDto): Field[] {
    const skip = new Set(['transactionId', 'eventId', 'eventName', 'recordDate', 'eventDate']);
    const fields: Field[] = [];
    for (const [key, value] of Object.entries(item ?? {})) {
      if (skip.has(key)) continue;
      fields.push({ label: this.formatLabel(key), value: this.formatValue(value) });
    }
    return fields.length ? fields : [{ label: '—', value: '—' }];
  }

  private splitInto2Columns(fields: Field[]): [Field[], Field[]] {
    const left: Field[] = [];
    const right: Field[] = [];
    fields.forEach((field, idx) => {
      if (idx % 2 === 0) left.push(field);
      else right.push(field);
    });
    return [left, right];
  }

  private formatLabel(label: string): string {
    const withSpaces = String(label ?? '')
      .replace(/_/g, ' ')
      .replace(/([a-z])([A-Z])/g, '$1 $2')
      .trim();
    return withSpaces ? withSpaces[0].toUpperCase() + withSpaces.slice(1) : '—';
  }

  private formatValue(value: unknown): string {
    if (value == null || value === '') return '—';
    if (typeof value === 'string' || typeof value === 'number' || typeof value === 'boolean') {
      return String(value);
    }
    if (Array.isArray(value)) return value.length ? value.join(', ') : '—';
    if (typeof value === 'object') return JSON.stringify(value);
    return String(value);
  }

  private toDate(raw: string | null | undefined): Date | null {
    if (!raw) return null;
    const date = new Date(raw);
    return Number.isNaN(date.getTime()) ? null : date;
  }

  private toTimeIso(raw: string | null | undefined): number {
    if (!raw) return 0;
    const date = new Date(raw);
    return Number.isNaN(date.getTime()) ? 0 : date.getTime();
  }
}
