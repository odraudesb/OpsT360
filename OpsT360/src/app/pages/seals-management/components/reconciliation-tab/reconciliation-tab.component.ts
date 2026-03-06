import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { ReconciliationRow } from '../../seals-management.types';
import { clampPage, getResponsivePageSize } from '../../../../core/utils/pagination';

@Component({
  selector: 't360-reconciliation-tab',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reconciliation-tab.component.html',
})
export class ReconciliationTabComponent {
  readonly defaultRows: ReconciliationRow[] = [
    {
      id: 'CONC-2025-001',
      date: '1.JUN.2025 10:45',
      vessel: 'MSC LORENZA - UX541A',
      operationType: 'Exportation',
      systemSeals: 900,
      physicalSeals: 850,
      difference: -50,
      status: 'Closed',
      user: 'JRamirez',
    },
    {
      id: 'CONC-2025-002',
      date: '12.MAY.2025 14:59',
      vessel: 'MSC BELMONTE III',
      operationType: 'Mixed',
      systemSeals: 1250,
      physicalSeals: 1100,
      difference: -150,
      status: 'In progress',
      user: 'PPorras',
    },
    {
      id: 'CONC-2025-003',
      date: '8.JUN.2025 8:21',
      vessel: 'CMA CGM IMAGINATION...',
      operationType: 'Exportation',
      systemSeals: 700,
      physicalSeals: 700,
      difference: 0,
      status: 'Close',
      user: 'ASarmiento',
    },
    {
      id: 'CONC-2025-004',
      date: '14.AGO.2025 13:41',
      vessel: 'CMA CGM IMAGINATI...',
      operationType: 'Exportation',
      systemSeals: 1000,
      physicalSeals: '-',
      difference: '-',
      status: 'Not initiated',
      user: 'CQuiroga',
    },
  ];

  @Input() rows: ReconciliationRow[] = this.defaultRows;
  @Output() viewDetail = new EventEmitter<void>();
  pageSize = getResponsivePageSize();
  currentPage = 1;

  get totalPages(): number {
    return Math.max(Math.ceil(this.rows.length / this.pageSize), 1);
  }

  get paginatedRows(): ReconciliationRow[] {
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

  trackById(_: number, item: { id: string }): string {
    return item.id;
  }
}
