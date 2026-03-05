import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { clampPage, getResponsivePageSize } from '../../../../core/utils/pagination';

type ActionRow = {
  containerId: string;
  sealNumber: string;
  status: string;
  user: string;
  sealType: string;
  deactivateReason: string;
  deactivateDate: string;
};

@Component({
  selector: 't360-actions-tab',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './actions-tab.component.html',
})
export class ActionsTabComponent {
  readonly defaultRows: ActionRow[] = [
    {
      containerId: 'MSCU3422129',
      sealNumber: 'S-9986621',
      status: 'Active',
      user: 'jperez (Seguridad)',
      sealType: 'Cable Seal',
      deactivateReason: '-',
      deactivateDate: '-',
    },
    {
      containerId: 'MSCU3422129',
      sealNumber: 'R-67734',
      status: 'inactive',
      user: 'mgarcía(Seguridad)',
      sealType: 'RFID Label Seal',
      deactivateReason: 'Dañado',
      deactivateDate: '23.APR.2025 17:31',
    },
    {
      containerId: '-',
      sealNumber: 'S-6465823',
      status: 'Ready',
      user: 'lrodriguez (Seguridad)',
      sealType: 'RFID Cable Seal',
      deactivateReason: '-',
      deactivateDate: '-',
    },
    {
      containerId: 'CMAU6477222',
      sealNumber: 'X-43665',
      status: 'Active',
      user: 'dmur Seguridad)',
      sealType: 'Bolt Seal',
      deactivateReason: '-',
      deactivateDate: '-',
    },
  ];

  @Input() rows: ActionRow[] = this.defaultRows;
  @Output() deactivate = new EventEmitter<void>();
  searchTerm = '';
  pageSize = getResponsivePageSize();
  currentPage = 1;

  get totalPages(): number {
    return Math.max(Math.ceil(this.filteredRows.length / this.pageSize), 1);
  }

  get filteredRows(): ActionRow[] {
    return this.filterRows(this.rows);
  }

  get paginatedRows(): ActionRow[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredRows.slice(start, start + this.pageSize);
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

  onSearchTermChange(): void {
    this.currentPage = clampPage(1, this.totalPages);
  }

  private filterRows(rows: ActionRow[]): ActionRow[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      return rows;
    }

    const fields: Array<keyof ActionRow> = [
      'containerId',
      'sealNumber',
      'status',
      'user',
      'sealType',
      'deactivateReason',
      'deactivateDate',
    ];

    return rows.filter((row) =>
      fields.some((field) => (row[field] ?? '').toString().toLowerCase().includes(term)),
    );
  }
}
