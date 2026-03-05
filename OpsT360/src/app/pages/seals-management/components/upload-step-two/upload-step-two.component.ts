import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UploadField } from '../../seals-management.types';

@Component({
  selector: 't360-upload-step-two',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './upload-step-two.component.html',
})
export class UploadStepTwoComponent {
  @Input() uploadColumns: string[] = [];
  @Input() uploadFields: UploadField[] = [];
  @Input() fieldMappings: Record<string, string> = {};
  @Input() mappedFields: UploadField[] = [];
  @Input() uploadFileName = '';
  @Input() rowsDetected = 0;
  @Input() fieldsMappedCount = 0;
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  isFieldMapped(fieldId: string): boolean {
    return Boolean(this.fieldMappings[fieldId] && this.fieldMappings[fieldId] !== 'no-mapping');
  }
}
