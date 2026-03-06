import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { APP_ROUTES } from '../../../core/constants/app-routes.constants';
import { PurchaseRequestService } from '../../../services/seals/purchase-request.service';

type AppShortcut = {
  id: string;
  label: string;
  subtitle?: string;
  iconSrc: string;
  route?: string;
  primary?: boolean;
};

type FooterLink = {
  id: string;
  label: string;
  iconSrc: string;
  route?: string;
};

@Component({
  selector: 't360-launcher',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './t360-launcher.html',
})
export class T360LauncherComponent {
  @Output() close = new EventEmitter<void>();

  searchTerm = '';

  // Ajustado a tus nombres de archivos (tal cual los mostraste en el explorer).
  readonly apps: AppShortcut[] = [
    {
      id: 'container-export',
      label: 'Containerized cargo',
      subtitle: 'EXPORTATION',
      iconSrc: 'assets/icons/containerLeft.png',
      route: APP_ROUTES.CONTAINERS_EXPORTATION,
      primary: true,
    },
    {
      id: 'container-import',
      label: 'Containerized cargo',
      subtitle: 'IMPORTATION',
      iconSrc: 'assets/icons/containerRight.png',
      // route: APP_ROUTES.CONTAINERS_IMPORTATION, // si lo tienes
      primary: true,
    },

    {
      id: 'cfs-export',
      label: 'Container Freight Station',
      subtitle: 'EXPORTATION',
      iconSrc: 'assets/icons/CFS.PNG 1.png',
      // route: APP_ROUTES.CFS_EXPORTATION, // si lo tienes
    },
    {
      id: 'cfs-import',
      label: 'Container Freight Station',
      subtitle: 'IMPORTATION',
      iconSrc: 'assets/icons/CFS.PNG 2.png',
      // route: APP_ROUTES.CFS_IMPORTATION, // si lo tienes
    },

    {
      id: 'breakbulk-import',
      label: 'Break Bulk cargo',
      subtitle: 'IMPORTATION',
      iconSrc: 'assets/icons/BBKEXPO.PNG 2.png',
      // route: APP_ROUTES.BREAKBULK_IMPORTATION, // si lo tienes
    },
    {
      id: 'breakbulk-export',
      label: 'Break Bulk Cargo',
      subtitle: 'EXPORTATION',
      iconSrc: 'assets/icons/BBKEXPO.PNG 1.png',
      // route: APP_ROUTES.BREAKBULK_EXPORTATION, // si lo tienes
    },

    {
      id: 'banana-export',
      label: 'Banano cargo',
      subtitle: 'EXPORTATION',
      iconSrc: 'assets/icons/BANANA.PNG 1.png',
      // route: APP_ROUTES.BANANO_EXPORTATION, // si lo tienes
    },
    {
      id: 'brokers',
      label: 'Brokers',
      iconSrc: 'assets/icons/BROKERS.PNG 1.png',
      // route: APP_ROUTES.BROKERS, // si lo tienes
    },
    {
      id: 'shipping-lines',
      label: 'Shipping Lines',
      iconSrc: 'assets/icons/SHIPPINGLINES.png',
      // route: APP_ROUTES.SHIPPING_LINES, // si lo tienes
    },

    {
      id: 'vessel-traffic',
      label: 'Vessel Traffic',
      iconSrc: 'assets/icons/VESSELTRAFIC 1.png',
      // route: APP_ROUTES.VESSEL_TRAFFIC, // si lo tienes
    },
    {
      id: 'seals',
      label: 'Seals Management',
      iconSrc: 'assets/icons/file-lock.png',
      route: APP_ROUTES.SEALS_MANAGEMENT,
      primary: true,
    },
    {
      id: 'users',
      label: 'Users Management',
      iconSrc: 'assets/icons/users-2.png',
      route: APP_ROUTES.USER_MANAGEMENT,
      primary: true,
    },
  ];

  readonly footerLinks: FooterLink[] = [
    {
      id: 'simulator',
      label: 'Simulator',
      iconSrc: 'assets/icons/sparkles.png',
      route: APP_ROUTES.RFID_SEAL_SIMULATOR,
    },
    {
      id: 'config',
      label: 'Configurations',
      iconSrc: 'assets/icons/cog.png',
      route: APP_ROUTES.CONFIGURATIONS,
    },
    {
      id: 'subs',
      label: 'Subscriptions',
      iconSrc: 'assets/icons/license.png',
      route: APP_ROUTES.SUBSCRIPTIONS,
    },
    {
      id: 'noti',
      label: 'Notifications',
      iconSrc: 'assets/icons/add-notification.png',
      route: APP_ROUTES.NOTIFICATIONS,
    },
  ];

  constructor(
    private readonly router: Router,
    private readonly purchaseRequestService: PurchaseRequestService,
  ) {}

  get filteredApps(): AppShortcut[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) return this.apps;

    return this.apps.filter((app) => {
      const inLabel = app.label.toLowerCase().includes(term);
      const inSubtitle = (app.subtitle ?? '').toLowerCase().includes(term);
      return inLabel || inSubtitle;
    });
  }

  trackByAppId(_: number, item: AppShortcut): string {
    return item.id;
  }

  trackByFooterId(_: number, item: FooterLink): string {
    return item.id;
  }

  onCloseClick(): void {
    this.close.emit();
  }

  async onAppClick(app: AppShortcut): Promise<void> {
    if (app.route) {
      if (app.route === APP_ROUTES.SEALS_MANAGEMENT) {
        this.purchaseRequestService.preload();
      }
      await this.router.navigate(['/', app.route]);
    }
    this.close.emit();
  }

  async onFooterClick(link: FooterLink): Promise<void> {
    if (link.route) {
      await this.router.navigate(['/', link.route]);
    }
    // si quieres que cierre al tocar footer (como modal):
    this.close.emit();
  }
}
