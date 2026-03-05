export interface ReeferTransactionDetailsDto {
  transactionId?: number;
  eventId?: number | null;
  eventName?: string | null;
  recordDate?: string | null;
  eventDate?: string | null;
  [key: string]: unknown;
}
