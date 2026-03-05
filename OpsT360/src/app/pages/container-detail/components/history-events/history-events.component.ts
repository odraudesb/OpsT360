import { CommonModule } from '@angular/common';
import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';

import type { HistoricalTransactionDetailsDto } from '../../../../interfaces/transactions/historical-transaction-details.interface';
import { TransactionsService } from '../../../../services/transactions/transactions.service';
import type { ContainerHistoryDetail } from '../../container-history-detail.type';
import type { Field } from '../../field.type';
import { isItemsModalField } from '../../is-items-modal-field';
import type { ItemsModalPayload } from '../../items-modal-payload.type';

@Component({
  selector: 't360-history-events',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './history-events.component.html',
})
export class HistoryEventsComponent implements OnChanges {
  @Input() entityId: number | null = null;

  // ✅ nuevo: tx a enfocar (viene desde queryparam historyTx)
  @Input() focusTxId: number | null = null;
  @Input() searchTerm = '';

  @Output() openItems = new EventEmitter<ItemsModalPayload>();
  @Output() contentChanged = new EventEmitter<void>();

  historyDetails: ContainerHistoryDetail[] = [];
  isHistoryLoading = false;
  private hasTriedLoadingHistory = false;

  private lastFocusedTxId: number | null = null;
  highlightTxId: number | null = null;

  readonly isItemsModalField = isItemsModalField;

  constructor(private readonly transactionsService: TransactionsService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if ('entityId' in changes) {
      if (changes['entityId']?.previousValue !== changes['entityId']?.currentValue) {
        this.hasTriedLoadingHistory = false;
        this.historyDetails = [];
        this.lastFocusedTxId = null;
        this.highlightTxId = null;
      }
      this.fetchHistoryDetailsIfPossible();
    }

    // ✅ si cambia focusTxId y ya hay data, intento enfocar
    if ('focusTxId' in changes) {
      this.highlightTxId = this.focusTxId;
      setTimeout(() => this.tryFocusTx(), 0);
    }

    if ('searchTerm' in changes) {
      this.applySearchTerm(this.searchTerm);
    }
  }

  onToggle(detail: ContainerHistoryDetail): void {
    detail.isOpen = !detail.isOpen;
    if (this.highlightTxId && this.highlightTxId !== detail.transactionId) {
      this.highlightTxId = null;
    }
    // para que el buscador global re-marque cuando se abre/cierra
    this.contentChanged.emit();
  }

  onOpenItems(payload: ItemsModalPayload): void {
    this.openItems.emit(payload);
  }

  trackByHistoryDetailId(_: number, ev: ContainerHistoryDetail): string {
    return ev.id;
  }

  private fetchHistoryDetailsIfPossible(): void {
    if (this.hasTriedLoadingHistory || this.isHistoryLoading) return;
    const id = this.entityId;
    if (!id) return;

    this.isHistoryLoading = true;

    this.transactionsService.getHistoricalTransactionDetails({ entityId: id }).subscribe({
      next: (list: HistoricalTransactionDetailsDto[]) => {
        this.historyDetails = this.mapHistoryDetails(list);
        this.isHistoryLoading = false;
        this.hasTriedLoadingHistory = true;
        this.contentChanged.emit();

        // ✅ cuando cargan eventos, intento enfocar el tx pedido
        setTimeout(() => this.tryFocusTx(), 0);
        this.applySearchTerm(this.searchTerm);
      },
      error: (err: unknown) => {
        console.error('[HistoryEvents] getHistoricalTransactionDetails error', err);
        this.historyDetails = [];
        this.isHistoryLoading = false;
        this.hasTriedLoadingHistory = true;
      },
    });
  }

  private tryFocusTx(): void {
    const tx = this.focusTxId;
    if (!tx || tx <= 0) return;
    if (this.lastFocusedTxId === tx) return;
    if (!this.historyDetails.length) return;

    const target = this.historyDetails.find((h) => h.transactionId === tx);
    if (!target) return;

    // abrirlo (no cierro los demás para no molestar)
    target.isOpen = true;
    this.lastFocusedTxId = tx;

    // forzar repaint para que exista el contenido antes del scroll
    this.contentChanged.emit();

    const domId = `history-event-tx-${tx}`;
    requestAnimationFrame(() => {
      requestAnimationFrame(() => {
        const el = document.getElementById(domId);
        el?.scrollIntoView({ behavior: 'smooth', block: 'center' });
      });
    });
  }

  private applySearchTerm(term: string): void {
    const normalized = this.normalizeSearchValue(term).trim();
    if (!normalized) {
      let hasChanges = false;
      for (const detail of this.historyDetails) {
        if (detail.isOpen) {
          detail.isOpen = false;
          hasChanges = true;
        }
      }
      if (hasChanges) this.contentChanged.emit();
      return;
    }
    if (!this.historyDetails.length) {
      return;
    }

    const target = this.historyDetails.find((detail) => this.matchesHistoryDetail(detail, normalized));
    if (!target) return;

    let hasChanges = false;
    for (const detail of this.historyDetails) {
      const shouldOpen = detail === target;
      if (detail.isOpen !== shouldOpen) {
        detail.isOpen = shouldOpen;
        hasChanges = true;
      }
    }

    if (hasChanges) this.contentChanged.emit();
  }

  private matchesHistoryDetail(detail: ContainerHistoryDetail, normalized: string): boolean {
    if (this.matchesString(detail.eventName, normalized)) return true;
    if (this.matchesString(String(detail.transactionId ?? ''), normalized)) return true;
    if (detail.eventId != null && this.matchesString(String(detail.eventId), normalized)) return true;

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

  private mapHistoryDetails(list: HistoricalTransactionDetailsDto[]): ContainerHistoryDetail[] {
    const sorted = [...list].sort(
      (a, b) =>
        this.toTimeIso(
          ((b as unknown as Record<string, unknown>)['eventDate'] as string | null) ??
            ((b as unknown as Record<string, unknown>)['recordDate'] as string | null),
        ) -
        this.toTimeIso(
          ((a as unknown as Record<string, unknown>)['eventDate'] as string | null) ??
            ((a as unknown as Record<string, unknown>)['recordDate'] as string | null),
        ),
    );

    return sorted.map((item, index) => {
      const r = item as unknown as Record<string, unknown>;
      const xml = this.normalizeXmlDetails(r['xmlDetails']);

      const cont = this.pickCaseInsensitive(xml, 'Contenedor');
      const xmlToShow = this.isRecord(cont) ? cont : xml;

      const eventName =
        (r['eventName'] as string | undefined) || (r['eventId'] != null ? `Evento ${r['eventId']}` : 'Evento');

      const txId = Number(r['transactionId'] ?? 0);
      const eventId = (r['eventId'] as number | null) ?? null;

      const id = txId ? `${txId}-${eventId ?? 'evt'}` : `${eventId ?? 'evt'}-${index}`;

      const fields = this.buildHistoryFields(xmlToShow);
      const fieldsCols = this.splitInto2Columns(fields);

      return {
        id,
        transactionId: txId,
        eventId,
        eventName,
        recordDate: this.toDate(r['recordDate'] as string | null),
        eventDate: this.toDate(r['eventDate'] as string | null),
        xmlDetails: xmlToShow,
        isOpen: false,
        fieldsCols,
      };
    });
  }

  private buildHistoryFields(xml: unknown): Field[] {
    if (!this.isRecord(xml)) return [{ label: '—', value: '—' }];

    const sealLookup = this.buildSealLookup(xml);
    const out: Field[] = [];
    for (const k of Object.keys(xml)) {
      if (this.isIdLikeKey(k)) continue;
      const value = (xml as Record<string, unknown>)[k];
      const safeValue = this.isFailedSealLabel(k) ? this.withFailedSealReference(value, sealLookup) : value;
      out.push(...this.historyValueToFields(safeValue, k));
    }
    return this.compactFieldsRaw(out);
  }

  private buildSealLookup(xml: Record<string, unknown>): Map<string, string> {
    const map = new Map<string, string>();
    for (const [key, value] of Object.entries(xml)) {
      if (typeof value !== 'string') continue;

      const sealNo = this.extractSealNumber(key);
      if (!sealNo) continue;

      const sealCode = value.trim();
      if (!sealCode) continue;
      map.set(sealCode, `Seal ${sealNo}`);
    }
    return map;
  }

  private withFailedSealReference(value: unknown, sealLookup: Map<string, string>): unknown {
    if (typeof value !== 'string') return value;
    const raw = value.trim();
    if (!raw) return value;

    const normalized = raw
      .split(',')
      .map((token) => token.trim())
      .filter(Boolean)
      .map((sealCode) => {
        const sealLabel = sealLookup.get(sealCode);
        return sealLabel ? `${sealLabel} (${sealCode})` : sealCode;
      });

    return normalized.join(', ') || value;
  }

  private historyValueToFields(value: unknown, label: string, depth = 0): Field[] {
    if (value == null) return [{ label: this.formatHistoryLabel(label), value: '—' }];
    if (depth > 6) return [{ label: this.formatHistoryLabel(label), value: this.safeString(value) }];

    if (typeof value === 'string' || typeof value === 'number' || typeof value === 'boolean') {
      return [{ label: this.formatHistoryLabel(label), value: String(value) }];
    }

    if (Array.isArray(value)) {
      const allPrimitive = value.every(
        (v) => v == null || typeof v === 'string' || typeof v === 'number' || typeof v === 'boolean',
      );
      if (allPrimitive) {
        const joined = value.map((v) => String(v ?? '')).filter(Boolean).join(', ');
        return [{ label: this.formatHistoryLabel(label), value: joined || '—' }];
      }

      const allObjects = value.every((v) => this.isRecord(v));
      if (allObjects) {
        const lbl = this.formatHistoryLabel(label);
        return [
          {
            label: lbl,
            value: value.length ? `View table (${value.length})` : '—',
            action: 'itemsModal',
            payload: { title: lbl, items: value as unknown[] },
          },
        ];
      }

      return [{ label: this.formatHistoryLabel(label), value: `${value.length} item(s)` }];
    }

    if (this.isRecord(value)) {
      const out: Field[] = [];
      for (const k of Object.keys(value)) {
        if (this.isIdLikeKey(k)) continue;
        out.push(...this.historyValueToFields((value as Record<string, unknown>)[k], `${label} / ${k}`, depth + 1));
      }
      return out.length ? out : [{ label: this.formatHistoryLabel(label), value: '—' }];
    }

    return [{ label: this.formatHistoryLabel(label), value: this.safeString(value) }];
  }

  private formatHistoryLabel(label: string): string {
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
      .filter((f) => f.label !== '' && !this.isIdLikeKey(f.label));

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

  private normalizeKey(s: string): string {
    return (s ?? '').toString().trim().toLowerCase();
  }

  private isIdLikeKey(key: string): boolean {
    const nk = this.normalizeKey(key);
    return nk.includes('id');
  }

  private isFailedSealLabel(key: string): boolean {
    const normalized = this.normalizeKey(key).replace(/_/g, ' ').replace(/\s+/g, ' ');
    return normalized === 'failed seals' || normalized === 'sellos fallidos' || normalized === 'sello fallido';
  }

  private extractSealNumber(key: string): string | null {
    const normalized = this.normalizeKey(key).replace(/_/g, '-').replace(/\s+/g, '');
    const match = normalized.match(/^sello-(\d+)$/);
    return match?.[1] ?? null;
  }

  private normalizeXmlDetails(xmlDetails: unknown): Record<string, unknown> {
    if (!xmlDetails) return {};
    if (typeof xmlDetails === 'string') {
      try {
        const parsed = JSON.parse(xmlDetails) as unknown;
        return this.isRecord(parsed) ? parsed : {};
      } catch {
        return {};
      }
    }
    return this.isRecord(xmlDetails) ? xmlDetails : {};
  }

  private pickCaseInsensitive(obj: Record<string, unknown>, key: string): unknown {
    if (key in obj) return obj[key];
    const found = Object.keys(obj).find((k) => k.toLowerCase() === key.toLowerCase());
    return found ? obj[found] : undefined;
  }

  private toDate(value: string | null | undefined): Date | null {
    const ms = this.toTimeIso(value);
    return ms ? new Date(ms) : null;
  }

  private toTimeIso(iso: string | null | undefined): number {
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
}
