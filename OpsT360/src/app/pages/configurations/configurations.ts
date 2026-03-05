import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { T360HeaderComponent } from '../../components/header/t360-header/t360-header';
import { APP_ROUTES } from '../../core/constants/app-routes.constants';

type ConfigLink = {
  id: string;
  label: string;
  active?: boolean;
};

type ConfigSection = {
  id: string;
  title: string;
  iconSrc?: string;
  links: ConfigLink[];
};

@Component({
  selector: 't360-configurations-page',
  standalone: true,
  imports: [CommonModule, FormsModule, T360HeaderComponent],
  templateUrl: './configurations.html',
})
export class ConfigurationsComponent {
  searchTerm = '';
  showSupportContactsModal = false;

  supportContactForm = {
    terminalEmail: '',
    terminalCountry: 'United States',
    terminalChannel: 'WSP',
    terminalNumber: '',
    providerCountry: 'Ecuador',
    providerChannel: 'SMS',
    providerNumber: '',
  };

  sections: ConfigSection[] = [
    {
      id: 'companies',
      title: 'Companies',
      iconSrc: 'assets/icons/Companies.png',
      links: [
        { id: 'import', label: 'Import Companies' },
        { id: 'export', label: 'Export Companies' },
        { id: 'regions', label: 'Country Regions' },
        { id: 'industries', label: 'Industries/Sectors' },
        { id: 'phones', label: 'Type of Phones' },
      ],
    },
    {
      id: 'support-contacts',
      title: 'Issues Support Contacts',
      iconSrc: 'assets/icons/issuesSupport.png',
      links: [{ id: 'channels', label: 'Email, Whatsapp & SMS' }],
    },
    {
      id: 'subscriptions',
      title: 'Suscriptions',
      iconSrc: 'assets/icons/suscription.png',
      links: [{ id: 'toggle', label: 'Enable / Disable suscriptions' }],
    },
    {
      id: 'hardware',
      title: 'Hardware enrollment',
      iconSrc: 'assets/icons/hardwareEnrolling.png',
      links: [{ id: 'rfid', label: 'RFID Equipment' }],
    },
    {
      id: 'language',
      title: 'Language by default',
      iconSrc: 'assets/icons/languaje.png',
      links: [
        { id: 'en', label: 'English', active: true },
        { id: 'es', label: 'Español' },
      ],
    },
    {
      id: 'business-units',
      title: 'Business Units',
      iconSrc: 'assets/icons/businessUnits.png',
      links: [{ id: 'events', label: 'Business Units Events' }],
    },
    {
      id: 'files',
      title: 'Files [Documents & Images]',
      iconSrc: 'assets/icons/write.png',
      links: [{ id: 'storage', label: 'Files storage path' }],
    },
  ];

  constructor(private readonly router: Router) {}

  onSectionIconClick(section: ConfigSection): void {
    if (section.id === 'companies') {
      this.router.navigate([`/${APP_ROUTES.COMPANIES}`]);
      return;
    }

    const firstLink = section.links[0];
    if (firstLink) {
      this.onSectionLinkClick(section, firstLink);
    }
  }

  onSectionLinkClick(section: ConfigSection, link: ConfigLink): void {
    if (section.id === 'companies') {
      this.router.navigate([`/${APP_ROUTES.COMPANIES}`]);
      return;
    }

    if (section.id === 'support-contacts' && link.id === 'channels') {
      this.showSupportContactsModal = true;
      return;
    }

    if (section.id === 'hardware' && link.id === 'rfid') {
      this.router.navigate([`/${APP_ROUTES.RFID_EQUIPMENT}`]);
    }
  }

  trackBySectionId(_: number, section: ConfigSection): string {
    return section.id;
  }

  trackByLinkId(_: number, link: ConfigLink): string {
    return link.id;
  }

  closeSupportContactsModal(): void {
    this.showSupportContactsModal = false;
  }

  saveSupportContacts(): void {
    this.closeSupportContactsModal();
  }
}
