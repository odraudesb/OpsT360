import type { ContainerStatus } from './container-statuses';

export interface ContainerRow {
  id: string;
  code: string;
  transactionId: number;
  status: ContainerStatus;
  category: string;
  cargoType: string;
  eta: string | null;
  lastEventDate: string | null;

  ship?: string | null;
  voyage?: string | null;

  lastEvent?: string | null;
  entityId?: number;
  isAlert?: boolean;
}
