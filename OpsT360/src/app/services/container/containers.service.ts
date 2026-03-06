import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';
import { ContainersExportacion } from '../../interfaces/containers/containers-exportacion';

@Injectable({ providedIn: 'root' })
export class ContainersService {
  // Mock data
  private readonly mockContainers: ContainersExportacion[] = [
    {
      id: 'C-001',
      code: 'MSDU1234567',
      operationType: 'import',
      status: 'completed',
      eta: '2025-01-10T09:00:00Z',
      client: 'Acme Corp',
      lastEvent: undefined,
      category: undefined,
      cargoType: undefined,
      containerId: '',
      type: '',
      date: ''
    },
    {
      id: 'C-002',
      code: 'MAEU7654321',
      operationType: 'export',
      status: 'in_transit',
      eta: '2025-01-11T14:30:00Z',
      client: 'Globex Inc.',
      lastEvent: undefined,
      category: undefined,
      cargoType: undefined,
      containerId: '',
      type: '',
      date: ''
    },
    {
      id: 'C-003',
      code: 'TRIU9876543',
      operationType: 'import',
      status: 'delivered',
      eta: '2025-01-12T08:15:00Z',
      client: 'Initech',
      lastEvent: undefined,
      category: undefined,
      cargoType: undefined,
      containerId: '',
      type: '',
      date: ''
    },
  ];

  getAll(): Observable<ContainersExportacion[]> {
    // Simula llamada al backend
    return of(this.mockContainers).pipe(delay(800));
  }
}
