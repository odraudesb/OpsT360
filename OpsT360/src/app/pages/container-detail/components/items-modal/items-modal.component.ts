import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { clampPage, getResponsivePageSize } from '../../../../core/utils/pagination';

@Component({
  selector: 't360-items-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './items-modal.component.html',
})
export class ItemsModalComponent {
  @Input() isOpen = false;
  @Input() title = '';
  @Input() columns: string[] = [];
  @Input() rows: Record<string, unknown>[] = [];
  pageSize = getResponsivePageSize();
  currentPage = 1;

  @Output() close = new EventEmitter<void>();

  get totalPages(): number {
    return Math.max(Math.ceil(this.rows.length / this.pageSize), 1);
  }

  get paginatedRows(): Record<string, unknown>[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.rows.slice(start, start + this.pageSize);
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

  onClose(): void {
    this.close.emit();
  }

  formatCell(value: unknown): string {
    if (value == null) return '—';
    if (typeof value === 'string' || typeof value === 'number' || typeof value === 'boolean') return String(value);
    try {
      return JSON.stringify(value);
    } catch {
      return String(value);
    }
  }
}
