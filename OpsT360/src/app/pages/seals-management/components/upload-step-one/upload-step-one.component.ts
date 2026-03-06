import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 't360-upload-step-one',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './upload-step-one.component.html',
})
export class UploadStepOneComponent {
  @Input() uploadFileName = '';
  @Input() uploadHasHeaders = true;
  @Input() purchaseOrderId = '';
  @Input() requestOrderId = '';
  @Input() purchaseOrderOptions: { id: string; description?: string }[] = [];
  @Input() requester = '';
  @Input() requestDate = '';
  @Input() requestDescription = '';
  @Output() uploadHasHeadersChange = new EventEmitter<boolean>();
  @Output() purchaseOrderIdChange = new EventEmitter<string>();
  @Output() fileSelected = new EventEmitter<Event>();
  @Output() cancel = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();
}
