import type { FileItemDto } from '../../interfaces/files/files.interface';
import type { Field } from './field.type';

export type ShipmentDocRow = {
  transactionId: number;
  description: string;
  files?: FileItemDto[];
  xmlDetails?: unknown;
  fieldsCols?: [Field[], Field[]];
  isOpen?: boolean;
};
