import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PurchaseRequestFilter, RequestRow } from '../../seals-management.types';
import { clampPage, getResponsivePageSize } from '../../../../core/utils/pagination';

@Component({
  selector: 't360-purchase-request-tab',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './purchase-request-tab.component.html',
})
export class PurchaseRequestTabComponent {
  @Input({ required: true }) rows: RequestRow[] = [];
  @Input() searchTerm = '';
  @Input() filterKey: PurchaseRequestFilter = 'all';
  @Output() searchTermChange = new EventEmitter<string>();
  @Output() filterKeyChange = new EventEmitter<PurchaseRequestFilter>();
  @Output() viewRequest = new EventEmitter<RequestRow>();

  showFilterMenu = false;
  showActionsCard = false;
  selectedActionRowId: string | null = null;
  pageSize = getResponsivePageSize();
  currentPage = 1;

  get filterLabel(): string {
    return this.filterKey === 'id'
      ? 'By Purchase Request Id'
      : this.filterKey === 'requester'
        ? 'By Requester'
        : this.filterKey === 'status'
          ? 'By Status'
          : 'Search by';
  }

  get filteredRows(): RequestRow[] {
    return this.filterRows(this.rows);
  }

  get totalPages(): number {
    return Math.max(Math.ceil(this.filteredRows.length / this.pageSize), 1);
  }

  get paginatedRows(): RequestRow[] {
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

  trackById(_: number, item: { id: string }): string {
    return item.id;
  }

  toggleFilterMenu(event?: MouseEvent): void {
    event?.stopPropagation();
    this.showFilterMenu = !this.showFilterMenu;
    if (this.showFilterMenu) {
      this.showActionsCard = false;
      this.selectedActionRowId = null;
    }
  }

  setFilterKey(filterKey: PurchaseRequestFilter): void {
    this.filterKey = filterKey;
    this.filterKeyChange.emit(filterKey);
    this.showFilterMenu = false;
    this.currentPage = 1;
  }

  clearFilterKey(event?: MouseEvent): void {
    event?.stopPropagation();
    this.setFilterKey('all');
  }

  closeFilterMenu(event?: MouseEvent): void {
    event?.stopPropagation();
    this.showFilterMenu = false;
  }

  toggleActions(event: MouseEvent, row: RequestRow): void {
    event.stopPropagation();
    this.showFilterMenu = false;
    if (this.selectedActionRowId === row.id) {
      this.showActionsCard = !this.showActionsCard;
      return;
    }
    this.selectedActionRowId = row.id;
    this.showActionsCard = true;
  }

  closeActions(event?: MouseEvent): void {
    event?.stopPropagation();
    this.showActionsCard = false;
    this.selectedActionRowId = null;
  }

  viewRow(event: MouseEvent, row: RequestRow): void {
    event.stopPropagation();
    this.viewRequest.emit(row);
    this.closeActions();
  }

  resetSearch(event?: MouseEvent): void {
    event?.stopPropagation();
    this.searchTerm = '';
    this.searchTermChange.emit('');
    this.setFilterKey('all');
    this.showFilterMenu = false;
    this.currentPage = 1;
  }

  handleSearchChange(value: string): void {
    this.searchTerm = value;
    this.searchTermChange.emit(value);
    this.currentPage = 1;
  }

  nextPage(): void {
    this.currentPage = clampPage(this.currentPage + 1, this.totalPages);
  }

  previousPage(): void {
    this.currentPage = clampPage(this.currentPage - 1, this.totalPages);
  }

  private filterRows(rows: RequestRow[]): RequestRow[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      return rows;
    }

    const fields: Array<keyof RequestRow> =
      this.filterKey === 'id'
        ? ['id']
        : this.filterKey === 'requester'
          ? ['requester']
          : this.filterKey === 'status'
            ? ['status']
            : [
                'id',
                'description',
                'quantity',
                'requester',
                'date',
                'sealType',
                'status',
                'timeSince',
              ];

    return rows.filter((row) =>
      fields.some((field) => (row[field] ?? '').toString().toLowerCase().includes(term)),
    );
  }
}
