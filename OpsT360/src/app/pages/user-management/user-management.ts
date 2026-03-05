import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { T360HeaderComponent } from '../../components/header/t360-header/t360-header';
import { AccessControlSettingsTabComponent } from './components/access-control-settings-tab/access-control-settings-tab';
import { ProfileUserAssignmentTabComponent } from './components/profile-user-assignment-tab/profile-user-assignment-tab';
import { ProfilesTabComponent } from './components/profiles-tab/profiles-tab';
import { UsersTabComponent } from './components/users-tab/users-tab';

type UserManagementTab =
  | 'users'
  | 'profiles'
  | 'profile-user-assignment'
  | 'access-control-settings';

@Component({
  selector: 't360-user-management-page',
  standalone: true,
  imports: [
    CommonModule,
    T360HeaderComponent,
    UsersTabComponent,
    ProfilesTabComponent,
    ProfileUserAssignmentTabComponent,
    AccessControlSettingsTabComponent,
  ],
  templateUrl: './user-management.html',
})
export class UserManagementComponent {
  // Header (evitar null)
  readonly sectionLabel = 'Users Management';
  readonly sectionLink = '';

  selectedTab: UserManagementTab = 'users';

  readonly tabs: { id: UserManagementTab; label: string }[] = [
    { id: 'users', label: 'Users' },
    { id: 'profiles', label: 'Profiles' },
    { id: 'profile-user-assignment', label: 'Profile User Assignment' },
    { id: 'access-control-settings', label: 'Access Control Settings' },
  ];

  trackByTabId(_: number, tab: { id: UserManagementTab }): UserManagementTab {
    return tab.id;
  }

  setTab(tab: UserManagementTab): void {
    this.selectedTab = tab;
  }
}
