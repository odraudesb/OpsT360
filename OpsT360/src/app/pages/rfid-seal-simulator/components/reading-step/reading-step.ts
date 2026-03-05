import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

type SealCard = {
  label: string;
  value: string;
  status: 'validated' | 'failed';
};

@Component({
  selector: 't360-reading-step',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reading-step.html',
})
export class ReadingStepComponent {
  @Input() containerId = '';
  @Output() readonly containerIdChange = new EventEmitter<string>();

  @Input() sectionLinkLabel = '';
  @Input() isGateIn = false;
  @Input() readFailures: boolean[] = [];
  @Input() failureSelections: boolean[] = [];
  @Input() failureSealNumbers = '';
  @Input() locationLabel = '';
  @Input() sealCards: SealCard[] = [];
  @Input() hasFailures = false;
  @Input() showResults = false;
  @Input() isProcessing = false;
  @Input() canTriggerReading = true;

  @Output() readonly triggerReading = new EventEmitter<void>();

  trackByIndex = (index: number): number => index;
}
