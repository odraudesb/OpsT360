import type { Field } from './field.type';

export type ContainerHistoryDetail = {
  id: string;
  transactionId: number;
  eventId: number | null;
  eventName: string;
  recordDate: Date | null;
  eventDate: Date | null;
  xmlDetails: unknown;
  isOpen: boolean;
  fieldsCols: [Field[], Field[]];
};
