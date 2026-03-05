import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

import { ApiService } from '../api/api.service';
import type { FilesResponseDto, FileItemDto } from '../../interfaces/files/files.interface';

@Injectable({ providedIn: 'root' })
export class FilesService {
    constructor(private readonly api: ApiService) { }

    // ✅ GET /files/:transactionId -> puede venir múltiples archivos en data.files
    getFiles(transactionId: number): Observable<FileItemDto[]> {
        console.info('[FilesService] getFiles transactionId', transactionId);
        return this.api.get<FilesResponseDto | any>(`files/${transactionId}`).pipe(
            map((res) => {
                const data = res?.data ?? res;

                // Caso real: { data: { files: [ { ... } ] } }
                const filesArr = data?.files;
                if (Array.isArray(filesArr)) return filesArr as FileItemDto[];

                // Fallbacks por si cambia backend:
                if (Array.isArray(data)) return data as FileItemDto[];
                if (data && typeof data === 'object') return [data as FileItemDto];

                return [];
            }),
        );
    }
}
