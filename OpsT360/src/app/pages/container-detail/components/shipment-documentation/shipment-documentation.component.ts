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
import { catchError, map, of } from 'rxjs';

import type { FileItemDto } from '../../../../interfaces/files/files.interface';
import type { TransactionDetailsWithFileItemDto } from '../../../../interfaces/transactions/transaction-details.interface';
import { FilesService } from '../../../../services/files/files.service';
import { ShipmentDocumentsService } from '../../../../services/shipment-documents/shipment-documents.service';
import type { ShipmentDocRow } from '../../shipment-doc-row.type';
import type { Field } from '../../field.type';
import { buildXmlDetailsFields } from '../../xml-details-fields';

@Component({
  selector: 't360-shipment-documentation-section',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './shipment-documentation.component.html',
})
export class ShipmentDocumentationComponent implements OnChanges, OnDestroy {
  @Input() entityId: number | null = null;
  @Output() contentChanged = new EventEmitter<void>();

  alertMessage = '';
  private alertTimer: number | null = null;

  documentsLoading = false;
  documents: ShipmentDocRow[] = [];
  private docsLoadedForEntityId: number | null = null;
  private docsInFlight = false;

  isModalOpen = false;
  modalTitle = '';
  modalFiles: FileItemDto[] = [];
  modalImages: FileItemDto[] = [];
  modalTransactionId: number | null = null;
  activeImageIndex = 0;
  zoomedImage: FileItemDto | null = null;

  constructor(
    private readonly shipmentDocsService: ShipmentDocumentsService,
    private readonly filesService: FilesService,
  ) { }

  ngOnDestroy(): void {
    this.clearAlertTimer();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ('entityId' in changes) {
      if (changes['entityId']?.previousValue !== changes['entityId']?.currentValue) {
        this.docsLoadedForEntityId = null;
        this.documents = [];
      }
      this.fetchDocumentsForEntityIfPossible();
    }
  }

  get hasDocs(): boolean {
    return this.documents.length > 0;
  }

  clearAlert(): void {
    this.alertMessage = '';
    this.clearAlertTimer();
    this.contentChanged.emit();
  }

  private setAlert(msg: string, autoCloseMs = 2500): void {
    this.alertMessage = msg;

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

  onOpenDocument(doc: ShipmentDocRow): void {
    this.clearAlert();
    const cachedFiles = Array.isArray(doc.files) ? doc.files : [];
    const hasCachedBase64 = cachedFiles.some((file) => !!file?.base64);

    if (cachedFiles.length && hasCachedBase64) {
      this.openModal(doc.transactionId, doc.description, cachedFiles);
      return;
    }

    this.filesService.getFiles(doc.transactionId).subscribe({
      next: (files: FileItemDto[]) => {
        if (!files.length) {
          this.setAlert('No files found for this document.');
          return;
        }

        this.openModal(doc.transactionId, doc.description, files);
      },
      error: (err: unknown) => {
        console.error('[ShipmentDocumentation] getFiles error', { doc, err });
        this.setAlert('Error loading file for this document.');
      },
    });
  }

  onToggleDocument(doc: ShipmentDocRow): void {
    doc.isOpen = !doc.isOpen;
    this.contentChanged.emit();
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
      this.setAlert('This file has no content available to download.');
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
    return (
      file.fileName ||
      file.description ||
      file.type ||
      `Document ${index + 1}`
    );
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

  private fetchDocumentsForEntityIfPossible(): void {
    const id = this.entityId;
    if (!id) return;
    if (this.docsLoadedForEntityId === id || this.docsInFlight) return;

    this.docsInFlight = true;
    this.documentsLoading = true;

    this.shipmentDocsService
      .getTransactionDetailsWithFile({ entityId: id })
      .pipe(
        map((rows: TransactionDetailsWithFileItemDto[]) =>
          rows
            .filter((x) => typeof x.description === 'string' && x.description.trim().length > 0)
            .map((x) => {
              const fields = buildXmlDetailsFields(x.xmlDetails);
              return {
                transactionId: x.transactionId,
                description: x.description.trim(),
                files: Array.isArray(x.files) ? x.files : [],
                xmlDetails: x.xmlDetails,
                fieldsCols: this.splitFieldsInto2Columns(fields),
                isOpen: false,
              };
            }),
        ),
        catchError((err: unknown) => {
          console.error('[ShipmentDocumentation] details-with-file error', { entityId: id, err });
          return of([] as ShipmentDocRow[]);
        }),
      )
      .subscribe((docs: ShipmentDocRow[]) => {
        this.docsLoadedForEntityId = id;
        this.docsInFlight = false;
        this.documentsLoading = false;

        this.documents = docs;
        this.contentChanged.emit();
      });
  }

  private splitFieldsInto2Columns(fields: Field[]): [Field[], Field[]] {
    const left: Field[] = [];
    const right: Field[] = [];
    fields.forEach((f, i) => (i % 2 === 0 ? left : right).push(f));
    return [left, right];
  }
}
