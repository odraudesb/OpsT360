import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PurchaseMgmtRow, PurchaseRequestFilter } from '../../seals-management.types';
import { clampPage, getResponsivePageSize } from '../../../../core/utils/pagination';

@Component({
  selector: 't360-seal-purchase-tab',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './seal-purchase-tab.component.html',
})
export class SealPurchaseTabComponent {
  @Input({ required: true }) rows: PurchaseMgmtRow[] = [];
  @Input() searchTerm = '';
  @Input() filterKey: PurchaseRequestFilter = 'all';
  @Output() searchTermChange = new EventEmitter<string>();
  @Output() filterKeyChange = new EventEmitter<PurchaseRequestFilter>();
  @Output() approve = new EventEmitter<PurchaseMgmtRow>();
  @Output() reject = new EventEmitter<PurchaseMgmtRow>();

  public showFilterMenu = false;
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

  get filteredRows(): PurchaseMgmtRow[] {
    return this.filterRows(this.rows);
  }

  get totalPages(): number {
    return Math.max(Math.ceil(this.filteredRows.length / this.pageSize), 1);
  }

  get paginatedRows(): PurchaseMgmtRow[] {
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

  private filterRows(rows: PurchaseMgmtRow[]): PurchaseMgmtRow[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      return rows;
    }

    const fields: Array<keyof PurchaseMgmtRow> =
      this.filterKey === 'id'
        ? ['id']
        : this.filterKey === 'requester'
          ? ['requester']
          : this.filterKey === 'status'
            ? ['status']
            : ['id', 'description', 'requester', 'date', 'sealType', 'total', 'status'];

    return rows.filter((row) =>
      fields.some((field) => (row[field] ?? '').toString().toLowerCase().includes(term)),
    );
  }

  isFinalStatus(status: string): boolean {
    const normalized = status.trim().toLowerCase();
    return normalized === 'approved' || normalized === 'rejected';
  }
}
