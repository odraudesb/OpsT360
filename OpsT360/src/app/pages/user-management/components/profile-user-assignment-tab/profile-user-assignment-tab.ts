import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

type AssignmentUser = {
  name: string;
  title: string;
  initials: string;
  online: boolean;
};

@Component({
  selector: 't360-profile-user-assignment-tab',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile-user-assignment-tab.html',
})
export class ProfileUserAssignmentTabComponent {
  readonly selectedProfile = 'Security Supervisor';
  searchTerm = '';

  readonly users: AssignmentUser[] = [
    { name: 'María García', title: 'Service Agent', initials: 'MG', online: true },
    { name: 'Carlos Navarrete', title: 'Service Agent', initials: 'CN', online: true },
    { name: 'Pedro Menendez', title: 'Service Agent', initials: 'PM', online: true },
    { name: 'Juan Portero', title: 'Service Agent', initials: 'JP', online: true },
    { name: 'Juan Mina', title: 'Service Agent', initials: 'JM', online: true },
    { name: 'Roberto Villacis', title: 'Service Agent', initials: 'RV', online: true },
    { name: 'Angel Porras', title: 'Service Agent', initials: 'AP', online: true },
  ];

  get filteredUsers(): AssignmentUser[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) return this.users;
    return this.users.filter((u) =>
      [u.name, u.title, u.initials].some((value) => value.toLowerCase().includes(term)),
    );
  }
}
