import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { T360HeaderComponent } from '../../components/header/t360-header/t360-header';

type ServiceCard = {
  id: string;
  name: string;
  price: string;
  description: string;
};

@Component({
  selector: 't360-subscriptions-page',
  standalone: true,
  imports: [CommonModule, FormsModule, T360HeaderComponent],
  templateUrl: './subscriptions.html',
})
export class SubscriptionsComponent {
  customerName = '';
  customerId = '';

  readonly exportServices: ServiceCard[] = [
    {
      id: 'containerized-export',
      name: 'Containerized Cargo',
      price: '10 USD / Cntr',
      description:
        'Full tracking of containerized export cargo: Documentation, logistics events, cargo security, reefer indicators',
    },
    {
      id: 'cfs-export',
      name: 'CFS Cargo',
      price: '15 USD / Cntr',
      description:
        'Full tracking of CFS cargo: Documentation, logistics events, cargo security, reefer indicators',
    },
    {
      id: 'breakbulk-export',
      name: 'Break Bulk Cargo',
      price: '20 USD / Bulk',
      description:
        'Full tracking of containerized export cargo: Documentation, logistics events, cargo security, reefer indicators',
    },
    {
      id: 'banana-export',
      name: 'Banana Cargo',
      price: '18 USD / Truck',
      description:
        'Full tracking of CFS cargo: Documentation, logistics events, cargo security, reefer indicators',
    },
  ];

  readonly importServices: ServiceCard[] = [
    {
      id: 'containerized-import',
      name: 'Containerized Cargo',
      price: '10 USD / Cntr',
      description:
        'Full tracking of containerized import cargo: Documentation, logistics events, cargo security, reefer indicators',
    },
    {
      id: 'cfs-import',
      name: 'CFS Cargo',
      price: '15 USD / Cntr',
      description:
        'Full tracking of CFS cargo: Documentation, logistics events, cargo security, reefer indicators',
    },
    {
      id: 'breakbulk-import',
      name: 'Break Bulk Cargo',
      price: '20 USD / Bulk',
      description:
        'Full tracking of Break Bulk import cargo: Documentation, logistics events, cargo security, reefer indicators',
    },
  ];

  readonly termsText = `Al suscribirse al servicio T360° de trazabilidad de carga, usted acepta los siguientes términos y condiciones:
1. La suscripción tiene un período indefinido y continuará hasta que sea cancelada por el cliente.
2. Los cargos se aplicarán por cada contenedor procesado a través de la terminal portuaria.
3. El servicio incluye notificaciones en tiempo real sobre el estado de la carga a través de la aplicación T360°.
4. El cliente puede cancelar la suscripción en cualquier momento a través de este portal o contactando al servicio al cliente.
5. Los precios están sujetos a cambios con previo aviso de 30 días.
6. El servicio T360° no incluye responsabilidad por demoras o problemas en la cadena logística fuera del control de la terminal.
7. Para más información sobre el manejo de datos personales, consulte nuestra política de privacidad.`;

  trackByServiceId(_: number, service: ServiceCard): string {
    return service.id;
  }
}
