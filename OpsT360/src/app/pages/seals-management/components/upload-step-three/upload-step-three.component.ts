import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { UploadField } from '../../seals-management.types';
import { clampPage, getResponsivePageSize } from '../../../../core/utils/pagination';

@Component({
  selector: 't360-upload-step-three',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './upload-step-three.component.html',
})
export class UploadStepThreeComponent {
  @Input() mappedFields: UploadField[] = [];
  @Input() previewRows: Record<string, string>[] = [];
  @Input() uploadFileName = '';
  @Input() rowsDetected = 0;
  @Input() fieldsMappedCount = 0;
  @Input() importPayload = '';
  @Input() importOptions = {
    skipDuplicates: true,
    overwriteAll: false,
    reportOnlyNonDuplicates: true,
  };
  @Output() importOptionsChange = new EventEmitter<{
    skipDuplicates: boolean;
    overwriteAll: boolean;
    reportOnlyNonDuplicates: boolean;
  }>();
  @Output() back = new EventEmitter<void>();
  @Output() importData = new EventEmitter<void>();
  pageSize = getResponsivePageSize();
  currentPage = 1;

  get totalPages(): number {
    return Math.max(Math.ceil(this.previewRows.length / this.pageSize), 1);
  }

  get paginatedPreviewRows(): Record<string, string>[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.previewRows.slice(start, start + this.pageSize);
  }

  @HostListener('window:resize')
  onResize(): void {
    const nextSize = getResponsivePageSize();
    if (nextSize !== this.pageSize) {
      this.pageSize = nextSize;
    }
    this.currentPage = clampPage(this.currentPage, this.totalPages);
  }

  nextPage(): void {
    this.currentPage = clampPage(this.currentPage + 1, this.totalPages);
  }

  previousPage(): void {
    this.currentPage = clampPage(this.currentPage - 1, this.totalPages);
  }

  updateOption<K extends keyof typeof this.importOptions>(key: K, value: boolean): void {
    this.importOptionsChange.emit({ ...this.importOptions, [key]: value });
  }
}
