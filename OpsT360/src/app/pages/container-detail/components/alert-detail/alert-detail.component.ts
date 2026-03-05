import { CommonModule } from '@angular/common';
import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';

import { TransactionsService } from '../../../../services/transactions/transactions.service';
import type { Field } from '../../field.type';
import { isItemsModalField } from '../../is-items-modal-field';
import type { ItemsModalPayload } from '../../items-modal-payload.type';

type AlertDetailItem = {
  id: string;
  title: string;
  recordDate: Date | null;
  isOpen: boolean;
  fieldsCols: [Field[], Field[]];
};

@Component({
  selector: 't360-alert-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './alert-detail.component.html',
})
export class AlertDetailComponent implements OnChanges {
  @Input() entityId: number | null = null;
  @Input() searchTerm = '';

  @Output() openItems = new EventEmitter<ItemsModalPayload>();
  @Output() contentChanged = new EventEmitter<void>();

  alertDetails: AlertDetailItem[] = [];
  isAlertLoading = false;
  private hasTriedLoadingAlerts = false;

  readonly isItemsModalField = isItemsModalField;

  constructor(private readonly transactionsService: TransactionsService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if ('entityId' in changes) {
      if (changes['entityId']?.previousValue !== changes['entityId']?.currentValue) {
        this.alertDetails = [];
        this.hasTriedLoadingAlerts = false;
      }
      this.fetchAlertDetailsIfPossible();
    }

    if ('searchTerm' in changes) {
      this.applySearchTerm(this.searchTerm);
    }
  }

  onToggle(detail: AlertDetailItem): void {
    detail.isOpen = !detail.isOpen;
    this.contentChanged.emit();
  }

  onOpenItems(payload: ItemsModalPayload): void {
    this.openItems.emit(payload);
  }

  trackByAlertDetailId(_: number, item: AlertDetailItem): string {
    return item.id;
  }

  private fetchAlertDetailsIfPossible(): void {
    if (this.hasTriedLoadingAlerts || this.isAlertLoading) return;
    const id = this.entityId;
    if (!id) return;

    this.isAlertLoading = true;

    this.transactionsService.getAlertDetails({ entityId: id }).subscribe({
      next: (list: unknown[]) => {
        this.alertDetails = this.mapAlertDetails(Array.isArray(list) ? list : []);
        this.isAlertLoading = false;
        this.hasTriedLoadingAlerts = true;
        this.contentChanged.emit();
        this.applySearchTerm(this.searchTerm);
      },
      error: (err: unknown) => {
        if (!this.isNotFoundError(err)) {
          console.error('[AlertDetail] getAlertDetails error', err);
        }
        this.alertDetails = [];
        this.isAlertLoading = false;
        this.hasTriedLoadingAlerts = true;
      },
    });
  }

  private applySearchTerm(term: string): void {
    const normalized = this.normalizeSearchValue(term).trim();
    if (!normalized) {
      let hasChanges = false;
      for (const detail of this.alertDetails) {
        if (detail.isOpen) {
          detail.isOpen = false;
          hasChanges = true;
        }
      }
      if (hasChanges) this.contentChanged.emit();
      return;
    }
    if (!this.alertDetails.length) {
      return;
    }

    const target = this.alertDetails.find((detail) => this.matchesAlertDetail(detail, normalized));
    if (!target) return;

    let hasChanges = false;
    for (const detail of this.alertDetails) {
      const shouldOpen = detail === target;
      if (detail.isOpen !== shouldOpen) {
        detail.isOpen = shouldOpen;
        hasChanges = true;
      }
    }

    if (hasChanges) this.contentChanged.emit();
  }

  private matchesAlertDetail(detail: AlertDetailItem, normalized: string): boolean {
    if (this.matchesString(detail.title, normalized)) return true;
    if (detail.recordDate && this.matchesString(detail.recordDate.toISOString(), normalized)) return true;

    const columns = detail.fieldsCols ?? [];
    for (const col of columns) {
      for (const field of col) {
        if (this.matchesString(field.label, normalized) || this.matchesString(field.value, normalized)) {
          return true;
        }
      }
    }

    return false;
  }

  private matchesString(value: string | null | undefined, normalized: string): boolean {
    return this.normalizeSearchValue(value).includes(normalized);
  }

  private normalizeSearchValue(value: string | null | undefined): string {
    return (value ?? '')
      .toString()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .toLowerCase();
  }

  private mapAlertDetails(list: unknown[]): AlertDetailItem[] {
    const sorted = [...list].sort((a, b) => this.toTime(this.pickDate(b)) - this.toTime(this.pickDate(a)));

    return sorted.map((item, index) => {
      const record = this.isRecord(item) ? item : {};
      const title = this.pickTitle(record, index);
      const date = this.toDate(this.pickDate(record));
      const id = this.pickId(record, index);
      const fields = this.buildAlertFields(record);
      const fieldsCols = this.splitInto2Columns(fields);

      return {
        id,
        title,
        recordDate: date,
        isOpen: false,
        fieldsCols,
      };
    });
  }

  private pickTitle(record: Record<string, unknown>, index: number): string {
    const fallback = `Alert ${index + 1}`;
    const candidates = [
      record['alertName'],
      record['eventName'],
      record['name'],
      record['description'],
      record['message'],
      record['errorMessage'],
    ];
    const hit = candidates.find((value) => typeof value === 'string' && value.trim().length > 0);
    return (hit as string | undefined)?.trim() || fallback;
  }

  private pickDate(record: Record<string, unknown> | unknown): string | null {
    if (!this.isRecord(record)) return null;
    const candidates = [
      record['eventDate'],
      record['recordDate'],
      record['createdAt'],
      record['createdOn'],
      record['timestamp'],
      record['date'],
    ];
    const hit = candidates.find((value) => typeof value === 'string' && value.trim().length > 0);
    return (hit as string | undefined) ?? null;
  }

  private pickId(record: Record<string, unknown>, index: number): string {
    const candidates = [record['alertDetailId'], record['alertId'], record['id']];
    const hit = candidates.find((value) => value != null);
    return hit != null ? String(hit) : `alert-${index}`;
  }

  private buildAlertFields(record: Record<string, unknown>): Field[] {
    const out: Field[] = [];
    for (const key of Object.keys(record)) {
      out.push(...this.alertValueToFields(record[key], key));
    }
    return this.compactFieldsRaw(out);
  }

  private alertValueToFields(value: unknown, label: string, depth = 0): Field[] {
    if (value == null) return [{ label: this.formatAlertLabel(label), value: '—' }];
    if (depth > 6) return [{ label: this.formatAlertLabel(label), value: this.safeString(value) }];

    if (typeof value === 'string' || typeof value === 'number' || typeof value === 'boolean') {
      return [{ label: this.formatAlertLabel(label), value: String(value) }];
    }

    if (Array.isArray(value)) {
      const allPrimitive = value.every(
        (v) => v == null || typeof v === 'string' || typeof v === 'number' || typeof v === 'boolean',
      );
      if (allPrimitive) {
        const joined = value.map((v) => String(v ?? '')).filter(Boolean).join(', ');
        return [{ label: this.formatAlertLabel(label), value: joined || '—' }];
      }

      const allObjects = value.every((v) => this.isRecord(v));
      if (allObjects) {
        const lbl = this.formatAlertLabel(label);
        return [
          {
            label: lbl,
            value: value.length ? `View table (${value.length})` : '—',
            action: 'itemsModal',
            payload: { title: lbl, items: value as unknown[] },
          },
        ];
      }

      return [{ label: this.formatAlertLabel(label), value: `${value.length} item(s)` }];
    }

    if (this.isRecord(value)) {
      const out: Field[] = [];
      for (const key of Object.keys(value)) {
        out.push(...this.alertValueToFields(value[key], `${label} / ${key}`, depth + 1));
      }
      return out.length ? out : [{ label: this.formatAlertLabel(label), value: '—' }];
    }

    return [{ label: this.formatAlertLabel(label), value: this.safeString(value) }];
  }

  private formatAlertLabel(label: string): string {
    return (
      String(label ?? '')
        .split('/')
        .map((p) => p.trim().replace(/_/g, ' '))
        .filter(Boolean)
        .join(' / ') || '—'
    );
  }

  private compactFieldsRaw(fields: Field[]): Field[] {
    const cleaned = fields
      .map((f) => ({
        ...f,
        label: (f.label ?? '').toString().trim() || '—',
        value: (f.value ?? '').toString().trim() || '—',
      }))
      .filter((f) => f.label !== '');

    const seen = new Set<string>();
    const unique: Field[] = [];
    for (const f of cleaned) {
      const key = f.label.toLowerCase();
      if (seen.has(key)) continue;
      seen.add(key);
      unique.push(f);
    }
    return unique;
  }

  private splitInto2Columns(fields: Field[]): [Field[], Field[]] {
    const left: Field[] = [];
    const right: Field[] = [];
    fields.forEach((f, i) => (i % 2 === 0 ? left : right).push(f));
    return [left, right];
  }

  private toDate(value: string | null | undefined): Date | null {
    const ms = this.toTime(value);
    return ms ? new Date(ms) : null;
  }

  private toTime(iso: string | null | undefined): number {
    if (!iso) return 0;
    const ms = Date.parse(iso);
    return Number.isFinite(ms) ? ms : 0;
  }

  private safeString(v: unknown): string {
    try {
      return JSON.stringify(v);
    } catch {
      return String(v);
    }
  }

  private isRecord(x: unknown): x is Record<string, unknown> {
    return !!x && typeof x === 'object' && !Array.isArray(x);
  }

  private isNotFoundError(error: unknown): boolean {
    const status = (error as { response?: { status?: number } })?.response?.status;
    return status === 404;
  }
}
