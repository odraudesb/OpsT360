import { CommonModule } from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { clampPage, getResponsivePageSize } from '../../../../core/utils/pagination';

type ProfileRow = {
  name: string;
  description: string;
  status: 'Active' | 'Inactive';
  system: string;
  usersAssigned: number;
  creationDate: string;
};

@Component({
  selector: 't360-profiles-tab',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profiles-tab.html',
})
export class ProfilesTabComponent {
  searchTerm = '';
  showAddProfileModal = false;
  pageSize = getResponsivePageSize();
  currentPage = 1;

  readonly profiles: ProfileRow[] = [
    {
      name: 'Assistant SEC',
      description: 'Admin access to all modules',
      status: 'Active',
      system: 'Seals Management',
      usersAssigned: 1,
      creationDate: 'Jul 14, 2025',
    },
    {
      name: 'Supervisor SEC',
      description: 'Profile with access to core modules',
      status: 'Inactive',
      system: 'Exportation',
      usersAssigned: 21,
      creationDate: 'Aug 21, 2025',
    },
  ];

  get filteredProfiles(): ProfileRow[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) return this.profiles;
    return this.profiles.filter((p) =>
      [
        p.name,
        p.description,
        p.status,
        p.system,
        p.usersAssigned.toString(),
        p.creationDate,
      ]
        .filter(Boolean)
        .some((value) => value.toLowerCase().includes(term)),
    );
  }

  get totalPages(): number {
    return Math.max(Math.ceil(this.filteredProfiles.length / this.pageSize), 1);
  }

  get paginatedProfiles(): ProfileRow[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredProfiles.slice(start, start + this.pageSize);
  }

  @HostListener('window:resize')
  onResize(): void {
    const nextSize = getResponsivePageSize();
    if (nextSize !== this.pageSize) {
      this.pageSize = nextSize;
    }
    this.currentPage = clampPage(this.currentPage, this.totalPages);
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

  openAddProfile(): void {
    this.showAddProfileModal = true;
  }

  closeAddProfile(): void {
    this.showAddProfileModal = false;
  }

  saveProfile(): void {
    // Placeholder for persistence; close after action.
    this.closeAddProfile();
  }
}
