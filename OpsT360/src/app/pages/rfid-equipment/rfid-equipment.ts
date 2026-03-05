import { CommonModule } from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

import { T360HeaderComponent } from '../../components/header/t360-header/t360-header';
import { APP_ROUTES } from '../../core/constants/app-routes.constants';

type EquipmentType = 'UHF RFID Antenna' | 'Hand Held';

type RfidEquipment = {
  id: string;
  equipmentType: EquipmentType;
  name: string;
  brand: string;
  model: string;
  serial: string;
  gpsLocation: string;
  status: 'Active' | 'Inactive';
};

type EquipmentForm = {
  id: string | null;
  equipmentName: string;
  equipmentType: EquipmentType;
  brand: string;
  model: string;
  serialNumber: string;
  gpsLocation: string;
  statusActive: boolean;
  features: string;
};

@Component({
  selector: 't360-rfid-equipment-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, T360HeaderComponent],
  templateUrl: './rfid-equipment.html',
})
export class RfidEquipmentComponent {
  readonly appRoutes = APP_ROUTES;
  readonly equipmentTypeOptions: readonly string[] = ['All', 'UHF RFID Antenna', 'Hand Held'];

  searchTerm = '';
  selectedEquipmentType = this.equipmentTypeOptions[0];

  showFormScreen = false;
  isEditing = false;
  selectedEquipmentIdForActions: string | null = null;
  selectedEquipmentForActions: RfidEquipment | null = null;
  actionsCardPosition: { top: string; left: string } = { top: '0px', left: '0px' };

  equipmentForm: EquipmentForm = this.createEmptyForm();

  equipments: RfidEquipment[] = [
    {
      id: 'eq-1',
      equipmentType: 'UHF RFID Antenna',
      name: 'L1',
      brand: 'Impinj',
      model: '950 MHZ',
      serial: '499996q23',
      gpsLocation: '-2.691101, -80.255084',
      status: 'Active',
    },
    {
      id: 'eq-2',
      equipmentType: 'Hand Held',
      name: 'HH01',
      brand: 'Chainway',
      model: 'C72 UHF RFID Reader',
      serial: '12457d545665',
      gpsLocation: '-2.694970, -80.256897',
      status: 'Active',
    },
  ];

  constructor(private readonly router: Router) {}

  @HostListener('document:click')
  onDocumentClick(): void {
    this.closeActions();
  }

  get filteredEquipments(): RfidEquipment[] {
    const term = this.searchTerm.trim().toLowerCase();

    return this.equipments
      .filter((equipment) =>
        this.selectedEquipmentType === 'All'
          ? true
          : equipment.equipmentType === this.selectedEquipmentType,
      )
      .filter((equipment) => {
        if (!term) return true;

        const searchableValues = [
          equipment.equipmentType,
          equipment.name,
          equipment.brand,
          equipment.model,
          equipment.serial,
          equipment.gpsLocation,
          equipment.status,
        ];

        return searchableValues.some((value) => value.toLowerCase().includes(term));
      });
  }

  goBackToConfigurations(): void {
    this.router.navigate([`/${APP_ROUTES.CONFIGURATIONS}`]);
  }

  openCreateForm(): void {
    this.isEditing = false;
    this.showFormScreen = true;
    this.equipmentForm = this.createEmptyForm();
    this.closeActions();
  }

  openEditForm(event: MouseEvent, equipment: RfidEquipment): void {
    event.stopPropagation();
    this.isEditing = true;
    this.showFormScreen = true;
    this.equipmentForm = {
      id: equipment.id,
      equipmentName: equipment.name,
      equipmentType: equipment.equipmentType,
      brand: equipment.brand,
      model: equipment.model,
      serialNumber: equipment.serial,
      gpsLocation: equipment.gpsLocation,
      statusActive: equipment.status === 'Active',
      features: '',
    };
    this.closeActions();
  }

  saveEquipment(): void {
    if (!this.equipmentForm.equipmentName.trim()) return;

    const payload: RfidEquipment = {
      id: this.equipmentForm.id ?? `eq-${Date.now()}`,
      equipmentType: this.equipmentForm.equipmentType,
      name: this.equipmentForm.equipmentName.trim(),
      brand: this.equipmentForm.brand.trim(),
      model: this.equipmentForm.model.trim(),
      serial: this.equipmentForm.serialNumber.trim(),
      gpsLocation: this.equipmentForm.gpsLocation.trim(),
      status: this.equipmentForm.statusActive ? 'Active' : 'Inactive',
    };

    if (this.isEditing && this.equipmentForm.id) {
      this.equipments = this.equipments.map((equipment) =>
        equipment.id === this.equipmentForm.id ? payload : equipment,
      );
    } else {
      this.equipments = [payload, ...this.equipments];
    }

    this.closeForm();
  }

  closeForm(): void {
    this.showFormScreen = false;
    this.isEditing = false;
    this.equipmentForm = this.createEmptyForm();
  }

  toggleEquipmentStatus(event: MouseEvent, equipment: RfidEquipment): void {
    event.stopPropagation();
    this.equipments = this.equipments.map((row) =>
      row.id === equipment.id
        ? { ...row, status: row.status === 'Active' ? 'Inactive' : 'Active' }
        : row,
    );
    this.closeActions();
  }

  toggleActions(event: MouseEvent, equipment: RfidEquipment): void {
    event.stopPropagation();
    const trigger = event.currentTarget as HTMLElement | null;

    if (!trigger) return;

    if (this.selectedEquipmentIdForActions === equipment.id) {
      this.closeActions();
      return;
    }

    this.selectedEquipmentIdForActions = equipment.id;
    this.selectedEquipmentForActions = equipment;
    this.updateActionsCardPosition(trigger);
  }

  closeActions(event?: MouseEvent): void {
    event?.stopPropagation();
    this.selectedEquipmentIdForActions = null;
    this.selectedEquipmentForActions = null;
  }

  trackByEquipmentId(_: number, equipment: RfidEquipment): string {
    return equipment.id;
  }

  private createEmptyForm(): EquipmentForm {
    return {
      id: null,
      equipmentName: '',
      equipmentType: 'UHF RFID Antenna',
      brand: '',
      model: '',
      serialNumber: '',
      gpsLocation: '',
      statusActive: true,
      features: '',
    };
  }

  private updateActionsCardPosition(trigger: HTMLElement): void {
    const rect = trigger.getBoundingClientRect();
    const cardWidth = 260;
    const margin = 12;
    const left = Math.max(margin, Math.min(window.innerWidth - cardWidth - margin, rect.right - cardWidth));

    this.actionsCardPosition = {
      top: `${rect.bottom + 8 + window.scrollY}px`,
      left: `${left + window.scrollX}px`,
    };
  }
}
