import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 't360-access-control-settings-tab',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './access-control-settings-tab.html',
})
export class AccessControlSettingsTabComponent {
  readonly profile = 'Operations Supervisor';
  readonly application = 'T360° App / T360° WEB';
  readonly system = 'Containerized Cargo Exportation';
}
