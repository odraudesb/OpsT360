import type { FileItemDto } from '../files/files.interface';

export type TransactionXmlDetails = {
    Contenedor?: {
        IDContenedor?: string;
        Tipo?: string;
        Estado?: string;
        Contenido?: {
            Item?:
            | Array<{
                Descripcion?: string;
                Cantidad?: string;
                ['Peso Unitario']?: string;
                ['Unidad Peso']?: string;
            }>
            | {
                Descripcion?: string;
                Cantidad?: string;
                ['Peso Unitario']?: string;
                ['Unidad Peso']?: string;
            };
        };
    };

    Origen?: string;
    Destino?: string;

    [k: string]: any;
};

export type TransactionDetailsDto = {
    transactionId: number;
    eventId?: number | null;
    entityType?: string;
    entityId?: number | null;
    recordDate?: string | null;
    eventDate?: string | null;
    xmlDetails?: TransactionXmlDetails | null;

    categoryId?: number | null;
    locationStatusId?: number | null;
    read?: number | null;
    isAlert?: boolean | null;
    isReefer?: boolean | null;
};

// ✅ Request para details-with-file
export interface TransactionDetailsWithFileRequestDto {
    entityId: number;
}

// ✅ Item que retorna details-with-file (según tu Postman)
export type TransactionDetailsWithFileItemDto = {
    transactionId: number;
    description: string;
    files?: FileItemDto[];

    eventId?: number | null;
    entityType?: string;
    entityId?: number | null;
    recordDate?: string | null;
    eventDate?: string | null;
    xmlDetails?: unknown;
};
