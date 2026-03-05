import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

type EvidencePanel = {
  id: string;
  label: string;
  previewUrl: string | null;
  file?: File | null;
  validatedFile?: File | null;
  imageBytes?: number[] | null;
  validationStatus?: 'idle' | 'pending' | 'success' | 'failed';
};

@Component({
  selector: 't360-pre-gate-step',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pre-gate-step.html',
})
export class PreGateStepComponent {
  @Input() containerId = '';
  @Output() readonly containerIdChange = new EventEmitter<string>();

  @Input() accessPanels: EvidencePanel[] = [];
  @Input() containerImage: EvidencePanel = {
    id: '',
    label: '',
    previewUrl: null,
    imageBytes: null,
    validationStatus: 'idle',
  };
  @Input() seals: string[] = [];
  @Input() validationState: 'idle' | 'processing' | 'ok' = 'idle';
  @Input() displayedProcessingMessages: string[] = [];
  @Input() isDataCaptureComplete = false;
  @Input() hasSeals = false;

  @Input() triggerFileInput: (input: HTMLInputElement) => void = () => {};
  @Input() onEvidenceSelected: (event: Event, panel: EvidencePanel) => void = () => {};
  @Input() openPreview: (
    url: string | null,
    label: string,
    status?: 'idle' | 'pending' | 'success' | 'failed',
  ) => void = () => {};
  @Input() readSeals: () => void = () => {};
  @Input() validate: () => void = () => {};
}
