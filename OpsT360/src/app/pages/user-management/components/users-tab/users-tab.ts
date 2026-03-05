import { CommonModule } from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { clampPage, getResponsivePageSize } from '../../../../core/utils/pagination';

type UserRow = {
  name: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  department: string;
  jobTitle: string;
  profile: string;
  status: 'Active' | 'Pending';
  selected?: boolean;
};

@Component({
  selector: 't360-users-tab',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './users-tab.html',
})
export class UsersTabComponent {
  searchTerm = '';
  showAddUserModal = false;
  pageSize = getResponsivePageSize();
  currentPage = 1;

  readonly users: UserRow[] = [
    {
      name: 'David',
      lastName: 'Murillo',
      email: 'dmz866@infraportus.com',
      phoneNumber: '+5939876463322',
      department: 'Development',
      jobTitle: 'Dev Manager',
      profile: 'Management',
      status: 'Active',
      selected: true,
    },
    {
      name: 'Sara',
      lastName: 'Zumárraga',
      email: 'saram@infraportus.com',
      phoneNumber: '',
      department: 'Management',
      jobTitle: 'Management',
      profile: 'Management',
      status: 'Pending',
    },
  ];

  get filteredUsers(): UserRow[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) return this.users;
    return this.users.filter((u) =>
      [
        u.name,
        u.lastName,
        u.email,
        u.phoneNumber,
        u.department,
        u.jobTitle,
        u.profile,
        u.status,
      ]
        .filter(Boolean)
        .some((value) => value.toLowerCase().includes(term)),
    );
  }

  get totalPages(): number {
    return Math.max(Math.ceil(this.filteredUsers.length / this.pageSize), 1);
  }

  get paginatedUsers(): UserRow[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredUsers.slice(start, start + this.pageSize);
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

  openAddUser(): void {
    this.showAddUserModal = true;
  }

  closeAddUser(): void {
    this.showAddUserModal = false;
  }

  saveUser(): void {
    // In a real scenario, call the service to persist the new/updated user.
    this.closeAddUser();
  }
}
