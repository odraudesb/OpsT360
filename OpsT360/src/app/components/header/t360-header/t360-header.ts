import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { T360LauncherComponent } from '../../launcher/t360-launcher/t360-launcher';

@Component({
  selector: 't360-header',
  standalone: true,
  imports: [CommonModule, T360LauncherComponent],
  templateUrl: './t360-header.html',
})
export class T360HeaderComponent implements OnInit {
  @Input() sectionLabel = 'Containerized cargo:';
  @Input() sectionLink = 'EXPORTATION category';
  @Input() showNotificationDot = true;
  @Input() showNotifications = true;
  @Input() disableSectionLink = false;
  @Input() startOpen = false;

  isLauncherOpen = false;

  ngOnInit(): void {
    this.isLauncherOpen = this.startOpen;
  }

  toggleLauncher(): void {
    this.isLauncherOpen = !this.isLauncherOpen;
  }

  closeLauncher(): void {
    this.isLauncherOpen = false;
  }
}
