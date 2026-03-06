export type FileItemDto = {
    type?: string;
    description?: string;
    fileName?: string;
    length?: number;
    mimeType?: string;
    base64?: string;
};

export type FilesResponseDto = {
    data: FileItemDto[] | { files?: FileItemDto[] };
    errors?: Record<string, unknown>;
};

export type PdfSessionPayload = {
    base64: string;
    mimeType?: string;
    fileName?: string;
};
