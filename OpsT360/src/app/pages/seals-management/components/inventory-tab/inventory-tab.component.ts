import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { InventoryFilter, InventoryRow } from '../../seals-management.types';
import { clampPage, getResponsivePageSize } from '../../../../core/utils/pagination';

@Component({
  selector: 't360-inventory-tab',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './inventory-tab.component.html',
})
export class InventoryTabComponent {
  @Input({ required: true }) rows: InventoryRow[] = [];
  @Output() editRequest = new EventEmitter<InventoryRow>();
  @Output() uploadRequest = new EventEmitter<InventoryRow>();

  searchTerm = '';
  filterKey: InventoryFilter = 'all';
  showFilterMenu = false;
  showRowMenu = false;
  selectedRowId: string | null = null;
  pageSize = getResponsivePageSize();
  currentPage = 1;

  get filterLabel(): string {
    return this.filterKey === 'id'
      ? 'By Purchase Order'
      : this.filterKey === 'requestOrderId'
        ? 'By Request Order'
      : this.filterKey === 'description'
        ? 'By Description'
        : this.filterKey === 'requester'
          ? 'By Requester'
          : this.filterKey === 'uploadDate'
            ? 'By Upload Date'
            : this.filterKey === 'sealType'
              ? 'By Seal Type'
              : this.filterKey === 'totalSeals'
                ? 'By Total Seals'
                : this.filterKey === 'receivedBy'
                  ? 'By Received By'
                  : 'Search by';
  }

  get filteredRows(): InventoryRow[] {
    return this.filterRows(this.rows);
  }

  get totalPages(): number {
    return Math.max(Math.ceil(this.filteredRows.length / this.pageSize), 1);
  }

  get paginatedRows(): InventoryRow[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredRows.slice(start, start + this.pageSize);
  }

  trackById(_: number, item: { id: string }): string {
    return item.id;
  }

  @HostListener('window:resize')
  onResize(): void {
    const nextSize = getResponsivePageSize();
    if (nextSize !== this.pageSize) {
      this.pageSize = nextSize;
    }
    this.currentPage = clampPage(this.currentPage, this.totalPages);
  }

  toggleFilterMenu(event?: MouseEvent): void {
    event?.stopPropagation();
    this.showFilterMenu = !this.showFilterMenu;
  }

  setFilterKey(filterKey: InventoryFilter): void {
    this.filterKey = filterKey;
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

  setFilterFromHeader(filterKey: InventoryFilter, event?: MouseEvent): void {
    event?.stopPropagation();
    this.setFilterKey(filterKey);
  }

  toggleRowMenu(event: MouseEvent, row: InventoryRow): void {
    event.stopPropagation();
    this.showFilterMenu = false;
    if (this.selectedRowId === row.id) {
      this.showRowMenu = !this.showRowMenu;
      return;
    }
    this.selectedRowId = row.id;
    this.showRowMenu = true;
  }

  closeRowMenu(event?: MouseEvent): void {
    event?.stopPropagation();
    this.showRowMenu = false;
    this.selectedRowId = null;
  }

  resetSearch(event?: MouseEvent): void {
    event?.stopPropagation();
    this.searchTerm = '';
    this.filterKey = 'all';
    this.showFilterMenu = false;
    this.currentPage = 1;
  }

  onSearchTermChange(): void {
    this.currentPage = 1;
  }

  nextPage(): void {
    this.currentPage = clampPage(this.currentPage + 1, this.totalPages);
  }

  previousPage(): void {
    this.currentPage = clampPage(this.currentPage - 1, this.totalPages);
  }

  private filterRows(rows: InventoryRow[]): InventoryRow[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      return rows;
    }

    const fields: Array<keyof InventoryRow> =
      this.filterKey === 'id'
        ? ['id']
        : this.filterKey === 'requestOrderId'
          ? ['requestOrderId']
        : this.filterKey === 'description'
          ? ['description']
          : this.filterKey === 'requester'
            ? ['requester']
            : this.filterKey === 'uploadDate'
              ? ['uploadDate']
              : this.filterKey === 'sealType'
                ? ['sealType']
                : this.filterKey === 'totalSeals'
                  ? ['totalSeals']
                  : this.filterKey === 'receivedBy'
                  ? ['receivedBy']
                  : [
                      'requestOrderId',
                      'id',
                      'description',
                      'requester',
                      'uploadDate',
                      'sealType',
                      'totalSeals',
                      'receivedBy',
                    ];

    return rows.filter((row) =>
      fields.some((field) => (row[field] ?? '').toString().toLowerCase().includes(term)),
    );
  }
}
