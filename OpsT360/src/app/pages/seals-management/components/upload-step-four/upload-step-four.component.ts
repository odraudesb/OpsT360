import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { UploadError, UploadSummary } from '../../seals-management.types';
import { clampPage, getResponsivePageSize } from '../../../../core/utils/pagination';

@Component({
  selector: 't360-upload-step-four',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './upload-step-four.component.html',
})
export class UploadStepFourComponent {
  @Input() importSummary: UploadSummary = { total: 0, success: 0, errors: 0 };
  @Input() importErrors: UploadError[] = [];
  @Input() importProgress = 0;
  @Input() reportRowsCount = 0;
  @Output() downloadReport = new EventEmitter<void>();
  pageSize = getResponsivePageSize();
  currentPage = 1;

  get totalPages(): number {
    return Math.max(Math.ceil(this.importErrors.length / this.pageSize), 1);
  }

  get paginatedErrors(): UploadError[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.importErrors.slice(start, start + this.pageSize);
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
}
