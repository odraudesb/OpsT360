import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, HostListener, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { catchError, forkJoin, map, of } from 'rxjs';

import { T360HeaderComponent } from '../../components/header/t360-header/t360-header';
import { APP_ROUTES } from '../../core/constants/app-routes.constants';
import type { ContainerRow } from '../../interfaces/transactions/container-row.interface';
import { TransactionsService } from '../../services/transactions/transactions.service';
import { clampPage, getResponsivePageSize } from '../../core/utils/pagination';
import type { ContainerDetailTab } from '@pages/container-detail/container-detail-tab.type';
import type { TransactionDetailsDto } from '@interfaces/transactions/transaction-details.interface';
import type { SecurityTransactionDetailsDto } from '@interfaces/transactions/security-transaction-details.interface';
import type { HistoricalTransactionDetailsDto } from '@interfaces/transactions/historical-transaction-details.interface';
import type { Field } from '@pages/container-detail/field.type';
import { buildXmlDetailsFields } from '@pages/container-detail/xml-details-fields';

type SearchByKey = 'code' | 'ship' | 'voyage';
type FilterValue = string | null;

type SecurityReportDevice = {
  id: string;
  title: string;
  recordDate: Date | null;
  fieldsCols: [Field[], Field[]];
};

type SecurityReportTimelineRow = {
  id: string;
  eventName: string;
  eventDate: Date | null;
  transactionId: number | null;
};

const STATIC_ROW_VALUES: readonly string[] = ['Exportation', 'Containerized'];

@Component({
  selector: 't360-containers-exportation-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, T360HeaderComponent],
  templateUrl: './containers-exportacion.html',
})
export class ContainersExportationComponent implements OnInit {
  readonly appRoutes = APP_ROUTES;
  pageSize = getResponsivePageSize();

  containers: ContainerRow[] = [];
  isLoading = true;

  currentPage = 1;
  searchTerm = '';
  selectedCategory: FilterValue = null;
  selectedCargoType: FilterValue = null;
  selectedStatus: FilterValue = null;

  // Search by dropdown
  isSearchByOpen = false;
  searchBy: SearchByKey | null = null;
  readonly searchByOptions: { key: SearchByKey; label: string }[] = [
    { key: 'code', label: 'Container id' },
    { key: 'ship', label: 'Vessel' },
    { key: 'voyage', label: 'Voyage' },
  ];

  readonly categoryOptions: readonly string[] = ['Exportation', 'Importation'];
  readonly cargoTypeOptions: readonly string[] = ['Containerized', 'CFS', 'BBK'];
  readonly statusOptions: readonly ContainerRow['status'][] = [
    'alerts',
    'inTransit',
    'inPort',
    'shipped',
    'outOfPort',
  ];

  // Acciones (1 popover)
  actionsOpenForId: string | null = null;
  selectedRowForActions: ContainerRow | null = null;
  actionsCardPosition: { top: string; left: string } = { top: '0px', left: '0px' };
  selectedRowForIssue: ContainerRow | null = null;
  showReportIssueModal = false;
  showSecurityReportModal = false;
  isSecurityReportLoading = false;
  selectedRowForReport: ContainerRow | null = null;
  securityReportGeneralFields: [Field[], Field[]] = [[], []];
  securityReportDevices: SecurityReportDevice[] = [];
  securityReportTimeline: SecurityReportTimelineRow[] = [];
  securityReportGeneratedAt: Date | null = null;
  readonly reportIssueTypes = [
    'Incoming Exit point',
    'Container damage',
    'Sensors failure',
    'Leaks',
    'Other',
  ] as const;
  readonly reportIssueCriticalityOptions = ['Urgent', 'High', 'Medium', 'Low'] as const;
  reportIssueForm: {
    issueType: (typeof ContainersExportationComponent.prototype.reportIssueTypes)[number] | null;
    criticality:
    (typeof ContainersExportationComponent.prototype.reportIssueCriticalityOptions)[number] | null;
    description: string;
    notifyTerminal: boolean;
    notifyReefer: boolean;
  } = {
      issueType: null,
      criticality: null,
      description: '',
      notifyTerminal: false,
      notifyReefer: false,
    };

  constructor(
    private readonly transactionsService: TransactionsService,
    private readonly cdr: ChangeDetectorRef,
    private readonly router: Router,
  ) { }

  get searchByLabel(): string {
    const opt = this.searchByOptions.find((o) => o.key === this.searchBy);
    return opt?.label ?? 'Search by';
  }

  @HostListener('document:click')
  onDocumentClick(): void {
    this.isSearchByOpen = false;
    this.closeRowActions();
  }

  @HostListener('window:resize')
  onResize(): void {
    const nextSize = getResponsivePageSize();
    if (nextSize !== this.pageSize) {
      this.pageSize = nextSize;
    }
    this.currentPage = clampPage(this.currentPage, this.totalPages);
  }

  get filteredContainers(): ContainerRow[] {
    const term = this.searchTerm.trim().toLowerCase();
    const hasFilters =
      Boolean(this.selectedCategory) ||
      Boolean(this.selectedCargoType) ||
      Boolean(this.selectedStatus);

    return this.containers
      .filter((row) => this.matchesFilter(row.category, this.selectedCategory))
      .filter((row) => this.matchesFilter(row.cargoType, this.selectedCargoType))
      .filter((row) => this.matchesFilter(row.status, this.selectedStatus))
      .filter((row) => {
        if (!term) return true;

        const searchPool: (string | number | null | undefined)[] =
          !hasFilters && !this.searchBy
            ? [
              row.code,
              row.cargoType,
              row.category,
              row.ship ?? '',
              row.voyage ?? '',
              this.formatStatus(row.status),
              row.lastEvent ?? '',
              row.lastEventDate ?? '',
            ]
            : this.searchBy === 'code'
              ? [row.code]
              : this.searchBy === 'ship'
                ? [row.ship ?? '']
                : this.searchBy === 'voyage'
                  ? [row.voyage ?? '']
                  : [row.code, row.ship ?? '', row.voyage ?? ''];

        return [...searchPool, ...STATIC_ROW_VALUES, row.category, row.cargoType].some(
          (value) => value != null && value.toString().toLowerCase().includes(term),
        );
      });
  }

  get totalPages(): number {
    return Math.max(Math.ceil(this.filteredContainers.length / this.pageSize), 1);
  }

  get paginatedContainers(): ContainerRow[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredContainers.slice(start, start + this.pageSize);
  }

  ngOnInit(): void {
    this.loadContainers();
  }

  nextPage(): void {
    this.currentPage = clampPage(this.currentPage + 1, this.totalPages);
  }

  previousPage(): void {
    this.currentPage = clampPage(this.currentPage - 1, this.totalPages);
  }

  onSearchTermChange(): void {
    this.currentPage = 1;
  }

  onSearchClick(): void {
    this.currentPage = 1;
  }

  // 🔄 Reload: resetea filtros + search
  resetSearchOnly(ev?: MouseEvent): void {
    ev?.stopPropagation();
    this.searchTerm = '';
    this.searchBy = null;
    this.isSearchByOpen = false;
    this.selectedCategory = null;
    this.selectedCargoType = null;
    this.selectedStatus = null;
    this.currentPage = 1;
  }

  trackById(_: number, item: ContainerRow): string {
    return item.id;
  }

  buildDetailQueryParams(row: ContainerRow): Record<string, unknown> {
    return {
      tx: row.transactionId,
      ...(row.entityId ? { entityId: row.entityId } : {}),
      status: this.formatStatus(row.status),
      ...(row.isAlert ? { alert: 1 } : {}),
    };
  }

  // Search by dropdown
  toggleSearchBy(ev: MouseEvent): void {
    ev.stopPropagation();
    this.isSearchByOpen = !this.isSearchByOpen;
    this.closeRowActions();
  }

  selectSearchBy(ev: MouseEvent, key: SearchByKey): void {
    ev.stopPropagation();
    this.searchBy = key;
    this.currentPage = 1;
    this.isSearchByOpen = false;
  }

  clearSearchBy(ev: MouseEvent): void {
    ev.stopPropagation();
    this.searchBy = null;
    this.isSearchByOpen = false;
    this.currentPage = 1;
  }

  closeSearchBy(ev: MouseEvent): void {
    ev.stopPropagation();
    this.isSearchByOpen = false;
  }

  // Row actions
  toggleRowActions(ev: MouseEvent, row: ContainerRow): void {
    ev.stopPropagation();
    this.isSearchByOpen = false;

    const button = ev.currentTarget as HTMLElement | null;
    const isSame = this.actionsOpenForId === row.id;
    if (isSame) {
      this.closeRowActions();
      return;
    }

    this.actionsOpenForId = row.id;
    this.selectedRowForActions = row;
    if (button) {
      this.updateActionsCardPosition(button);
    }
  }

  closeRowActions(ev?: MouseEvent): void {
    ev?.stopPropagation();
    this.actionsOpenForId = null;
    this.selectedRowForActions = null;
  }

  private updateActionsCardPosition(button: HTMLElement): void {
    const rect = button.getBoundingClientRect();
    const cardWidth = 256;
    const cardHeight = 260;
    const padding = 16;
    const horizontalOffset = 12;

    let left = rect.left - cardWidth - horizontalOffset;
    if (left < padding) {
      left = rect.right + horizontalOffset;
    }

    let top = rect.top + rect.height / 2 - cardHeight / 2;
    top = Math.max(padding, Math.min(top, window.innerHeight - cardHeight - padding));

    this.actionsCardPosition = {
      top: `${Math.round(top)}px`,
      left: `${Math.round(left)}px`,
    };
  }

  openReportIssueModal(ev: MouseEvent, row: ContainerRow): void {
    ev.stopPropagation();
    this.closeRowActions();
    this.selectedRowForIssue = row;
    this.resetReportIssueForm();
    this.showReportIssueModal = true;
  }

  closeReportIssueModal(): void {
    this.showReportIssueModal = false;
    this.selectedRowForIssue = null;
  }

  submitReportIssue(): void {
    this.closeReportIssueModal();
  }

  openCargoSecurityReport(ev: MouseEvent, row: ContainerRow): void {
    ev.stopPropagation();
    this.closeRowActions();
    this.selectedRowForReport = row;
    this.showSecurityReportModal = true;
    this.securityReportGeneratedAt = new Date();
    this.loadSecurityReportData(row);
  }

  closeSecurityReportModal(): void {
    this.showSecurityReportModal = false;
    this.selectedRowForReport = null;
    this.securityReportGeneralFields = [[], []];
    this.securityReportDevices = [];
    this.securityReportTimeline = [];
    this.securityReportGeneratedAt = null;
  }

  getReportTableRows(columns: [Field[], Field[]]): Array<{ left: Field | null; right: Field | null }> {
    const [left, right] = columns;
    const maxLength = Math.max(left.length, right.length);
    return Array.from({ length: maxLength }, (_, index) => ({
      left: left[index] ?? null,
      right: right[index] ?? null,
    }));
  }

  downloadSecurityReportPdf(): void {
    const content = document.getElementById('cargo-security-report-content');
    if (!content) return;

    const container = this.selectedRowForReport?.code ?? 'container';
    const title = `Cargo Security Report - ${container}`;
    const styles = `
      body { font-family: Arial, sans-serif; padding: 24px; color: #111827; }
      h2 { font-size: 16px; margin: 18px 0 8px; }
      h3 { font-size: 14px; margin: 16px 0 8px; }
      .report-header { border: 1px solid #e5e7eb; padding: 16px; border-radius: 8px; }
      .report-title { color: #3758F9; font-weight: 700; font-size: 16px; }
      .report-table { width: 100%; border-collapse: collapse; margin-bottom: 12px; }
      .report-table td { border: 1px solid #e5e7eb; padding: 6px 8px; font-size: 12px; text-align: left; vertical-align: top; }
      .report-table th { border: 1px solid #e5e7eb; padding: 6px 8px; font-size: 12px; text-align: left; background: #f8fafc; }
      .report-table__label { font-weight: 700; color: #111827; width: 22%; }
      .report-table__value { color: #4b5563; }
      .report-section-title { font-size: 14px; font-weight: 700; margin: 16px 0 8px; color: #111827; }
      .report-device-meta { font-size: 12px; color: #6b7280; margin-bottom: 6px; }
      .report-card { border: 1px solid #e5e7eb; border-radius: 8px; padding: 12px; }
      @media print { body { padding: 0; } }
    `;

    const html = `<!doctype html>
      <html>
        <head>
          <title>${title}</title>
          <style>${styles}</style>
        </head>
        <body>
          ${content.innerHTML}
        </body>
      </html>`;

    const iframe = document.createElement('iframe');
    iframe.style.position = 'fixed';
    iframe.style.right = '0';
    iframe.style.bottom = '0';
    iframe.style.width = '0';
    iframe.style.height = '0';
    iframe.style.border = '0';
    iframe.setAttribute('aria-hidden', 'true');
    document.body.appendChild(iframe);

    const iframeDocument = iframe.contentDocument ?? iframe.contentWindow?.document;
    if (!iframeDocument) {
      document.body.removeChild(iframe);
      return;
    }

    iframe.onload = () => {
      iframe.contentWindow?.focus();
      iframe.contentWindow?.print();
      setTimeout(() => {
        document.body.removeChild(iframe);
      }, 1000);
    };

    iframeDocument.open();
    iframeDocument.write(html);
    iframeDocument.close();
  }

  private loadSecurityReportData(row: ContainerRow): void {
    this.isSecurityReportLoading = true;
    const entityId = row.entityId ?? null;

    const general$ = this.transactionsService.getTransactionDetails(row.transactionId).pipe(
        catchError((error: unknown) => {
          console.error('[SecurityReport] getTransactionDetails error', error);
          return of(null);
        }),
      );

    const security$ = entityId
      ? this.transactionsService.getSecurityTransactionDetails({ entityId }).pipe(
        catchError((error: unknown) => {
          console.error('[SecurityReport] getSecurityTransactionDetails error', error);
          return of([]);
        }),
      )
      : of([]);

    const history$ = entityId
      ? this.transactionsService.getHistoricalTransactionDetails({ entityId }).pipe(
        catchError((error: unknown) => {
          console.error('[SecurityReport] getHistoricalTransactionDetails error', error);
          return of([]);
        }),
      )
      : of([]);

    forkJoin({ general: general$, security: security$, history: history$ }).subscribe({
      next: ({ general, security, history }) => {
        this.securityReportGeneralFields = this.splitInto2Columns(
          this.buildGeneralFields(general),
        );
        this.securityReportDevices = this.buildSecurityDevices(security);
        this.securityReportTimeline = this.buildTimeline(history);
        this.isSecurityReportLoading = false;
        this.cdr.detectChanges();
      },
      error: (error: unknown) => {
        console.error('[SecurityReport] load error', error);
        this.isSecurityReportLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  private buildGeneralFields(details: TransactionDetailsDto | null): Field[] {
    if (!details) return [{ label: '—', value: '—' }];
    const fields = buildXmlDetailsFields(details.xmlDetails ?? {});
    if (this.isPlaceholderFields(fields)) {
      const { xmlDetails, ...rest } = details;
      return buildXmlDetailsFields(rest);
    }
    return fields;
  }

  private buildSecurityDevices(details: SecurityTransactionDetailsDto[]): SecurityReportDevice[] {
    return (details ?? []).map((detail, index) => {
      const fields = this.buildSecurityFields(detail);
      const title = detail.eventName ?? `Security device ${index + 1}`;
      const recordDate = this.toDate(detail.eventDate ?? detail.recordDate ?? null);
      const id = detail.transactionId ? String(detail.transactionId) : `security-${index}`;
      return {
        id,
        title,
        recordDate,
        fieldsCols: this.splitInto2Columns(fields),
      };
    });
  }

  private buildSecurityFields(detail: SecurityTransactionDetailsDto): Field[] {
    const fields = buildXmlDetailsFields(detail.xmlDetails ?? {});
    if (this.isPlaceholderFields(fields)) {
      const { xmlDetails, ...rest } = detail as Record<string, unknown>;
      return buildXmlDetailsFields(rest);
    }
    return fields;
  }

  private buildTimeline(list: HistoricalTransactionDetailsDto[]): SecurityReportTimelineRow[] {
    const sorted = [...(list ?? [])].sort(
      (a, b) =>
        this.toTime(b.eventDate ?? b.recordDate ?? null) -
        this.toTime(a.eventDate ?? a.recordDate ?? null),
    );

    return sorted.map((item, index) => {
      const eventName =
        item.eventName ?? (item.eventId != null ? `Evento ${item.eventId}` : `Evento ${index + 1}`);
      return {
        id: item.transactionId ? String(item.transactionId) : `history-${index}`,
        eventName,
        eventDate: this.toDate(item.eventDate ?? item.recordDate ?? null),
        transactionId: item.transactionId ?? null,
      };
    });
  }

  private splitInto2Columns(fields: Field[]): [Field[], Field[]] {
    const left: Field[] = [];
    const right: Field[] = [];
    fields.forEach((field, index) => (index % 2 === 0 ? left : right).push(field));
    return [left, right];
  }

  private isPlaceholderFields(fields: Field[]): boolean {
    return fields.length === 1 && fields[0].label === '—' && fields[0].value === '—';
  }

  private toDate(value: string | null | undefined): Date | null {
    const ms = this.toTime(value);
    return ms ? new Date(ms) : null;
  }

  // ✅ Ahora apunta a los TITULOS (autoscroll al título)
  private sectionIdForTab(tab: ContainerDetailTab): string {
    const map: Record<ContainerDetailTab, string> = {
      trackAndTrace: 'track-and-trace-title',
      shipmentDocumentation: 'shipment-documentation-title',
      cargoSecurityControl: 'cargo-security-title',
      reeferTrace: 'reefer-trace-title',
      historyEvents: 'history-events-title',
      alertDetail: 'alert-detail-title',
    };
    return map[tab] ?? 'track-and-trace-title';
  }

  goToDetail(
    ev: MouseEvent,
    row: ContainerRow,
    tab?: ContainerDetailTab,
    section?: string,
    extraQuery?: Record<string, unknown>,
  ): void {
    ev.stopPropagation();
    this.closeRowActions();

    const sectionId = section ?? (tab ? this.sectionIdForTab(tab) : undefined);

    this.router.navigate(['/', APP_ROUTES.CONTAINER_DETAIL, row.code], {
      queryParams: {
        ...this.buildDetailQueryParams(row),
        ...(tab ? { tab } : {}),
        ...(sectionId ? { section: sectionId } : {}),
        ...(extraQuery ?? {}),
      },
    });
  }

  // ✅ Last Event => abre History Events, scrollea al título, y “busca” el TX dentro de History Events
  goToLastEvent(ev: MouseEvent, row: ContainerRow): void {
    this.goToDetail(ev, row, 'historyEvents', 'history-events-title', {
      focusTx: row.transactionId,
    });
  }

  formatStatus(status: ContainerRow['status']): string {
    const normalized = status.toString();
    const map: Record<string, string> = {
      shipped: 'Shipped',
      inPort: 'In Port',
      inTransit: 'In Transit',
      outOfPort: 'Out of Port',
      alerts: 'Alert',
      unknown: 'Unknown',
    };
    return map[normalized] ?? normalized.replace(/_/g, ' ').replace(/\b\w/g, (m) => m.toUpperCase());
  }

  private resetReportIssueForm(): void {
    this.reportIssueForm = {
      issueType: null,
      criticality: null,
      description: '',
      notifyTerminal: false,
      notifyReefer: false,
    };
  }

  private loadContainers(): void {
    this.isLoading = true;

    this.transactionsService
      .getAllTransactions()
      .pipe(map((transactions) => this.buildLatestRowsByContainer(transactions as any[])))
      .subscribe({
        next: (rows: ContainerRow[]) => {
          this.containers = rows;
          this.isLoading = false;
          this.cdr.detectChanges();
          this.checkAlertDetails(rows);
        },
        error: (error: unknown) => {
          console.error('[Transactions] error loading rows', error);
          this.isLoading = false;
          this.cdr.detectChanges();
        },
      });
  }

  private checkAlertDetails(rows: ContainerRow[]): void {
    if (!rows.length) return;

    const alertChecks = rows.map((row) => {
      if (!row.entityId) {
        return of({ id: row.id, hasAlert: false });
      }

      return this.transactionsService.getAlertDetails({ entityId: row.entityId }).pipe(
        map((details) => ({
          id: row.id,
          hasAlert: this.hasAlertDetails(details),
        })),
        catchError((error: unknown) => {
          if (!this.isNotFoundError(error)) {
            console.error('[Transactions] error loading alert details', error);
          }
          return of({ id: row.id, hasAlert: false });
        }),
      );
    });

    forkJoin(alertChecks).subscribe((results) => {
      const alertMap = new Map(results.map((result) => [result.id, result.hasAlert]));

      this.containers = this.containers.map((row) => {
        const hasAlert = alertMap.get(row.id);
        if (!hasAlert) return row;

        return {
          ...row,
          isAlert: true,
          status: 'alerts',
        };
      });

      this.cdr.detectChanges();
    });
  }

  private hasAlertDetails(details: unknown): boolean {
    if (Array.isArray(details)) return details.length > 0;
    if (!details || typeof details !== 'object') return false;
    return Object.keys(details as Record<string, unknown>).length > 0;
  }

  private isNotFoundError(error: unknown): boolean {
    const status = (error as { response?: { status?: number } })?.response?.status;
    return status === 404;
  }


  private buildLatestRowsByContainer(transactions: any[]): ContainerRow[] {
    const byContainer = new Map<string, any[]>();

    for (const t of transactions) {
      const key = this.normalizeContainerCode(t?.entityType);
      if (!key) continue;
      if (!byContainer.has(key)) byContainer.set(key, []);
      byContainer.get(key)!.push(t);
    }

    const rows: ContainerRow[] = [];

    byContainer.forEach((list, code) => {
      list.sort(
        (a, b) =>
          this.toTime(b?.eventDate ?? b?.recordDate ?? b?.eta) -
          this.toTime(a?.eventDate ?? a?.recordDate ?? a?.eta),
      );
      const latest = list[0];
      if (!latest) return;

      const baseStatus = latest.isAlert ? 'alerts' : this.mapStatus(latest.locationStatusId ?? null);

      rows.push({
        id: `${code}-${latest.transactionId}`,
        code,
        transactionId: latest.transactionId,
        status: baseStatus,
        category: this.mapCategory(latest.categoryId ?? null),
        cargoType: 'Containerized',
        eta: latest.eta ?? null,
        lastEventDate: latest.eventDate ?? latest.recordDate ?? null,
        ship: latest.ship ?? latest.vessel ?? null,
        voyage: latest.voyage ?? latest.Voyage ?? latest.voyageNumber ?? null,
        lastEvent: latest.eventDescription ?? '—',
        entityId: latest.entityId ?? undefined,
        isAlert: Boolean(latest.isAlert),
      });
    });

    rows.sort((a, b) => this.toTime(b.lastEventDate) - this.toTime(a.lastEventDate));
    return rows;
  }

  private toTime(value: string | null | undefined): number {
    if (!value) return 0;
    const ms = Date.parse(value);
    return Number.isFinite(ms) ? ms : 0;
  }

  private normalizeContainerCode(value: unknown): string {
    return String(value ?? '').trim().toUpperCase();
  }

  private mapStatus(locationStatusId: number | null): ContainerRow['status'] {
    const map: Record<number, ContainerRow['status']> = {
      1: 'shipped',
      2: 'inPort',
      3: 'inTransit',
      4: 'outOfPort',
      5: 'alerts',
    };
    return map[locationStatusId ?? 0] ?? 'unknown';
  }

  private mapCategory(categoryId: number | null): string {
    const map: Record<number, string> = {
      1: 'Exportation',
      2: 'Importation',
    };
    return map[categoryId ?? 1] ?? 'Exportation';
  }

  private matchesFilter(
    value: string | ContainerRow['status'],
    selected: FilterValue | ContainerRow['status'] | null,
  ): boolean {
    if (!selected) return true;
    return (value ?? '').toString().toLowerCase() === selected.toString().toLowerCase();
  }
}
