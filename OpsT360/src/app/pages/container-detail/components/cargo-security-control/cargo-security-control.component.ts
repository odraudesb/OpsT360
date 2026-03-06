import { CommonModule } from '@angular/common';
import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  Output,
  SimpleChanges,
} from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';

import type { FileItemDto } from '@interfaces/files/files.interface';
import type { SecurityTransactionDetailsDto } from '@interfaces/transactions/security-transaction-details.interface';
import { FilesService } from '@services/files/files.service';
import { TransactionsService } from '@services/transactions/transactions.service';
import type { Field } from '../../field.type';
import { buildXmlDetailsFields } from '../../xml-details-fields';

type SecurityDetailRow = SecurityTransactionDetailsDto & {
  isOpen?: boolean;
  fieldsCols: [Field[], Field[]];
};

@Component({
  selector: 't360-cargo-security-control',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './cargo-security-control.component.html',
})
export class CargoSecurityControlComponent implements OnChanges, OnDestroy {
  @Input() entityId: number | null = null;
  @Output() contentChanged = new EventEmitter<void>();

  isLoading = false;
  hasTriedLoading = false;

  securityDetails: SecurityDetailRow[] = [];

  alertMessage = '';
  private alertTimer: number | null = null;

  isModalOpen = false;
  modalTitle = '';
  modalFiles: FileItemDto[] = [];
  modalImages: FileItemDto[] = [];
  modalTransactionId: number | null = null;
  activeImageIndex = 0;
  zoomedImage: FileItemDto | null = null;

  constructor(
    private readonly transactionsService: TransactionsService,
    private readonly filesService: FilesService,
    private readonly translate: TranslateService,
  ) { }

  ngOnDestroy(): void {
    this.clearAlertTimer();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ('entityId' in changes) {
      if (changes['entityId']?.previousValue !== changes['entityId']?.currentValue) {
        this.hasTriedLoading = false;
        this.securityDetails = [];
        this.clearAlert();
      }
      void this.fetchSecurityDetailsIfPossible();
    }
  }

  get hasDetails(): boolean {
    return this.securityDetails.length > 0;
  }

  onSelectDetail(detail: SecurityTransactionDetailsDto): void {
    void this.openDocument(detail);
  }

  onToggleDetail(detail: SecurityDetailRow): void {
    detail.isOpen = !detail.isOpen;
    this.contentChanged.emit();
  }

  clearAlert(): void {
    this.alertMessage = '';
    this.clearAlertTimer();
    this.contentChanged.emit();
  }

  private setAlert(message: string, autoCloseMs = 2500): void {
    this.alertMessage = message;
    this.clearAlertTimer();
    this.alertTimer = window.setTimeout(() => {
      this.alertMessage = '';
      this.alertTimer = null;
      this.contentChanged.emit();
    }, autoCloseMs);
    this.contentChanged.emit();
  }

  private clearAlertTimer(): void {
    if (this.alertTimer != null) {
      window.clearTimeout(this.alertTimer);
      this.alertTimer = null;
    }
  }

  private splitFieldsInto2Columns(fields: Field[]): [Field[], Field[]] {
    const left: Field[] = [];
    const right: Field[] = [];
    fields.forEach((f, i) => (i % 2 === 0 ? left : right).push(f));
    return [left, right];
  }

  private async openDocument(detail: SecurityTransactionDetailsDto): Promise<void> {
    this.clearAlert();

    try {
      const files = await firstValueFrom(this.filesService.getFiles(detail.transactionId));
      if (!files.length) {
        this.setAlert(this.translate.instant('CARGO_SECURITY.NO_FILES'));
        return;
      }

      this.openModal(detail.transactionId, detail.eventName ?? 'Document', files);
    } catch (err: unknown) {
      console.error('[CargoSecurity] getFiles error', { detail, err });
      this.setAlert(this.translate.instant('CARGO_SECURITY.LOAD_ERROR'));
    }
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.modalTitle = '';
    this.modalFiles = [];
    this.modalImages = [];
    this.modalTransactionId = null;
    this.activeImageIndex = 0;
    this.zoomedImage = null;
    this.contentChanged.emit();
  }

  onDownloadFile(file: FileItemDto): void {
    if (!file.base64) {
      this.setAlert(this.translate.instant('CARGO_SECURITY.NO_CONTENT'));
      return;
    }
    this.downloadFile(file);
  }

  openImageZoom(file: FileItemDto): void {
    this.zoomedImage = file;
  }

  closeImageZoom(): void {
    this.zoomedImage = null;
  }

  onNextImage(): void {
    if (!this.modalImages.length) return;
    this.activeImageIndex = (this.activeImageIndex + 1) % this.modalImages.length;
  }

  onPrevImage(): void {
    if (!this.modalImages.length) return;
    this.activeImageIndex =
      (this.activeImageIndex - 1 + this.modalImages.length) % this.modalImages.length;
  }

  selectImage(index: number): void {
    if (index < 0 || index >= this.modalImages.length) return;
    this.activeImageIndex = index;
  }

  visibleImages(): { image: FileItemDto; index: number }[] {
    const total = this.modalImages.length;
    if (!total) return [];
    if (total <= 5) {
      return this.modalImages.map((image, index) => ({ image, index }));
    }
    const start = Math.min(Math.max(this.activeImageIndex - 2, 0), total - 5);
    return this.modalImages
      .slice(start, start + 5)
      .map((image, offset) => ({ image, index: start + offset }));
  }

  imageSrc(file: FileItemDto): string {
    if (!file.base64) return '';
    const mimeType = file.mimeType ?? 'image/jpeg';
    return `data:${mimeType};base64,${file.base64}`;
  }

  fileLabel(file: FileItemDto, index: number): string {
    return file.fileName || file.description || file.type || `Document ${index + 1}`;
  }

  private openModal(transactionId: number, title: string, files: FileItemDto[]): void {
    this.modalTransactionId = transactionId;
    this.modalTitle = title;
    this.modalFiles = files.filter((file) => this.isPdfFile(file));
    this.modalImages = files.filter((file) => this.isImageFile(file) && !!file.base64);
    this.activeImageIndex = 0;
    this.isModalOpen = true;
    this.contentChanged.emit();
  }

  private isPdfFile(file: FileItemDto): boolean {
    const mime = file.mimeType ?? '';
    if (mime === 'application/pdf') return true;
    const name = file.fileName ?? '';
    return /\.pdf$/i.test(name);
  }

  private isImageFile(file: FileItemDto): boolean {
    const mime = file.mimeType ?? '';
    if (mime.startsWith('image/')) return true;
    const name = file.fileName ?? '';
    return /\.(png|jpe?g|gif|webp|bmp|svg)$/i.test(name);
  }

  private downloadFile(file: FileItemDto): void {
    if (!file.base64) return;
    const mimeType = file.mimeType ?? 'application/octet-stream';
    const byteChars = atob(file.base64);
    const byteNumbers = new Array(byteChars.length);
    for (let i = 0; i < byteChars.length; i += 1) {
      byteNumbers[i] = byteChars.charCodeAt(i);
    }
    const blob = new Blob([new Uint8Array(byteNumbers)], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = file.fileName || file.description || file.type || 'document';
    link.click();
    URL.revokeObjectURL(url);
  }

  private async fetchSecurityDetailsIfPossible(): Promise<void> {
    if (this.hasTriedLoading || this.isLoading) return;
    const id = this.entityId;
    if (!id) return;

    this.isLoading = true;

    try {
      const details = await firstValueFrom(
        this.transactionsService.getSecurityTransactionDetails({ entityId: id }),
      );
      this.securityDetails = (details ?? []).map((detail) => {
        const fields = this.getSecurityFields(detail);
        return {
          ...detail,
          fieldsCols: this.splitFieldsInto2Columns(fields),
          isOpen: false,
        };
      });
    } catch (err: unknown) {
      console.error('[CargoSecurity] getSecurityTransactionDetails error', err);
      this.securityDetails = [];
    } finally {
      this.isLoading = false;
      this.hasTriedLoading = true;
      this.contentChanged.emit();
    }
  }

  private getSecurityFields(detail: SecurityTransactionDetailsDto): Field[] {
    const fields = buildXmlDetailsFields(detail.xmlDetails);
    if (this.isPlaceholderField(fields)) {
      return buildXmlDetailsFields(this.stripXmlDetails(detail));
    }
    return fields;
  }

  private stripXmlDetails(detail: SecurityTransactionDetailsDto): Record<string, unknown> {
    const { xmlDetails, ...rest } = detail;
    return rest as Record<string, unknown>;
  }

  private isPlaceholderField(fields: Field[]): boolean {
    return fields.length === 1 && fields[0].label === '—' && fields[0].value === '—';
  }
}
