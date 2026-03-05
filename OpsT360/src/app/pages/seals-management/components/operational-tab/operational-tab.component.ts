import { CommonModule } from '@angular/common';
import { Component, HostListener, Input } from '@angular/core';
import { OperationalRow } from '../../seals-management.types';
import { clampPage, getResponsivePageSize } from '../../../../core/utils/pagination';

@Component({
  selector: 't360-operational-tab',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './operational-tab.component.html',
})
export class OperationalTabComponent {
  readonly defaultRows: OperationalRow[] = [
    {
      id: 'OP-2025-001',
      vessel: 'MSC LORENZA - UX541A',
      category: 'Exportation',
      containers: 861,
      sealsRequested: 878,
      sealsDelivered: 878,
      sealsReturned: 0,
      status: 'Delivered',
      requester: 'azambrano',
      delivery: 'jmina',
    },
    {
      id: 'OP-2025-002',
      vessel: 'MSC LORENZA - UX541A',
      category: 'Police Inspections',
      containers: 65,
      sealsRequested: 67,
      sealsDelivered: 67,
      sealsReturned: 65,
      status: 'Completed',
      requester: 'azambrano',
      delivery: 'jmina',
    },
    {
      id: 'OP-2025-002B',
      vessel: 'CMA CGM IMAGINATION- ODVN...',
      category: 'Exportation',
      containers: 1840,
      sealsRequested: 1876,
      sealsDelivered: '-',
      sealsReturned: '-',
      status: 'Pending',
      requester: 'azambrano',
      delivery: 'jmina',
    },
    {
      id: 'OP-2025-003',
      vessel: 'CMA CGM IMAGINATION- ODVN...',
      category: 'Police Inspections',
      containers: 120,
      sealsRequested: 36,
      sealsDelivered: 156,
      sealsReturned: 16,
      status: 'Partial Del.',
      requester: 'azambrano',
      delivery: 'jmina',
    },
  ];

  @Input() rows: OperationalRow[] = this.defaultRows;
  pageSize = getResponsivePageSize();
  currentPage = 1;

  get totalPages(): number {
    return Math.max(Math.ceil(this.rows.length / this.pageSize), 1);
  }

  get paginatedRows(): OperationalRow[] {
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
