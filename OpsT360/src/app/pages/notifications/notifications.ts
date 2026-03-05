import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { T360HeaderComponent } from '../../components/header/t360-header/t360-header';

type NotificationType = 'Alert' | 'Announcement' | 'Draft';

type NotificationCard = {
  id: string;
  title: string;
  date: string;
  time: string;
  description: string;
  type: NotificationType;
};

@Component({
  selector: 't360-notifications-page',
  standalone: true,
  imports: [CommonModule, FormsModule, T360HeaderComponent],
  templateUrl: './notifications.html',
  styleUrl: './notifications.css',
})
export class NotificationsComponent {
  tabs: { id: NotificationType | 'All'; label: string }[] = [
    { id: 'All', label: 'All' },
    { id: 'Alert', label: 'Alerts' },
    { id: 'Announcement', label: 'Announcements' },
    { id: 'Draft', label: 'Drafts' },
  ];

  activeTab: NotificationType | 'All' = 'All';
  searchTerm = '';
  showCreateModal = false;

  notifications: NotificationCard[] = [
    {
      id: 'n1',
      title: 'CONTENEDOR MSKU1234567 CON ALERTA DE BLOQUEO POR FALTA DE PAGO',
      date: '18.06.2025',
      time: '09:15',
      description:
        'Realice el pago antes de las 18H00 para evitar problemas al embarque de la carga. Para realizar su pago online ingrese a nuestro portal de clientes.',
      type: 'Alert',
    },
    {
      id: 'n2',
      title: 'ALERTA DE TEMPERATURA CRÍTICA EN CONTENEDOR REFRIGERADO',
      date: '10.04.2025',
      time: '10:00',
      description:
        'El contenedor MKBJ1234567 presenta temperatura de -1.2°C fuera del rango permitido (-0.5°C a +0.5°C). Se recomienda contactar al proveedor de soporte técnico.',
      type: 'Alert',
    },
    {
      id: 'n3',
      title: 'MANTENIMIENTO PROGRAMADO DEL SISTEMA T360°',
      date: '08.05.2025',
      time: '08:30',
      description:
        'El contenedor MKBJ1234567 presenta temperatura de -1.2°C fuera del rango permitido (-0.5°C a +0.5°C). Se recomienda contactar al proveedor de soporte técnico.',
      type: 'Announcement',
    },
  ];

  createModel = {
    title: '',
    type: 'Alert / Notice',
    message: '',
    recipients: {
      all: false,
      exporters: true,
      importers: true,
      shippingLines: false,
      brokers: true,
      authorities: false,
    },
    schedule: '',
    category: '',
    channels: {
      app: true,
      portal: true,
      email: true,
      sms: false,
    },
    relatedOwner: '',
  };

  get filteredNotifications(): NotificationCard[] {
    const term = this.searchTerm.trim().toLowerCase();

    return this.notifications.filter((item) => {
      const matchesTab = this.activeTab === 'All' ? true : item.type === this.activeTab;
      const matchesSearch =
        !term ||
        item.title.toLowerCase().includes(term) ||
        item.description.toLowerCase().includes(term);
      return matchesTab && matchesSearch;
    });
  }

  setTab(tab: NotificationType | 'All'): void {
    this.activeTab = tab;
  }

  openCreate(): void {
    this.showCreateModal = true;
  }

  closeCreate(): void {
    this.showCreateModal = false;
  }

  trackById(_: number, item: NotificationCard): string {
    return item.id;
  }
}
