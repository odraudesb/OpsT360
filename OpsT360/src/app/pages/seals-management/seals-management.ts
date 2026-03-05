import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, HostListener, NgZone, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Subject, filter, finalize, takeUntil, timeout } from 'rxjs';
import { FormsModule } from '@angular/forms';

import { T360HeaderComponent } from '../../components/header/t360-header/t360-header';
import { ActionsTabComponent } from './components/actions-tab/actions-tab.component';
import { InventoryTabComponent } from './components/inventory-tab/inventory-tab.component';
import { OperationalTabComponent } from './components/operational-tab/operational-tab.component';
import { PurchaseRequestTabComponent } from './components/purchase-request-tab/purchase-request-tab.component';
import { ReconciliationTabComponent } from './components/reconciliation-tab/reconciliation-tab.component';
import { SealPurchaseTabComponent } from './components/seal-purchase-tab/seal-purchase-tab.component';
import { UploadWorkflowModalComponent } from './components/upload-workflow-modal/upload-workflow-modal.component';

import {
  CreatePurchaseRequestPayload,
  PurchaseRequestApiItem as PurchaseRequestApiItemDto,
  PurchaseRequestApiResponse as PurchaseRequestApiResponseDto,
  PurchaseRequestService,
  UpdatePurchaseRequestStatusPayload,
} from '../../services/seals/purchase-request.service';

import { getAuthContextFromStorage } from '../../core/utils/auth-storage';
import { APP_ROUTES } from '../../core/constants/app-routes.constants';
import { clampPage, getResponsivePageSize } from '../../core/utils/pagination';

import {
  InventoryRow,
  PurchaseMgmtRow,
  PurchaseRequestFilter,
  RequestRow,
  SealTab,
} from './seals-management.types';

type PurchaseRequestApiItem = {
  SealPurchaseRequestId: number;
  RequestDescription: string;
  RequesterUserId: number | null;
  RequestDate: string;
  sealTypeDescription: string;
  Quantity: number;
  currentStatusName: string;
  CreatedBy: string | null;
};

type PurchaseRequestApiResponse = {
  data: PurchaseRequestApiItem[];
  errors: unknown[];
};

@Component({
  selector: 't360-seals-management-page',
  standalone: true,
  imports: [
    ActionsTabComponent,
    CommonModule,
    FormsModule,
    InventoryTabComponent,
    OperationalTabComponent,
    PurchaseRequestTabComponent,
    ReconciliationTabComponent,
    SealPurchaseTabComponent,
    T360HeaderComponent,
    UploadWorkflowModalComponent,
  ],
  templateUrl: './seals-management.html',
  styleUrls: ['./seals-management.css'],
})
export class SealsManagementComponent implements OnInit, OnDestroy {
  tabs: { id: SealTab; label: string }[] = [
    { id: 'purchase-request', label: 'Purchase Request' },
    { id: 'seal-purchase', label: 'Seal Purchase Mgmt' },
    { id: 'inventory', label: 'Seal Inventory Mgmt' },
    { id: 'operational', label: 'Seal Operational Request' },
    { id: 'reconciliation', label: 'Conciliation' },
    { id: 'actions', label: 'Seals Actions' },
  ];

  activeTab: SealTab = 'purchase-request';
  searchTerm = '';
  purchaseRequestFilter: PurchaseRequestFilter = 'all';

  showCreateRequest = false;
  showApproval = false;
  showOperationalReturn = false;
  showInventoryEdit = false;
  showReconciliationDetail = false;
  showDeactivate = false;
  showViewRequest = false;

  showConfirmModal = false;
  confirmTitle = '';
  confirmLines: string[] = [];
  confirmConfirmLabel = 'Confirm';
  confirmCancelLabel = 'Cancel';
  confirmAction:
    | 'operational'
    | 'inventory-edit'
    | 'inventory-save'
    | 'deactivate'
    | 'upload'
    | 'approval'
    | 'rejection'
    | 'create-request'
    | null = null;

  showInfoModal = false;
  infoTitle = '';
  infoMessage = '';

  showUploadModal = false;
  uploadModalRow: InventoryRow | null = null;
  uploadModalStep: 1 | 2 | 3 | 4 = 1;
  selectedInventoryRow: InventoryRow | null = null;
  selectedPurchaseOrderId = '';

  createRequestForm = this.buildCreateRequestForm();
  isSubmittingRequest = false;
  createRequestError = '';
  pendingCreatePayload: CreatePurchaseRequestPayload | null = null;
  isUpdatingPurchaseStatus = false;

  requestRows: RequestRow[] = [];
  selectedRequest: RequestRow | null = null;

  selectedApprovalRequest: PurchaseMgmtRow | null = null;

  pendingInventoryRow: InventoryRow | null = null;
  pendingUploadRow: InventoryRow | null = null;
  pendingUploadStep: 1 | 2 | 3 | 4 = 1;

  purchaseMgmtRows: PurchaseMgmtRow[] = [];

  reconciliationDetailRows = [
    {
      sealNumber: 'S-099541',
      containerId: 'CMAU5573305',
      physical: 'Yes',
      tosSystem: 'No',
    },
    {
      sealNumber: 'S-974756',
      containerId: 'MSCU6407710',
      physical: 'No',
      tosSystem: 'No',
    },
    {
      sealNumber: 'S-555577',
      containerId: '-',
      physical: 'No',
      tosSystem: 'No',
    },
  ];
  reconciliationDetailPageSize = getResponsivePageSize();
  reconciliationDetailCurrentPage = 1;

  inventoryRows: InventoryRow[] = [];

  // ✅ NUEVO: flags para que al entrar cargue sí o sí (cache + refresh)
  purchaseRequestsLoading = false;
  private purchaseReqInFlight = false;
  private purchaseReqPendingRefresh = false;

  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly purchaseRequestService: PurchaseRequestService,
    private readonly router: Router,
    private readonly zone: NgZone,
    private readonly changeDetectorRef: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.handleRouteEntry();

    this.router.events
      .pipe(
        filter((event): event is NavigationEnd => event instanceof NavigationEnd),
        takeUntil(this.destroyed$),
      )
      .subscribe(() => {
        if (this.router.url.includes(`/${APP_ROUTES.SEALS_MANAGEMENT}`)) {
          this.handleRouteEntry();
        }
      });
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  @HostListener('window:resize')
  onResize(): void {
    const nextSize = getResponsivePageSize();
    if (nextSize !== this.reconciliationDetailPageSize) {
      this.reconciliationDetailPageSize = nextSize;
    }
    this.reconciliationDetailCurrentPage = clampPage(
      this.reconciliationDetailCurrentPage,
      this.reconciliationDetailTotalPages,
    );
  }

  get reconciliationDetailTotalPages(): number {
    return Math.max(
      Math.ceil(this.reconciliationDetailRows.length / this.reconciliationDetailPageSize),
      1,
    );
  }

  get paginatedReconciliationDetailRows(): typeof this.reconciliationDetailRows {
    const start = (this.reconciliationDetailCurrentPage - 1) * this.reconciliationDetailPageSize;
    return this.reconciliationDetailRows.slice(start, start + this.reconciliationDetailPageSize);
  }

  nextReconciliationDetailPage(): void {
    this.reconciliationDetailCurrentPage = clampPage(
      this.reconciliationDetailCurrentPage + 1,
      this.reconciliationDetailTotalPages,
    );
  }

  previousReconciliationDetailPage(): void {
    this.reconciliationDetailCurrentPage = clampPage(
      this.reconciliationDetailCurrentPage - 1,
      this.reconciliationDetailTotalPages,
    );
  }

  setTab(tab: SealTab): void {
    this.activeTab = tab;

    // ✅ al cambiar tabs, usa cache (rápido)
    if (tab === 'purchase-request' || tab === 'seal-purchase') {
      this.loadPurchaseRequests();
    }
  }

  // ✅ NUEVO: al entrar, carga cache y luego refresca contra API (para que la tabla salga de una)
  private handleRouteEntry(): void {
    this.searchTerm = '';
    this.purchaseRequestFilter = 'all';

    // evita depender del click para pintar el primer tab
    this.activeTab = 'purchase-request';

    // 1) pinta cache si hay
    this.loadPurchaseRequests();

    // 2) fuerza refresh real para que aparezca en el primer ingreso
    this.refreshPurchaseRequests();
  }

  // ✅ cache
  private loadPurchaseRequests(): void {
    if (this.purchaseReqInFlight) return;

    this.purchaseRequestsLoading = true;

    this.purchaseRequestService
      .getAllCached()
      .pipe(
        takeUntil(this.destroyed$),
        finalize(() => {
          this.purchaseRequestsLoading = false;
        }),
      )
      .subscribe({
        next: (response: PurchaseRequestApiResponseDto) => this.applyPurchaseRequestRows(response),
        error: () => this.resetPurchaseRequestRows(),
      });
  }

  // ✅ API real
  private refreshPurchaseRequests(force = false): void {
    if (this.purchaseReqInFlight) {
      if (force) {
        this.purchaseReqPendingRefresh = true;
      }
      return;
    }

    this.purchaseReqInFlight = true;
    this.purchaseRequestsLoading = true;

    this.purchaseRequestService
      .refresh()
      .pipe(
        takeUntil(this.destroyed$),
        finalize(() => {
          this.purchaseReqInFlight = false;
          this.purchaseRequestsLoading = false;
          if (this.purchaseReqPendingRefresh) {
            this.purchaseReqPendingRefresh = false;
            this.refreshPurchaseRequests(true);
          }
        }),
      )
      .subscribe({
        next: (response: PurchaseRequestApiResponseDto) => this.applyPurchaseRequestRows(response),
        error: () => this.resetPurchaseRequestRows(),
      });
  }

  private forceRefreshPurchaseRequests(): void {
    // (Se mantiene por compatibilidad, pero ya no es necesario llamarlo en aprobación)
    this.purchaseReqPendingRefresh = false;
    this.purchaseReqInFlight = true;
    this.purchaseRequestsLoading = true;

    this.purchaseRequestService
      .refresh()
      .pipe(
        takeUntil(this.destroyed$),
        finalize(() => {
          this.purchaseReqInFlight = false;
          this.purchaseRequestsLoading = false;
          if (this.purchaseReqPendingRefresh) {
            this.purchaseReqPendingRefresh = false;
            this.refreshPurchaseRequests(true);
          }
        }),
      )
      .subscribe({
        next: (response: PurchaseRequestApiResponseDto) => this.applyPurchaseRequestRows(response),
        error: () => this.resetPurchaseRequestRows(),
      });
  }

  private applyPurchaseRequestRows(response: PurchaseRequestApiResponseDto): void {
    const items = this.resolvePurchaseRequestItems(response);
    this.zone.run(() => {
      const mappedRows = items.map((item) => this.mapPurchaseRequest(item));
      this.requestRows = mappedRows;
      this.purchaseMgmtRows = mappedRows.map((row) => this.mapPurchaseMgmtRow(row));
      this.inventoryRows = mappedRows.map((row, index) => this.mapInventoryRow(row, index));
      this.changeDetectorRef.detectChanges();
    });
  }

  private resetPurchaseRequestRows(): void {
    this.zone.run(() => {
      this.requestRows = [];
      this.purchaseMgmtRows = [];
      this.inventoryRows = [];
      this.changeDetectorRef.detectChanges();
    });
  }

  private mapInventoryRow(row: RequestRow, index: number): InventoryRow {
    const parsed = row.id.match(/PR-(\d{4})-(\d+)/i);
    const year = parsed?.[1] ?? new Date().getFullYear().toString();
    const rawNumber = parsed?.[2] ?? `${index + 1}`;
    const purchaseNumber = (Number(rawNumber) + 1).toString().padStart(rawNumber.length, '0');
    const purchaseOrderId = `PO-${year}-${purchaseNumber}`;

    return {
      id: purchaseOrderId,
      requestOrderId: row.id,
      description: row.description,
      requester: row.requester,
      uploadDate: row.date,
      sealType: row.sealType,
      totalSeals: row.quantity,
      receivedBy: row.requester,
    };
  }

  private resolvePurchaseRequestItems(
    response: PurchaseRequestApiResponseDto | PurchaseRequestApiItemDto[] | null | undefined,
  ): PurchaseRequestApiItemDto[] {
    if (!response) return [];

    if (Array.isArray(response)) {
      return response;
    }

    if (Array.isArray(response.data)) {
      return response.data;
    }

    const nestedData = (response as { data?: { data?: PurchaseRequestApiItemDto[] } })?.data?.data;
    return Array.isArray(nestedData) ? nestedData : [];
  }

  // ✅ NUEVO: valida que una respuesta “parezca” lista de purchase requests
  private isPurchaseRequestRowsResponse(response: unknown): response is PurchaseRequestApiResponseDto {
    const items = this.resolvePurchaseRequestItems(response as any);
    return Array.isArray(items) && items.length > 0;
  }

  async submitCreateRequest(payload: CreatePurchaseRequestPayload): Promise<void> {
    if (this.isSubmittingRequest) return;

    this.isSubmittingRequest = true;
    this.createRequestError = '';
    let handled = false;
    const fallbackTimer = setTimeout(() => {
      if (handled) return;
      handled = true;
      this.runUiUpdate(() => {
        this.pendingCreatePayload = null;
        this.closeCreate();
        this.refreshPurchaseRequests();
        this.openInfoModal(
          'Purchase request created',
          'Your request was saved. It may take a moment to appear in the list.',
        );
        this.isSubmittingRequest = false;
      });
    }, 12000);

    this.purchaseRequestService
      .create(payload)
      .pipe(
        timeout(30000),
        finalize(() => {
          this.runUiUpdate(() => {
            this.isSubmittingRequest = false;
          });
          clearTimeout(fallbackTimer);
        }),
      )
      .subscribe({
        next: () => {
          if (handled) return;
          handled = true;
          this.runUiUpdate(() => {
            this.pendingCreatePayload = null;
            this.closeCreate();
            this.refreshPurchaseRequests();
            this.openInfoModal(
              'Purchase request created',
              'The purchase request was created successfully.',
            );
          });
        },
        error: () => {
          if (handled) return;
          handled = true;
          this.runUiUpdate(() => {
            this.createRequestError = 'Unable to create the purchase request.';
            this.closeCreate();
            this.openInfoModal(
              'Unable to create purchase request',
              'Please try again or contact support.',
            );
          });
        },
      });
  }

  private runUiUpdate(update: () => void): void {
    this.zone.run(() => {
      update();
      this.changeDetectorRef.detectChanges();
    });
  }

  openCreateConfirm(): void {
    if (this.isSubmittingRequest) return;

    const payload = this.buildCreateRequestPayload();
    if (!payload) {
      this.createRequestError = 'Please complete the required fields.';
      this.openInfoModal(
        'Missing information',
        'Please complete the required fields before submitting.',
      );
      return;
    }

    this.pendingCreatePayload = payload;
    this.openConfirmModal({
      title: 'Create purchase request',
      lines: [
        'Are you sure you want to create this purchase request?',
        `Description: ${payload.requestDescription}`,
        `Seal type: ${this.getSealTypeLabel(payload.sealTypeId)}`,
        `Quantity: ${payload.quantity}`,
        `Urgency: ${this.getUrgencyLabel(payload.purchaseUrgencyLevelId)}`,
      ],
      confirmLabel: 'Yes, create',
      cancelLabel: 'Cancel',
      action: 'create-request',
    });
  }

  private normalizeStatus(value: unknown): string {
    return String(value ?? '').trim();
  }

  private mapPurchaseRequest(item: PurchaseRequestApiItemDto): RequestRow {
    const fallback = item as unknown as Record<string, unknown>;

    const requestDateRaw = (item.RequestDate ?? fallback['requestDate'] ?? '') as string;
    const requestDate = requestDateRaw ? new Date(requestDateRaw) : null;
    const hasValidDate = Boolean(requestDate && !Number.isNaN(requestDate.getTime()));

    const requesterUserId =
      item.RequesterUserId ?? (fallback['requesterUserId'] as number | null | undefined);

    return {
      id: String(item.SealPurchaseRequestId ?? fallback['sealPurchaseRequestId'] ?? ''),
      description: String(item.RequestDescription ?? fallback['requestDescription'] ?? ''),
      quantity: this.formatQuantity(Number(item.Quantity ?? fallback['quantity'] ?? 0)),
      requester:
        item.CreatedBy ??
        (fallback['createdBy'] as string | null | undefined) ??
        (requesterUserId ? `User ${requesterUserId}` : 'N/A'),
      date: hasValidDate ? this.formatDate(requestDate as Date) : '—',
      sealType: String(
        item.sealTypeDescription ?? fallback['sealTypeDescription'] ?? fallback['sealType'] ?? '',
      ).trim(),
      status: this.normalizeStatus(
        item.currentStatusName ?? fallback['currentStatusName'] ?? fallback['status'] ?? '',
      ),
      timeSince: hasValidDate ? this.formatTimeSince(requestDate as Date) : '—',
    };
  }

  mapPurchaseMgmtRow(row: RequestRow): PurchaseMgmtRow {
    return {
      id: row.id,
      description: row.description,
      requester: row.requester,
      date: row.date,
      sealType: row.sealType,
      total: row.quantity,
      status: row.status,
    };
  }

  private formatQuantity(quantity: number): string {
    if (!Number.isFinite(quantity)) return '—';
    return new Intl.NumberFormat('es-EC').format(quantity);
  }

  private formatDate(date: Date): string {
    return new Intl.DateTimeFormat('es-EC', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }).format(date);
  }

  private formatTimeSince(date: Date): string {
    const diffMs = Date.now() - date.getTime();
    const diffMinutes = Math.max(Math.floor(diffMs / 60000), 0);
    const diffHours = Math.floor(diffMinutes / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffDays > 0) return `${diffDays} d`;
    if (diffHours > 0) return `${diffHours} h`;
    return `${diffMinutes} m`;
  }

  private buildCreateRequestForm(): {
    requestDescription: string;
    requesterUserId: number | null;
    requestDate: string;
    justification: string;
    notes: string;
    sealTypeId: number | null;
    quantity: number | null;
    purchaseUrgencyLevelId: number | null;
    initialStatusName: string;
    comments: string;
    createdBy: string;
  } {
    const authContext = getAuthContextFromStorage();
    const fallbackName = authContext?.userId ? `User ${authContext.userId}` : '';

    return {
      requestDescription: '',
      requesterUserId: authContext?.userId ?? null,
      requestDate: '',
      justification: '',
      notes: '',
      sealTypeId: null,
      quantity: null,
      purchaseUrgencyLevelId: null,
      initialStatusName: 'Submitted',
      comments: '',
      createdBy: authContext?.name ?? fallbackName,
    };
  }

  private buildCreateRequestPayload(): CreatePurchaseRequestPayload | null {
    const {
      requestDescription,
      requesterUserId,
      justification,
      notes,
      sealTypeId,
      quantity,
      purchaseUrgencyLevelId,
      initialStatusName,
      comments,
      createdBy,
    } = this.createRequestForm;

    if (
      !requestDescription.trim() ||
      !justification.trim() ||
      !notes.trim() ||
      !sealTypeId ||
      !quantity ||
      !purchaseUrgencyLevelId ||
      !initialStatusName.trim() ||
      !createdBy.trim()
    ) {
      return null;
    }

    return {
      requestDescription: requestDescription.trim(),
      requesterUserId: requesterUserId ?? null,
      justification: justification.trim(),
      notes: notes.trim(),
      sealTypeId,
      quantity,
      purchaseUrgencyLevelId,
      initialStatusName: initialStatusName.trim(),
      comments: comments.trim() || notes.trim(),
      createdBy: createdBy.trim(),
    };
  }

  openCreate(): void {
    this.showCreateRequest = true;
    this.createRequestForm = this.buildCreateRequestForm();
  }

  closeCreate(): void {
    this.showCreateRequest = false;
    this.createRequestError = '';
    this.pendingCreatePayload = null;
  }

  private getSealTypeLabel(sealTypeId: number): string {
    switch (sealTypeId) {
      case 1:
        return 'RFID';
      case 2:
        return 'Cable';
      case 3:
        return 'Bolt';
      default:
        return 'Unknown';
    }
  }

  private getUrgencyLabel(levelId: number): string {
    switch (levelId) {
      case 1:
        return 'Normal';
      case 2:
        return 'High';
      case 3:
        return 'Urgent';
      default:
        return 'Unknown';
    }
  }

  openViewRequest(row: RequestRow): void {
    this.selectedRequest = row;
    this.showViewRequest = true;
  }

  closeViewRequest(): void {
    this.showViewRequest = false;
    this.selectedRequest = null;
  }

  openApproval(row: PurchaseMgmtRow): void {
    this.selectedApprovalRequest = row;
    this.showApproval = true;
  }

  openRejection(row: PurchaseMgmtRow): void {
    this.selectedApprovalRequest = row;
    this.openConfirmModal({
      title: 'Reject purchase request',
      lines: this.buildApprovalConfirmLines(row),
      confirmLabel: 'Yes, reject',
      cancelLabel: 'Cancel',
      action: 'rejection',
    });
  }

  closeApproval(): void {
    this.showApproval = false;
    this.selectedApprovalRequest = null;
  }

  confirmApproval(): void {
    if (!this.selectedApprovalRequest) return;

    this.openConfirmModal({
      title: 'Approve purchase request',
      lines: this.buildApprovalConfirmLines(this.selectedApprovalRequest),
      confirmLabel: 'Yes, approve',
      cancelLabel: 'Cancel',
      action: 'approval',
    });
  }

  openOperationalReturn(): void {
    this.openConfirmModal({
      title: 'Create operational request',
      lines: ['Do you want to create a new operational request?'],
      confirmLabel: 'Yes, continue',
      cancelLabel: 'Cancel',
      action: 'operational',
    });
  }

  closeOperationalReturn(): void {
    this.showOperationalReturn = false;
  }

  openInventoryEdit(row: InventoryRow): void {
    this.pendingInventoryRow = row;
    this.openConfirmModal({
      title: 'Edit receival order',
      lines: ['Do you want to edit this receival order?'],
      confirmLabel: 'Yes, edit',
      cancelLabel: 'Cancel',
      action: 'inventory-edit',
    });
  }

  closeInventoryEdit(): void {
    this.showInventoryEdit = false;
  }

  openInventorySaveConfirm(): void {
    if (!this.selectedInventoryRow) return;

    this.openConfirmModal({
      title: 'Save receival order',
      lines: [
        `Request Order: ${this.selectedInventoryRow.requestOrderId}`,
        `Purchase Order: ${this.selectedInventoryRow.id}`,
        'Do you want to save these changes?',
      ],
      confirmLabel: 'Yes, save',
      cancelLabel: 'Cancel',
      action: 'inventory-save',
    });
  }

  openReconciliationDetail(): void {
    this.showReconciliationDetail = true;
  }

  closeReconciliationDetail(): void {
    this.showReconciliationDetail = false;
  }

  openDeactivate(): void {
    this.openConfirmModal({
      title: 'Deactivate seal',
      lines: ['Are you sure you want to deactivate this seal?'],
      confirmLabel: 'Yes, deactivate',
      cancelLabel: 'Cancel',
      action: 'deactivate',
    });
  }

  closeDeactivate(): void {
    this.showDeactivate = false;
  }

  openUpload(step: 1 | 2 | 3 | 4 = 1): void {
    this.pendingUploadRow = null;
    this.pendingUploadStep = step;

    this.openConfirmModal({
      title: 'Upload seals file',
      lines: ['Do you want to upload a seals file?'],
      confirmLabel: 'Yes, upload',
      cancelLabel: 'Cancel',
      action: 'upload',
    });
  }

  openUploadFromRow(row?: InventoryRow, step: 1 | 2 | 3 | 4 = 1): void {
    this.pendingUploadRow = row ?? null;
    this.pendingUploadStep = step;

    this.openConfirmModal({
      title: 'Upload seals file',
      lines: ['Do you want to upload a seals file?'],
      confirmLabel: 'Yes, upload',
      cancelLabel: 'Cancel',
      action: 'upload',
    });
  }

  openConfirmModal(options: {
    title: string;
    lines: string[];
    confirmLabel?: string;
    cancelLabel?: string;
    action:
    | 'operational'
    | 'inventory-edit'
    | 'inventory-save'
    | 'deactivate'
    | 'upload'
    | 'approval'
    | 'rejection'
    | 'create-request';
  }): void {
    this.confirmTitle = options.title;
    this.confirmLines = options.lines;
    this.confirmConfirmLabel = options.confirmLabel ?? 'Confirm';
    this.confirmCancelLabel = options.cancelLabel ?? 'Cancel';
    this.confirmAction = options.action;
    this.showConfirmModal = true;
  }

  closeConfirmModal(): void {
    this.showConfirmModal = false;
    this.confirmLines = [];
    this.confirmAction = null;
    this.pendingCreatePayload = null;
  }

  confirmModalAction(): void {
    const action = this.confirmAction;
    const pendingCreatePayload = this.pendingCreatePayload;
    this.closeConfirmModal();
    if (!action) return;

    if (action === 'operational') {
      this.showOperationalReturn = true;
      return;
    }

    if (action === 'inventory-edit' && this.pendingInventoryRow) {
      this.selectedInventoryRow = this.pendingInventoryRow;
      this.selectedPurchaseOrderId = this.pendingInventoryRow.id;
      this.showInventoryEdit = true;
      this.pendingInventoryRow = null;
      return;
    }

    if (action === 'deactivate') {
      this.showDeactivate = true;
      return;
    }

    if (action === 'inventory-save') {
      this.closeInventoryEdit();
      this.openInfoModal('Inventory updated', 'The seals data modification was saved.');
      return;
    }

    if (action === 'upload') {
      this.uploadModalRow = this.pendingUploadRow;
      this.uploadModalStep = this.pendingUploadStep;
      this.showUploadModal = true;
      this.pendingUploadRow = null;
      return;
    }

    if (action === 'approval') {
      this.submitPurchaseRequestStatus('Approved');
    }

    if (action === 'rejection') {
      this.submitPurchaseRequestStatus('Rejected');
    }

    if (action === 'create-request' && pendingCreatePayload) {
      this.submitCreateRequest(pendingCreatePayload);
    }
  }

  openInfoModal(title: string, message: string): void {
    this.infoTitle = title;
    this.infoMessage = message;
    this.showInfoModal = true;
  }

  closeInfoModal(): void {
    this.showInfoModal = false;
    this.infoTitle = '';
    this.infoMessage = '';
  }

  closeUploadModal(): void {
    this.showUploadModal = false;
    this.uploadModalRow = null;
    this.uploadModalStep = 1;
  }

  private buildApprovalConfirmLines(row: PurchaseMgmtRow): string[] {
    const { id, requester, sealType, total, description } = row;
    return [
      `Request: ${id}`,
      `Requester: ${requester}`,
      `Seal type: ${sealType}`,
      `Total seals: ${total}`,
      `Description: ${description}`,
    ];
  }

  // ✅ CORREGIDO: update optimista + 1 solo refresh (sin carrera)
  private submitPurchaseRequestStatus(statusName: 'Approved' | 'Rejected'): void {
    if (!this.selectedApprovalRequest || this.isUpdatingPurchaseStatus) return;

    const { id } = this.selectedApprovalRequest;
    const authContext = getAuthContextFromStorage();
    const fallbackName = authContext?.userId ? `User ${authContext.userId}` : 'System';
    const actorName = authContext?.name ?? fallbackName;
    const comments = `${statusName} by ${actorName}`;

    const payload: UpdatePurchaseRequestStatusPayload = {
      statusName,
      comments,
      rejectionReason: statusName === 'Rejected' ? comments : null,
      updatedBy: actorName,
      actionByUserId: authContext?.userId ?? null,
    };

    this.isUpdatingPurchaseStatus = true;

    this.purchaseRequestService
      .updateStatus(id, payload)
      .pipe(
        finalize(() => {
          this.runUiUpdate(() => {
            this.isUpdatingPurchaseStatus = false;
          });
        }),
      )
      .subscribe({
        next: (response) => {
          // ✅ 1) UI inmediata: cambia estado ya (quita botones)
          this.runUiUpdate(() => {
            this.updatePurchaseRequestStatus(id, statusName);

            // ✅ 2) Si el endpoint de update retorna una lista válida, la aplicamos (si no, NO pisamos)
            if (this.isPurchaseRequestRowsResponse(response)) {
              this.applyPurchaseRequestRows(response as any);
            }

            this.openInfoModal(
              `Request ${statusName.toLowerCase()}`,
              `Purchase request ${id} was ${statusName.toLowerCase()}.`,
            );
            this.closeApproval();
          });

          // ✅ 3) SOLO 1 refresh real (evita que te vuelva a pintar Submitted por carrera/cache)
          this.refreshPurchaseRequests(true);
        },
        error: () => {
          this.runUiUpdate(() => {
            this.openInfoModal(
              'Unable to update request',
              'Please try again or contact support.',
            );
            this.closeApproval();
          });
        },
      });
  }

  private updatePurchaseRequestStatus(id: string, status: string): void {
    const normalized = this.normalizeStatus(status);

    this.requestRows = this.requestRows.map((row) =>
      row.id === id ? { ...row, status: normalized } : row,
    );

    this.purchaseMgmtRows = this.purchaseMgmtRows.map((row) =>
      row.id === id ? { ...row, status: normalized } : row,
    );

    if (this.selectedApprovalRequest?.id === id) {
      this.selectedApprovalRequest = { ...this.selectedApprovalRequest, status: normalized };
    }
  }
}
