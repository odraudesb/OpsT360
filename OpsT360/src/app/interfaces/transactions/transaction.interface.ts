export type TransactionDto = {
  transactionId: number;
  eventId: number;
  eventDescription: string;
  entityType: string;
  entityId: number;
  recordDate: string;
  eventDate: string;
  eta: string;
  ship: string | null;
  categoryId: number | null;
  locationStatusId: number | null;
  read: number | null;
  isAlert: boolean | null;
  isReefer: boolean | null;
};

export type ApiResponse<T> = {
  data: T;
  errors?: unknown;
};
