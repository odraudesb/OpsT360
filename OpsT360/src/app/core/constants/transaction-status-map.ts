import type { ContainerStatus } from '../../interfaces/transactions/container-statuses';

export const TRANSACTION_STATUS_MAP: Record<number, ContainerStatus> = {
  1: 'shipped',
  2: 'inPort',
  3: 'inTransit',
  4: 'outOfPort',
  5: 'alerts',
};
