export type SecurityTransactionDetailsDto = {
  transactionId: number;
  eventId: number;
  entityType: string;
  entityId: number;
  recordDate: string;
  eventDate: string;
  eventName: string;
  isAlert?: boolean | null;
  hasSeal: boolean;
  sealType: string | number | null;
  xmlDetails?: unknown;
};
