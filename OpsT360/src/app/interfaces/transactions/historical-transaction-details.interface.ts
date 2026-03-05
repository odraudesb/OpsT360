import type { TransactionXmlDetails } from './transaction-details.interface';

export type HistoricalTransactionDetailsDto = {
    transactionId: number;
    eventId: number | null;
    eventName?: string | null;
    entityType: string;
    entityId: number;
    recordDate?: string | null;
    eventDate?: string | null;
    xmlDetails?: TransactionXmlDetails | null;
};