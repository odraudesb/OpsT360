import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  InventoryRow,
  UploadError,
  UploadField,
  UploadSummary,
} from '../../seals-management.types';
import { UploadStepFourComponent } from '../upload-step-four/upload-step-four.component';
import { UploadStepOneComponent } from '../upload-step-one/upload-step-one.component';
import { UploadStepThreeComponent } from '../upload-step-three/upload-step-three.component';
import { UploadStepTwoComponent } from '../upload-step-two/upload-step-two.component';

@Component({
  selector: 't360-upload-workflow-modal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    UploadStepFourComponent,
    UploadStepOneComponent,
    UploadStepThreeComponent,
    UploadStepTwoComponent,
  ],
  templateUrl: './upload-workflow-modal.component.html',
})
export class UploadWorkflowModalComponent implements OnInit, OnChanges {
  @Input() inventoryRows: InventoryRow[] = [];
  @Input() initialRow: InventoryRow | null = null;
  @Input() initialStep: 1 | 2 | 3 | 4 = 1;
  @Output() close = new EventEmitter<void>();

  uploadStep: 1 | 2 | 3 | 4 = 1;
  uploadFileName = '';
  uploadHasHeaders = true;
  selectedInventoryRow: InventoryRow | null = null;
  selectedPurchaseOrderId = '';

  uploadColumns: string[] = [];
  uploadMaxColumns = 0;

  uploadFields: UploadField[] = [
    { id: 'sealType', label: 'Seal Type', type: 'string', required: true },
    { id: 'sealNumber', label: 'Seal Number', type: 'string', required: true },
    { id: 'dateLoad', label: 'Date Load', type: 'date' },
    { id: 'requestOrder', label: 'Request Order', type: 'string' },
    { id: 'purchaseOrder', label: 'Purchase Order', type: 'string' },
    { id: 'receiverName', label: 'Receiver Name', type: 'string' },
    { id: 'status', label: 'Status', type: 'string' },
    { id: 'quantity', label: 'Quantity', type: 'number' },
  ];

  fieldMappings: Record<string, string> = {};
  uploadRows: Record<string, string>[] = [];
  uploadRawRows: string[][] = [];

  importPayload = '';
  importProgress = 0;

  importOptions = {
    skipDuplicates: true,
    overwriteAll: false,
    reportOnlyNonDuplicates: true,
  };

  importSummary: UploadSummary = {
    total: 0,
    success: 0,
    errors: 0,
  };

  importErrors: UploadError[] = [];
  reportRows: Record<string, string>[] = [];

  ngOnInit(): void {
    this.applyInitialState();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['initialRow'] || changes['initialStep']) {
      this.applyInitialState();
    }
  }

  setSelectedPurchaseOrderId(value: string): void {
    this.selectedPurchaseOrderId = value;
    this.selectedInventoryRow = this.inventoryRows.find((row) => row.id === value) ?? null;
  }

  goToUploadStep(step: 1 | 2 | 3 | 4): void {
    this.uploadStep = step;
  }

  closeUpload(): void {
    this.resetUploadState();
    this.close.emit();
  }

  updateImportOptions(options: {
    skipDuplicates: boolean;
    overwriteAll: boolean;
    reportOnlyNonDuplicates: boolean;
  }): void {
    this.importOptions = {
      ...options,
      skipDuplicates: options.overwriteAll ? false : options.skipDuplicates,
    };
  }

  async onUploadFileSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    const xlsx = (window as Window & { XLSX?: any }).XLSX;
    if (!xlsx) {
      console.warn('XLSX library not available. Please ensure it is loaded.');
      return;
    }

    this.uploadFileName = file.name;
    const arrayBuffer = await file.arrayBuffer();
    const workbook = xlsx.read(arrayBuffer, { type: 'array' });

    const best = this.pickBestSheet(workbook, xlsx);
    const sheet = best.sheet;

    const sheetRange = sheet && sheet['!ref'] ? xlsx.utils.decode_range(sheet['!ref']) : null;
    const sheetBounds = this.getSheetBounds(sheet, xlsx);

    type SheetCell = string | number | boolean | Date | null | undefined;
    type SheetRow = SheetCell[];

    const rows = xlsx.utils.sheet_to_json(sheet, {
      header: 1,
      defval: '',
      blankrows: true,
      range: sheetBounds ? sheetBounds : undefined,
    }) as SheetRow[];

    this.uploadMaxColumns = sheetBounds
      ? sheetBounds.e.c + 1
      : sheetRange
        ? sheetRange.e.c + 1
        : 0;

    this.uploadRawRows = rows.map((row) => row.map((cell) => (cell == null ? '' : String(cell))));
    this.applyUploadRows();
  }

  onUploadHeadersChange(value: boolean): void {
    this.uploadHasHeaders = value;
    this.applyUploadRows();
  }

  get mappedFields(): UploadField[] {
    return this.uploadFields.filter((field) => {
      const mapping = this.fieldMappings[field.id];
      return Boolean(mapping && mapping !== 'no-mapping');
    });
  }

  get mappedRows(): Record<string, string>[] {
    const fields = this.mappedFields;
    return this.uploadRows.map((row) => {
      const mapped: Record<string, string> = {};
      fields.forEach((field) => {
        const column = this.fieldMappings[field.id];
        mapped[field.label] = column ? row[column] ?? '' : '';
      });
      return mapped;
    });
  }

  get previewRows(): Record<string, string>[] {
    return this.mappedRows.slice(0, 5);
  }

  get fieldsMappedCount(): number {
    return this.uploadFields.filter((field) => {
      const mapping = this.fieldMappings[field.id];
      return Boolean(mapping && mapping !== 'no-mapping');
    }).length;
  }

  importData(): void {
    const mappedRows = this.mappedRows;
    const errors: UploadError[] = [];

    const seenKeys = new Map<string, number>();
    const duplicateLines = new Set<number>();

    const sealNumberField = this.uploadFields.find((field) => field.id === 'sealNumber');

    mappedRows.forEach((row, index) => {
      const values = Object.values(row).map((value) => (value ?? '').toString().trim());
      const isEmptyRow = values.every((value) => !value);

      if (isEmptyRow) {
        errors.push({ line: index + 1, message: 'Fila vacía detectada' });
        return;
      }

      if (sealNumberField) {
        const keyValue = (row[sealNumberField.label] ?? '').toString().trim();
        if (keyValue) {
          const existingLine = seenKeys.get(keyValue);
          if (existingLine !== undefined) {
            duplicateLines.add(index + 1);
            if (this.importOptions.skipDuplicates && !this.importOptions.overwriteAll) {
              errors.push({
                line: index + 1,
                column: sealNumberField.label,
                value: keyValue,
                message: 'Fila duplicada detectada',
              });
            }
          } else {
            seenKeys.set(keyValue, index + 1);
          }
        }
      }

      this.mappedFields.forEach((field) => {
        if (field.type === 'number') {
          const value = row[field.label] ?? '';
          const normalized = value.toString().trim();

          if (!normalized) {
            errors.push({
              line: index + 1,
              column: field.label,
              value: normalized,
              message: 'Valor numérico requerido',
            });
            return;
          }

          const isValid = Number.isFinite(Number(normalized));
          if (!isValid) {
            errors.push({
              line: index + 1,
              column: field.label,
              value: normalized,
              message: 'Formato inválido (solo números)',
            });
          }
        }
      });
    });

    const errorLines = new Set(errors.map((error) => error.line));

    const rowsToImport = mappedRows.filter((_, index) => {
      if (!this.importOptions.skipDuplicates || this.importOptions.overwriteAll) return true;
      return !duplicateLines.has(index + 1);
    });

    this.importPayload = JSON.stringify(rowsToImport, null, 2);
    this.reportRows = this.importOptions.reportOnlyNonDuplicates ? rowsToImport : mappedRows;
    this.importErrors = errors;
    this.importSummary = {
      total: mappedRows.length,
      success: mappedRows.length - errorLines.size,
      errors: errorLines.size,
    };
    this.importProgress = mappedRows.length ? 100 : 0;
    this.uploadStep = 4;
  }

  downloadReport(): void {
    if (!this.reportRows.length) return;

    const headers = Object.keys(this.reportRows[0]);
    const csvLines = [
      headers.join(','),
      ...this.reportRows.map((row) =>
        headers.map((header) => `"${(row[header] ?? '').toString().replace(/"/g, '""')}"`).join(','),
      ),
    ];

    const blob = new Blob([csvLines.join('\n')], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `seals-import-report-${Date.now()}.csv`;
    link.click();
    URL.revokeObjectURL(url);
  }

  private applyInitialState(): void {
    this.uploadStep = this.initialStep ?? 1;
    this.selectedInventoryRow = this.initialRow;
    this.selectedPurchaseOrderId = this.initialRow?.id ?? '';
  }

  private applyUploadRows(): void {
    if (!this.uploadRawRows.length) {
      this.uploadColumns = [];
      this.uploadRows = [];
      this.fieldMappings = {};
      this.uploadMaxColumns = 0;
      this.resetImportState();
      return;
    }

    const maxColumns = Math.max(
      this.uploadMaxColumns,
      ...this.uploadRawRows.map((row) => row.length),
      0,
    );

    let dataRows = this.uploadRawRows;

    if (this.uploadHasHeaders) {
      const [headerRow = []] = this.uploadRawRows;
      this.uploadColumns = Array.from({ length: maxColumns }, (_, index) => {
        const header = headerRow[index];
        return header ? header.trim() : `Column ${index + 1}`;
      });
      dataRows = this.uploadRawRows.slice(1);
    } else {
      this.uploadColumns = Array.from({ length: maxColumns }, (_, index) => `Column ${index + 1}`);
    }

    this.uploadRows = dataRows.map((row) => {
      const mappedRow: Record<string, string> = {};
      this.uploadColumns.forEach((column, index) => {
        mappedRow[column] = row[index] ?? '';
      });
      return mappedRow;
    });

    const nextMappings: Record<string, string> = {};
    this.uploadFields.forEach((field) => {
      const existing = this.fieldMappings[field.id];
      if (existing) {
        nextMappings[field.id] = existing;
        return;
      }

      const normalized = field.label.toLowerCase().trim();
      const matchedColumn = this.uploadColumns.find(
        (column) => column.toLowerCase().trim() === normalized,
      );
      nextMappings[field.id] = matchedColumn ?? 'no-mapping';
    });

    this.fieldMappings = nextMappings;
    this.resetImportState();
  }

  private pickBestSheet(workbook: any, xlsx: any): { name: string; sheet: any } {
    const sheetNames: string[] = workbook?.SheetNames ?? [];
    if (!sheetNames.length) return { name: '', sheet: null };

    let bestName = sheetNames[0];
    let bestSheet = workbook.Sheets[bestName];
    let bestScore = -1;
    let bestMaxCols = -1;

    for (const name of sheetNames) {
      const sheet = workbook.Sheets[name];
      if (!sheet) continue;

      const bounds = this.getSheetBounds(sheet, xlsx);

      const rows = xlsx.utils.sheet_to_json(sheet, {
        header: 1,
        defval: '',
        blankrows: false,
        range: bounds ? bounds : undefined,
      }) as any[][];

      let nonEmptyCells = 0;
      let maxCols = 0;

      for (const row of rows) {
        if (!row) continue;
        maxCols = Math.max(maxCols, row.length);
        for (const cell of row) {
          const v = (cell ?? '').toString().trim();
          if (v) nonEmptyCells++;
        }
      }

      if (nonEmptyCells > bestScore || (nonEmptyCells === bestScore && maxCols > bestMaxCols)) {
        bestScore = nonEmptyCells;
        bestMaxCols = maxCols;
        bestName = name;
        bestSheet = sheet;
      }
    }

    return { name: bestName, sheet: bestSheet };
  }

  private getSheetBounds(
    sheet: Record<string, any> | undefined,
    xlsx: {
      utils: {
        decode_range: (range: string) => { s: { r: number; c: number }; e: { r: number; c: number } };
        decode_cell: (cell: string) => { r: number; c: number };
      };
    },
  ): { s: { r: number; c: number }; e: { r: number; c: number } } | null {
    if (!sheet) return null;

    const decodedRange = sheet['!ref'] ? xlsx.utils.decode_range(sheet['!ref']) : null;
    let maxRow = decodedRange ? decodedRange.e.r : 0;
    let maxColumn = decodedRange ? decodedRange.e.c : 0;

    Object.keys(sheet).forEach((key) => {
      if (key.startsWith('!')) return;

      const cell = xlsx.utils.decode_cell(key);
      if (cell.r > maxRow) maxRow = cell.r;
      if (cell.c > maxColumn) maxColumn = cell.c;
    });

    return { s: { r: 0, c: 0 }, e: { r: maxRow, c: maxColumn } };
  }

  private resetImportState(): void {
    this.importPayload = '';
    this.importErrors = [];
    this.importSummary = { total: 0, success: 0, errors: 0 };
    this.importProgress = 0;
    this.reportRows = [];
  }

  private resetUploadState(): void {
    this.uploadStep = 1;
    this.uploadFileName = '';
    this.uploadHasHeaders = true;
    this.selectedInventoryRow = null;
    this.selectedPurchaseOrderId = '';
    this.uploadColumns = [];
    this.uploadMaxColumns = 0;
    this.fieldMappings = {};
    this.uploadRows = [];
    this.uploadRawRows = [];
    this.resetImportState();
    this.importOptions = {
      skipDuplicates: true,
      overwriteAll: false,
      reportOnlyNonDuplicates: true,
    };
  }
}
