import { CommonModule } from '@angular/common';
import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { map, Subject, takeUntil } from 'rxjs';

import type { ContainerShipmentDetails } from '../../../../interfaces/containers/ContainerShipmentDetails';
import type { TransactionDto } from '../../../../interfaces/transactions/transaction.interface';
import type { TransactionDetailsDto } from '../../../../interfaces/transactions/transaction-details.interface';
import { TransactionsService } from '../../../../services/transactions/transactions.service';
import type { Field } from '../../field.type';
import { isItemsModalField } from '../../is-items-modal-field';
import type { ItemsModalPayload } from '../../items-modal-payload.type';

@Component({
  selector: 't360-track-and-trace-section',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './track-and-trace-section.component.html',
})
export class TrackAndTraceSectionComponent implements OnInit, OnDestroy {
  @Input() containerId = '';
  @Input() initialTxId: number | null = null;
  @Input() initialEntityId: number | null = null;
  @Input() searchTerm = '';

  @Output() searchInput = new EventEmitter<Event>();
  @Output() searchEnter = new EventEmitter<void>();
  @Output() openItems = new EventEmitter<ItemsModalPayload>();
  @Output() entityIdResolved = new EventEmitter<number>();
  @Output() contentChanged = new EventEmitter<void>();

  shipmentDetails: ContainerShipmentDetails = { ...DEFAULT_SHIPMENT_DETAILS };
  shipmentFieldsCols: [Field[], Field[]] = [[], []];
  private entityId: number | null = null;
  private readonly containerTransactionsMap = new Map<string, TransactionDto[]>();
  private readonly destroy$ = new Subject<void>();

  readonly isItemsModalField = isItemsModalField;

  get hasFields(): boolean {
    return (this.shipmentFieldsCols?.[0]?.length || 0) + (this.shipmentFieldsCols?.[1]?.length || 0) > 0;
  }

  constructor(private readonly transactionsService: TransactionsService) { }

  ngOnInit(): void {
    this.shipmentDetails = { ...DEFAULT_SHIPMENT_DETAILS, containerId: this.containerId };
    this.entityId = this.initialEntityId;
    this.loadAll();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSearchInput(ev: Event): void {
    this.searchInput.emit(ev);
  }

  onSearchEnter(): void {
    this.searchEnter.emit();
  }

  onOpenItems(payload: ItemsModalPayload): void {
    this.openItems.emit(payload);
  }

  private loadAll(): void {
    if (this.initialTxId) this.fetchDetails(this.initialTxId);
    this.fetchTransactionsForContainer(this.containerId);
  }

  private fetchTransactionsForContainer(code: string): void {
    const cached = this.containerTransactionsMap.get(code) ?? [];
    if (cached.length > 0) {
      this.processTransactions(cached);
      return;
    }

    this.transactionsService
      .getAllTransactions()
      .pipe(
        map((txs: TransactionDto[]) =>
          txs.filter(
            (t) =>
              this.normalizeContainerCode((t as unknown as Record<string, unknown>)['entityType']) ===
              this.normalizeContainerCode(code),
          ),
        ),
        map((list: TransactionDto[]) => {
          list.sort((a, b) => this.toTime(b) - this.toTime(a));
          return list;
        }),
        takeUntil(this.destroy$),
      )
      .subscribe({
        next: (list: TransactionDto[]) => {
          this.containerTransactionsMap.set(code, list);
          this.processTransactions(list);
        },
        error: (err: unknown) => console.error('[TrackAndTrace] getAllTransactions error', err),
      });
  }

  private processTransactions(list: TransactionDto[]): void {
    this.ensureEntityIdFromTransactions(list);

    if (!this.initialTxId) {
      const first = list[0] as unknown as Record<string, unknown>;
      const latestTxId = Number(first?.['transactionId'] ?? 0);
      if (latestTxId) this.fetchDetails(latestTxId);
    }
  }

  private ensureEntityIdFromTransactions(list: TransactionDto[]): void {
    if (this.entityId != null) return;

    const withId = list.find((t) => Number.isFinite(Number((t as unknown as Record<string, unknown>)['entityId'])));
    if (!withId) return;

    const eid = Number((withId as unknown as Record<string, unknown>)['entityId']);
    if (Number.isFinite(eid) && eid > 0) {
      this.entityId = eid;
      this.entityIdResolved.emit(eid);
    }
  }

  private fetchDetails(transactionId: number): void {
    this.transactionsService
      .getTransactionDetails(transactionId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (details: TransactionDetailsDto) => {
          this.shipmentDetails = this.mapDetailsToShipment(details);

          const fields = this.buildShipmentFields(details);
          const safeFields = fields.length ? fields : [{ label: '—', value: '—' }];
          this.shipmentFieldsCols = this.splitInto2Columns(safeFields);

          this.contentChanged.emit();
        },
        error: (err: unknown) => console.error('[TrackAndTrace] getTransactionDetails error', err),
      });
  }

  private mapDetailsToShipment(details: TransactionDetailsDto): ContainerShipmentDetails {
    const xml = this.normalizeXmlDetails(details.xmlDetails);

    const cont = this.pickCaseInsensitive(xml, 'Contenedor');
    const contRec = this.isRecord(cont) ? cont : {};

    const contenido = contRec['Contenido'];
    const itemsRaw = this.isRecord(contenido) ? contenido['Item'] : undefined;
    const itemsArr: unknown[] = Array.isArray(itemsRaw) ? itemsRaw : itemsRaw ? [itemsRaw] : [];

    const goods =
      itemsArr
        .map((x) => this.pickAnyString(x, ['Descripcion', 'Descripción', 'description', 'Description']))
        .filter((s) => !!s)
        .join(', ') || '—';

    const tipo = this.getString(contRec, 'Tipo') ?? '—';
    const destino = this.getString(contRec, 'Destino') ?? '—';

    const fechaSalida = this.getString(contRec, 'Fecha Salida') ?? this.getString(contRec, 'Fecha_Salida');
    const fechaEta =
      this.getString(contRec, 'Fecha Llegada Estimada') ?? this.getString(contRec, 'Fecha_Llegada_Estimada');

    return {
      ...DEFAULT_SHIPMENT_DETAILS,
      containerId: this.containerId,
      size: String(tipo),
      goods,
      pod: String(destino),
      preAdvice: this.parseDateString(fechaSalida),
      gateIn: this.parseDateString(fechaEta),
    };
  }

  private buildShipmentFields(details: TransactionDetailsDto): Field[] {
    const xml = this.normalizeXmlDetails(details.xmlDetails);
    const out: Field[] = [];

    const cont = this.pickCaseInsensitive(xml, 'Contenedor');
    if (this.isRecord(cont)) out.push(...this.objectToFields(cont, ''));

    if (this.isRecord(xml)) {
      for (const k of Object.keys(xml)) {
        if (k.toLowerCase() === 'contenedor') continue;
        out.push(...this.valueToFields((xml as Record<string, unknown>)[k], k));
      }
    }

    return this.compactFields(out);
  }

  private objectToFields(obj: Record<string, unknown>, prefix: string): Field[] {
    const out: Field[] = [];

    for (const k of Object.keys(obj)) {
      const rawValue = obj[k];
      const kLower = String(k).toLowerCase();
      const rawLabel = prefix ? `${prefix} / ${k}` : String(k);

      const valueToUse = this.isContainerIdKey(k) ? this.containerId : rawValue;

      if (kLower === 'contenido') {
        const itemsRaw = this.isRecord(rawValue) ? rawValue['Item'] : undefined;
        const items: unknown[] = Array.isArray(itemsRaw) ? itemsRaw : itemsRaw ? [itemsRaw] : [];

        out.push({
          label: this.translateLabel(rawLabel),
          value: items.length ? `View items (${items.length})` : '—',
          action: 'itemsModal',
          payload: { title: this.translateLabel(`${rawLabel} / Item`), items },
        });

        if (this.isRecord(rawValue)) {
          for (const k2 of Object.keys(rawValue)) {
            if (String(k2).toLowerCase() === 'item') continue;
            out.push(...this.valueToFields((rawValue as Record<string, unknown>)[k2], `${rawLabel} / ${k2}`));
          }
        }
        continue;
      }

      out.push(...this.valueToFields(valueToUse, rawLabel));
    }

    return out;
  }

  private valueToFields(value: unknown, label: string, depth = 0): Field[] {
    if (value == null) return [{ label: this.translateLabel(label), value: '—' }];
    if (depth > 6) return [{ label: this.translateLabel(label), value: this.safeString(value) }];

    if (typeof value === 'string' || typeof value === 'number' || typeof value === 'boolean') {
      return [{ label: this.translateLabel(label), value: String(value) }];
    }

    if (Array.isArray(value)) {
      const allPrimitive = value.every(
        (v) => v == null || typeof v === 'string' || typeof v === 'number' || typeof v === 'boolean',
      );
      if (allPrimitive) {
        const joined = value.map((v) => String(v ?? '')).filter(Boolean).join(', ');
        return [{ label: this.translateLabel(label), value: joined || '—' }];
      }

      const allObjects = value.every((v) => this.isRecord(v));
      if (allObjects) {
        const lbl = this.translateLabel(label);
        return [
          {
            label: lbl,
            value: value.length ? `View table (${value.length})` : '—',
            action: 'itemsModal',
            payload: { title: lbl, items: value as unknown[] },
          },
        ];
      }

      return [{ label: this.translateLabel(label), value: `${value.length} item(s)` }];
    }

    if (this.isRecord(value)) {
      const out: Field[] = [];
      for (const k of Object.keys(value)) {
        out.push(...this.valueToFields((value as Record<string, unknown>)[k], `${label} / ${k}`, depth + 1));
      }
      return out.length ? out : [{ label: this.translateLabel(label), value: '—' }];
    }

    return [{ label: this.translateLabel(label), value: this.safeString(value) }];
  }

  private compactFields(fields: Field[]): Field[] {
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

  // Label helpers
  private normalizeKey(s: string): string {
    return (s ?? '')
      .toString()
      .trim()
      .normalize('NFD')
      .replace(/[\\u0300-\\u036f]/g, '')
      .toLowerCase();
  }

  private translateLabel(label: string): string {
    const parts = String(label ?? '')
      .split('/')
      .map((p) => p.trim())
      .filter(Boolean);

    const translated = parts.map((p) => {
      const nk = this.normalizeKey(p).replace(/\\s+/g, ' ');
      return LABEL_TRANSLATIONS[nk] ?? p;
    });

    return translated.join(' / ') || '—';
  }

  private isContainerIdKey(key: string): boolean {
    const nk = this.normalizeKey(key).replace(/\\s+/g, ' ');
    return nk === 'idcontenedor' || nk === 'id contenedor' || nk === 'idcont' || nk === 'container id';
  }

  // xml helpers + misc
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

  private pickAnyString(obj: unknown, keys: string[]): string {
    if (!this.isRecord(obj)) return '';
    for (const k of keys) {
      if (k in obj && typeof obj[k] === 'string') return String(obj[k]).trim();
      const found = Object.keys(obj).find((x) => x.toLowerCase() === k.toLowerCase());
      if (found && typeof obj[found] === 'string') return obj[found] as string;
    }
    return '';
  }

  private getString(obj: Record<string, unknown>, key: string): string | null {
    if (key in obj && typeof obj[key] === 'string') return obj[key] as string;
    const found = Object.keys(obj).find((k) => k.toLowerCase() === key.toLowerCase());
    if (found && typeof obj[found] === 'string') return obj[found] as string;
    return null;
  }

  private parseDateString(value: unknown): string | null {
    if (!value) return null;
    const s = String(value).trim();
    if (!s) return null;
    const ms = Date.parse(s);
    return Number.isFinite(ms) ? s : null;
  }

  private safeString(v: unknown): string {
    try {
      return JSON.stringify(v);
    } catch {
      return String(v);
    }
  }

  private toTime(transaction: TransactionDto): number {
    const r = transaction as unknown as Record<string, unknown>;
    const iso = (r['eventDate'] as string | undefined) ?? (r['eta'] as string | undefined);
    return this.toTimeIso(iso);
  }

  private toTimeIso(iso: string | null | undefined): number {
    if (!iso) return 0;
    const ms = Date.parse(iso);
    return Number.isFinite(ms) ? ms : 0;
  }

  private isRecord(x: unknown): x is Record<string, unknown> {
    return !!x && typeof x === 'object' && !Array.isArray(x);
  }

  private normalizeContainerCode(value: unknown): string {
    return String(value ?? '').trim().toUpperCase();
  }
}

type ContainerHistoryEvent = {
  id: string;
  timestamp: Date;
  description: string;
  referenceCode?: string;
};

const DEFAULT_SHIPMENT_DETAILS: ContainerShipmentDetails = {
  containerId: '',
  size: '—',
  shippingLine: '—',
  pod: '—',
  goods: '—',
  booking: '—',
  weight: '—',
  gateIn: null,
  preAdvice: null,
  transportationCompany: '—',
  truckPlate: '—',
  driver: '—',
  seals: '—',
};

const LABEL_TRANSLATIONS: Record<string, string> = {
  contenedor: 'Container',
  idcontenedor: 'Container ID',
  'id contenedor': 'Container ID',
  tipo: 'Type',
  tamano: 'Size',
  tamaño: 'Size',
  estado: 'Status',
  naviera: 'Shipping line',
  'puerto carga': 'Port of loading',
  'puerto descarga': 'Port of discharge',
  'puerto origen': 'Port of origin',
  'puerto destino': 'Port of destination',
  destino: 'Destination',
  booking: 'Booking',
  mercancia: 'Goods',
  mercancía: 'Goods',
  'peso origen': 'Weight (origin)',
  transportista: 'Transport company',
  conductor: 'Driver',
  placa: 'Plate',
  observaciones: 'Notes',
  'sello-1': 'Seal 1',
  'sello-2': 'Seal 2',
  'sello aduana': 'Customs seal',
  contenido: 'Cargo items',
  item: 'Item',
  descripcion: 'Description',
  descripción: 'Description',
  cantidad: 'Qty',
  'peso unitario': 'Unit weight',
  'unidad peso': 'Weight unit',
  'fecha salida': 'Pre-advice date',
  'fecha llegada estimada': 'Gate in (ETA)',
};
