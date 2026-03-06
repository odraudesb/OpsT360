import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, NgZone, OnDestroy } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { T360HeaderComponent } from '../../components/header/t360-header/t360-header';
import { APP_ROUTES } from '../../core/constants/app-routes.constants';
import { TRANSACTION_EVENTS } from '../../core/constants/transaction-events.constants';
import { TransactionsService } from '../../services/transactions/transactions.service';
import { PreGateStepComponent } from './components/pre-gate-step/pre-gate-step';
import { ReadingStepComponent } from './components/reading-step/reading-step';
import { RoboflowValidationService } from './services/roboflow-validation.service';

type SimulatorStepId = 'pre-gate' | 'gate-in' | 'loading-north' | 'loading-south';

type SimulatorStep = {
  id: SimulatorStepId;
  code: string;
  labelLines: string[];
};

type ReadingStepState = {
  readFailures: boolean[];
  failureSelections: boolean[];
  hasReadingResults: boolean;
};



type ContainerXmlProfile = {
  size: string;
  shippingLine: string;
  originPort: string;
  loadingPort: string;
  dischargePort: string;
  destinationPort: string;
  goods: string;
  booking: string;
  originWeight: number;
  terminalEntryDate: string;
  preNoticeDate: string;
  carrier: string;
  plate: string;
  driver: string;
  customsSeal: string;
  observations: string;
};

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
  selector: 't360-rfid-seal-simulator',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    T360HeaderComponent,
    PreGateStepComponent,
    ReadingStepComponent,
  ],
  templateUrl: './rfid-seal-simulator.html',
  styleUrl: './rfid-seal-simulator.css',
})
export class RfidSealSimulatorComponent implements OnDestroy {
  private readonly simulatorEntityIdByContainer: Record<string, number> = {
    HSU3523430: 100004,
    TEMU3523471: 100003,
    SUDU3436921: 100002,
    MSCU8460995: 100001,
    MAEU5083343: 90009,
    MAEU7429918: 80008,
    MAEU9045524: 70007,
    MAEU6651037: 60006,
    CMAU9921401: 40004,
    CMAU7004129: 30003,
    CMAU5573305: 20002,
    CMAU2849912: 20001,
    TEMU0927644: 12345,
  };

  // Comentario solicitado: perfiles XML inventados por contenedor para el simulador.
  private readonly simulatorXmlProfileByContainer: Record<string, ContainerXmlProfile> = {
    HSU3523430: {
      size: '40HC', shippingLine: 'HAPAG-LLOYD', originPort: 'ECGYE', loadingPort: 'ECGYE', dischargePort: 'USMIA', destinationPort: 'USMIA',
      goods: 'Piña fresca', booking: 'BK2025-000101', originWeight: 22340, terminalEntryDate: '2025-09-10T08:15:00', preNoticeDate: '2025-09-08T13:40:00',
      carrier: 'TRANSPORTES PACIFICO S.A.', plate: 'ABC101', driver: 'Conductor 1', customsSeal: 'SAP561', observations: 'Contenedor refrigerado, temperatura 8°C',
    },
    TEMU3523471: {
      size: '40HC', shippingLine: 'ONE', originPort: 'ECGYE', loadingPort: 'ECGYE', dischargePort: 'USMIA', destinationPort: 'USMIA',
      goods: 'Mango fresco', booking: 'BK2025-000102', originWeight: 21980, terminalEntryDate: '2025-09-10T09:00:00', preNoticeDate: '2025-09-08T14:00:00',
      carrier: 'RUTAS DEL ECUADOR S.A.', plate: 'ABC102', driver: 'Conductor 2', customsSeal: 'SAP562', observations: 'Cadena de frío activa 10°C',
    },
    SUDU3436921: {
      size: '40HC', shippingLine: 'HMM', originPort: 'ECGYE', loadingPort: 'ECGYE', dischargePort: 'USMIA', destinationPort: 'USMIA',
      goods: 'Papaya fresca', booking: 'BK2025-000103', originWeight: 22850, terminalEntryDate: '2025-09-10T09:35:00', preNoticeDate: '2025-09-08T15:00:00',
      carrier: 'LOGISTICA DEL LITORAL C.A.', plate: 'ABC103', driver: 'Conductor 3', customsSeal: 'SAP563', observations: 'Sin novedades durante inspección',
    },
    MSCU8460995: {
      size: '40HC', shippingLine: 'MSC', originPort: 'ECGYE', loadingPort: 'ECGYE', dischargePort: 'USMIA', destinationPort: 'USMIA',
      goods: 'Banano fresco', booking: 'BK2025-000004', originWeight: 23400, terminalEntryDate: '2025-09-09T08:00:00', preNoticeDate: '2025-09-07T15:30:00',
      carrier: 'TRANSPORTES ANDES S.A.', plate: 'ABC124', driver: 'Conductor 4', customsSeal: 'SAP564', observations: 'Contenedor refrigerado, temperatura 13°C',
    },
    MAEU5083343: {
      size: '40HC', shippingLine: 'MAERSK', originPort: 'ECGYE', loadingPort: 'ECGYE', dischargePort: 'USMIA', destinationPort: 'USMIA',
      goods: 'Pitahaya fresca', booking: 'BK2025-000105', originWeight: 21650, terminalEntryDate: '2025-09-10T10:10:00', preNoticeDate: '2025-09-08T15:20:00',
      carrier: 'ECUATORIANA DE CARGA S.A.', plate: 'ABC105', driver: 'Conductor 5', customsSeal: 'SAP565', observations: 'Despacho prioritario exportación',
    },
    MAEU7429918: {
      size: '40HC', shippingLine: 'MAERSK', originPort: 'ECGYE', loadingPort: 'ECGYE', dischargePort: 'USMIA', destinationPort: 'USMIA',
      goods: 'Plátano orgánico', booking: 'BK2025-000106', originWeight: 23020, terminalEntryDate: '2025-09-10T10:55:00', preNoticeDate: '2025-09-08T16:00:00',
      carrier: 'RUTA COSTERA LOGISTICS', plate: 'ABC106', driver: 'Conductor 6', customsSeal: 'SAP566', observations: 'Contenedor pre-enfriado 11°C',
    },
    MAEU9045524: {
      size: '40HC', shippingLine: 'MAERSK', originPort: 'ECGYE', loadingPort: 'ECGYE', dischargePort: 'USMIA', destinationPort: 'USMIA',
      goods: 'Cacao en grano', booking: 'BK2025-000107', originWeight: 21000, terminalEntryDate: '2025-09-10T11:25:00', preNoticeDate: '2025-09-08T16:40:00',
      carrier: 'TRAMAR CARGA ECUADOR', plate: 'ABC107', driver: 'Conductor 7', customsSeal: 'SAP567', observations: 'Sellado y pesado en origen',
    },
    MAEU6651037: {
      size: '40HC', shippingLine: 'MAERSK', originPort: 'ECGYE', loadingPort: 'ECGYE', dischargePort: 'USMIA', destinationPort: 'USMIA',
      goods: 'Yuca procesada', booking: 'BK2025-000108', originWeight: 20540, terminalEntryDate: '2025-09-10T12:05:00', preNoticeDate: '2025-09-08T17:10:00',
      carrier: 'ANDINA FREIGHT MOVERS', plate: 'ABC108', driver: 'Conductor 8', customsSeal: 'SAP568', observations: 'Unidad apta para carga seca',
    },
    CMAU9921401: {
      size: '40HC', shippingLine: 'CMA CGM', originPort: 'ECGYE', loadingPort: 'ECGYE', dischargePort: 'USMIA', destinationPort: 'USMIA',
      goods: 'Atún en conserva', booking: 'BK2025-000109', originWeight: 19870, terminalEntryDate: '2025-09-10T12:45:00', preNoticeDate: '2025-09-08T17:35:00',
      carrier: 'LOGISTICA ECUAMAR', plate: 'ABC109', driver: 'Conductor 9', customsSeal: 'SAP569', observations: 'Inspección documental completada',
    },
    CMAU7004129: {
      size: '40HC', shippingLine: 'CMA CGM', originPort: 'ECGYE', loadingPort: 'ECGYE', dischargePort: 'USMIA', destinationPort: 'USMIA',
      goods: 'Camarón congelado', booking: 'BK2025-000110', originWeight: 22560, terminalEntryDate: '2025-09-10T13:20:00', preNoticeDate: '2025-09-08T18:00:00',
      carrier: 'TRANSCOLD ECUADOR', plate: 'ABC110', driver: 'Conductor 10', customsSeal: 'SAP570', observations: 'Reefer configurado a -18°C',
    },
    CMAU5573305: {
      size: '40HC', shippingLine: 'CMA CGM', originPort: 'ECGYE', loadingPort: 'ECGYE', dischargePort: 'USMIA', destinationPort: 'USMIA',
      goods: 'Maracuyá fresca', booking: 'BK2025-000111', originWeight: 21480, terminalEntryDate: '2025-09-10T14:00:00', preNoticeDate: '2025-09-08T18:20:00',
      carrier: 'CARGA EXPRESS COSTA', plate: 'ABC111', driver: 'Conductor 11', customsSeal: 'SAP571', observations: 'Sello aduana verificado en patio',
    },
    CMAU2849912: {
      size: '40HC', shippingLine: 'CMA CGM', originPort: 'ECGYE', loadingPort: 'ECGYE', dischargePort: 'USMIA', destinationPort: 'USMIA',
      goods: 'Uva de mesa', booking: 'BK2025-000112', originWeight: 22110, terminalEntryDate: '2025-09-10T14:30:00', preNoticeDate: '2025-09-08T18:40:00',
      carrier: 'PACIFIC TRUCKING GROUP', plate: 'ABC112', driver: 'Conductor 12', customsSeal: 'SAP572', observations: 'Carga sensible, manipulación cuidadosa',
    },
    TEMU0927644: {
      size: '40HC', shippingLine: 'ONE', originPort: 'ECGYE', loadingPort: 'ECGYE', dischargePort: 'USMIA', destinationPort: 'USMIA',
      goods: 'Arándano fresco', booking: 'BK2025-000113', originWeight: 20880, terminalEntryDate: '2025-09-10T15:00:00', preNoticeDate: '2025-09-08T19:00:00',
      carrier: 'NORTE SUR TRANSPORT', plate: 'ABC113', driver: 'Conductor 13', customsSeal: 'SAP573', observations: 'Control térmico continuo 6°C',
    },
  };

  private completedStepIndexes = new Set<number>();
  private readonly readingStateByStep = new Map<SimulatorStepId, ReadingStepState>();


  readonly steps: SimulatorStep[] = [
    { id: 'pre-gate', code: 'P', labelLines: ['PRE-GATE'] },
    { id: 'gate-in', code: 'G', labelLines: ['GATE IN'] },
    { id: 'loading-north', code: 'L1', labelLines: ['LOADING - NORTH', 'BERTH SIDE'] },
    { id: 'loading-south', code: 'L2', labelLines: ['LOADING - SOUTH', 'BERTH SIDE'] },
  ];

  activeStep: SimulatorStepId = 'pre-gate';

  containerId = '';
  activationPoint = 'Pre-Gate';
  cellphoneMode = false;

  accessPanels: EvidencePanel[] = [
    {
      id: 'panel-1',
      label: 'Access Panel 1',
      previewUrl: null,
      imageBytes: null,
      validationStatus: 'idle',
    },
    {
      id: 'panel-2',
      label: 'Access Panel 2',
      previewUrl: null,
      imageBytes: null,
      validationStatus: 'idle',
    },
  ];

  containerImage: EvidencePanel = {
    id: 'container-image',
    label: 'Container image',
    previewUrl: null,
    imageBytes: null,
    validationStatus: 'idle',
  };

  seals = ['', '', '', ''];
  readFailures = [false, false, false, false];
  failureSelections = [false, false, false, false];
  hasReadingResults = false;
  isReadingProcessing = false;
  private readingTimeoutId?: number;

  validationState: 'idle' | 'processing' | 'ok' = 'idle';
  previewImageUrl: string | null = null;
  previewImageLabel: string | null = null;
  previewImageStatus: 'success' | 'failed' | null = null;
  previewZoom = 1;
  displayedProcessingMessages: string[] = [];

  private readonly processingMessages = [
    'Collecting images...',
    'Access Panel 1 detection...',
    'Access Panel 2 detection...',
    'RFID Labels detection...',
    'Segmentation Label / Access Panels process',
  ];
  private processingIntervalId?: number;
  private validationTimeoutId?: number;

  constructor(
    private readonly router: Router,
    private readonly transactionsService: TransactionsService,
    private readonly roboflowValidationService: RoboflowValidationService,
    private readonly ngZone: NgZone,
    private readonly changeDetectorRef: ChangeDetectorRef,
  ) { }

  ngOnDestroy(): void {
    this.clearProcessingTimers();
    this.clearReadingTimer();
  }

  setActiveStep(stepId: SimulatorStepId): void {
    if (!this.canSelectStep(stepId)) {
      return;
    }
    this.updateActiveStep(stepId);
  }

  goToNextStep(): void {
    if (!this.canProceedNext) {
      return;
    }

    const currentIndex = this.steps.findIndex((step) => step.id === this.activeStep);
    if (currentIndex >= 0) {
      // Comentario solicitado: este registro habilita volver/avanzar sin perder datos entre pasos.
      this.completedStepIndexes.add(currentIndex);
    }

    if (this.activeStep === 'loading-south') {
      this.resetSimulatorWorkflow();
      return;
    }

    const nextStep = this.steps[currentIndex + 1];
    if (nextStep) {
      this.storeCurrentReadingState();
      this.updateActiveStep(nextStep.id);
    }
  }

  goToPreviousStep(): void {
    this.storeCurrentReadingState();
    const currentIndex = this.steps.findIndex((step) => step.id === this.activeStep);
    const prevStep = this.steps[currentIndex - 1];
    if (prevStep) {
      this.updateActiveStep(prevStep.id);
    }
  }

  cancel(): void {
    this.router.navigate(['/', APP_ROUTES.HOME]);
  }

  triggerFileInput = (input: HTMLInputElement): void => {
    input.click();
  };

  onEvidenceSelected = (event: Event, panel: EvidencePanel): void => {
    const input = event.target as HTMLInputElement;
    const [file] = input.files ?? [];
    if (file) {
      panel.previewUrl = URL.createObjectURL(file);
      panel.file = file;
      panel.validatedFile = null;
      panel.validationStatus = 'idle';
      void this.loadImageBytes(file, panel);
    }
  };

  readSeals = (): void => {
    this.seals = this.seals.map(() => this.generateSealCode());
  };

  handleReadingAction(): void {
    if (this.isPreGate || this.isReadingProcessing || !this.containerId.trim()) {
      return;
    }

    this.isReadingProcessing = true;
    if (this.readingTimeoutId) {
      window.clearTimeout(this.readingTimeoutId);
      this.readingTimeoutId = undefined;
    }

    this.readingTimeoutId = window.setTimeout(async () => {
      this.hasReadingResults = true;
      const failureIndexes = this.failureSelections
        .map((selected, index) => (selected ? index : -1))
        .filter((index) => index !== -1);

      if (this.activeStep === 'gate-in') {
        this.seals = this.seals.map(() => this.generateSealCode());
      }

      this.readFailures = this.readFailures.map((_, index) => failureIndexes.includes(index));

      const successDate = new Date();
      await this.registerStepEventTransaction(successDate);

      if (this.readFailures.some(Boolean)) {
        await this.registerStepFailureTransaction(failureIndexes, new Date(successDate.getTime() + 1000));
      }

      this.isReadingProcessing = false;
      this.readingTimeoutId = undefined;
      this.changeDetectorRef.detectChanges();
    }, 2200);
  }

  validate = (): void => {
    if (!this.isDataCaptureComplete) {
      return;
    }

    this.validationState = 'processing';
    this.displayedProcessingMessages = [];
    this.clearProcessingTimers();
    let messageIndex = 0;
    const validationDelayMs = 600;

    this.ngZone.run(() => {
      this.processingIntervalId = window.setInterval(() => {
        this.displayedProcessingMessages = [
          ...this.displayedProcessingMessages,
          this.processingMessages[messageIndex],
        ];
        this.changeDetectorRef.detectChanges();
        messageIndex += 1;

        if (messageIndex >= this.processingMessages.length) {
          window.clearInterval(this.processingIntervalId);
          this.processingIntervalId = undefined;
        }
      }, 120);

      this.validationTimeoutId = window.setTimeout(() => {
        this.displayedProcessingMessages = [];
        void this.processValidationApis();
      }, validationDelayMs);
    });
  };

  get hasFailures(): boolean {
    return this.readFailures.some(Boolean);
  }

  get canProceedNext(): boolean {
    if (this.activeStep === 'pre-gate') {
      return this.validationState === 'ok';
    }
    if (this.activeStep === 'loading-south') {
      return true;
    }
    return this.hasReadingResults;
  }

  canSelectStep(stepId: SimulatorStepId): boolean {
    const targetIndex = this.steps.findIndex((step) => step.id === stepId);
    if (targetIndex === -1) {
      return false;
    }

    const highestCompleted = this.completedStepIndexes.size
      ? Math.max(...Array.from(this.completedStepIndexes))
      : -1;
    const maxUnlockedIndex = Math.min(highestCompleted + 1, this.steps.length - 1);

    // Comentario solicitado: solo se puede avanzar hasta pasos previamente desbloqueados.
    return targetIndex <= maxUnlockedIndex;
  }

  get isDataCaptureComplete(): boolean {
    const hasAccessPanels = this.accessPanels.every((panel) => panel.previewUrl);
    const hasContainerEvidence = Boolean(this.containerImage.previewUrl);
    const hasSeals = this.seals.every((seal) => seal.trim().length > 0);
    const hasContainerId = this.containerId.trim().length > 0;

    return hasAccessPanels && hasContainerEvidence && hasSeals && hasContainerId;
  }

  get hasSeals(): boolean {
    return this.seals.every((seal) => seal.trim().length > 0);
  }

  get failureSealNumbers(): string {
    return this.readFailures
      .map((flag, index) => (flag ? this.seals[index] || `RFID-${index + 1}` : null))
      .filter(Boolean)
      .join('; ');
  }

  get nextStepLabel(): string {
    if (this.activeStep === 'pre-gate') {
      return 'Gate In';
    }
    if (this.activeStep === 'gate-in') {
      return 'LOADING L1';
    }
    if (this.activeStep === 'loading-north') {
      return 'LOADING L2';
    }
    return 'INIT';
  }


  get cancelButtonLabel(): string {
    return this.activeStep === 'loading-south' ? 'Exit' : 'Cancel';
  }

  get nextButtonLabel(): string {
    if (this.activeStep === 'loading-south') {
      return 'INIT';
    }
    return `Next: ${this.nextStepLabel}`;
  }

  get canTriggerReading(): boolean {
    return this.containerId.trim().length > 0;
  }

  get isPreGate(): boolean {
    return this.activeStep === 'pre-gate';
  }

  get isGateIn(): boolean {
    return this.activeStep === 'gate-in';
  }

  get isLoadingStep(): boolean {
    return this.activeStep === 'loading-north' || this.activeStep === 'loading-south';
  }

  get sectionLinkLabel(): string {
    if (this.activeStep === 'loading-north') {
      return 'RFID Seal Reading Control previous to load the container - North Berth Side';
    }
    if (this.activeStep === 'loading-south') {
      return 'RFID Seal Reading Control previous to load the container - South Berth Side';
    }
    return 'RFID Seal Reading Control at Gate in';
  }

  get locationLabel(): string {
    if (this.activeStep === 'loading-north') {
      return 'NORTH BERTH SIDE';
    }
    if (this.activeStep === 'loading-south') {
      return 'SOUTH BERTH SIDE';
    }
    return 'GATE IN';
  }

  get sealCards(): { label: string; value: string; status: 'validated' | 'failed' }[] {
    return this.seals.map((value, index) => ({
      label: `RFID - ${value || `Seal ${index + 1}`}`,
      value: value || 'JAN, 20, 2026 17:46',
      status: this.readFailures[index] ? 'failed' : 'validated',
    }));
  }

  onContainerIdChange(containerId: string): void {
    if (this.containerId === containerId) {
      return;
    }
    this.containerId = containerId;
    this.resetReadingState();
  }

  private getResolvedEntityId(): number {
    const containerId = this.containerId.trim().toUpperCase();
    return this.simulatorEntityIdByContainer[containerId] ?? 100004;
  }

  private generateSealCode(): string {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
    return Array.from({ length: 12 }, () => chars[Math.floor(Math.random() * chars.length)]).join(
      '',
    );
  }

  private updateActiveStep(stepId: SimulatorStepId): void {
    if (this.activeStep === stepId) {
      return;
    }

    const currentIndex = this.steps.findIndex((step) => step.id === this.activeStep);
    const targetIndex = this.steps.findIndex((step) => step.id === stepId);
    const isGoingBack = targetIndex >= 0 && currentIndex >= 0 && targetIndex < currentIndex;

    this.activeStep = stepId;

    if (stepId === 'pre-gate') {
      return;
    }

    if (isGoingBack) {
      this.restoreReadingState(stepId);
      return;
    }

    this.resetReadingState();
  }

  private resetReadingState(): void {
    this.clearReadingTimer();
    this.readFailures = [false, false, false, false];
    this.failureSelections = [false, false, false, false];
    this.hasReadingResults = false;
  }

  private storeCurrentReadingState(): void {
    if (this.activeStep === 'pre-gate') {
      return;
    }

    this.readingStateByStep.set(this.activeStep, {
      readFailures: [...this.readFailures],
      failureSelections: [...this.failureSelections],
      hasReadingResults: this.hasReadingResults,
    });
  }

  private restoreReadingState(stepId: SimulatorStepId): void {
    const saved = this.readingStateByStep.get(stepId);
    if (!saved) {
      this.resetReadingState();
      return;
    }

    this.clearReadingTimer();
    this.readFailures = [...saved.readFailures];
    this.failureSelections = [...saved.failureSelections];
    this.hasReadingResults = saved.hasReadingResults;
  }

  openPreview = (
    url: string | null,
    label: string,
    status: 'idle' | 'pending' | 'success' | 'failed' | undefined = undefined,
  ): void => {
    if (!url) {
      return;
    }
    this.previewImageUrl = url;
    this.previewImageLabel = label;
    this.previewImageStatus = status === 'success' || status === 'failed' ? status : null;
    this.previewZoom = 1;
  };

  closePreview = (): void => {
    this.previewImageUrl = null;
    this.previewImageLabel = null;
    this.previewImageStatus = null;
    this.previewZoom = 1;
  };

  increasePreviewZoom = (): void => {
    this.previewZoom = Math.min(this.previewZoom + 0.2, 4);
  };

  decreasePreviewZoom = (): void => {
    this.previewZoom = Math.max(this.previewZoom - 0.2, 0.5);
  };

  handlePreviewWheel = (event: WheelEvent): void => {
    event.preventDefault();
    if (event.deltaY < 0) {
      this.increasePreviewZoom();
      return;
    }
    this.decreasePreviewZoom();
  };


  private async processValidationApis(): Promise<void> {
    const evidencePanels = this.getEvidencePanels();
    const validationPanels = this.getValidationPanels();

    if (!evidencePanels.length) {
      return;
    }

    validationPanels.forEach((panel) => {
      panel.validationStatus = 'pending';
    });
    if (this.containerImage.file) {
      this.containerImage.validationStatus = 'success';
    }
    this.changeDetectorRef.detectChanges();

    await Promise.allSettled(validationPanels.map((panel) => this.validatePhotoEvidence(panel)));

    const failedPanels = validationPanels.filter((panel) => panel.validationStatus === 'failed');

    const evidencePhotos = evidencePanels
      .map((panel) => panel.validatedFile ?? panel.file)
      .filter((file): file is File => Boolean(file));

    const successDate = new Date();
    await this.registerTransactionWithFiles(evidencePhotos, successDate);
    await this.registerValidationFailureTransaction(
      failedPanels,
      new Date(successDate.getTime() + 1000),
    );
    this.validationState = 'ok';
    this.changeDetectorRef.detectChanges();
  }

  private getEvidencePanels(): EvidencePanel[] {
    const panels = [...this.accessPanels, this.containerImage];
    return panels.filter((panel) => panel.file);
  }

  private getValidationPanels(): EvidencePanel[] {
    return this.accessPanels.filter((panel) => panel.file);
  }

  private async registerTransactionWithFiles(photos: File[], timestamp = new Date()): Promise<void> {
    try {
      const event = this.getCurrentStepBaseEvent();
      const entityType = this.containerId.trim().toUpperCase();
      const xmlDetails = this.buildContainerXmlDetails(event);

      await firstValueFrom(
        this.transactionsService.registerTransactionWithFiles({
          photos,
          eventId: event.id,
          entityType,
          entityId: this.getResolvedEntityId(),
          status: 1,
          isReefer: false,
          details: event.name,
          document: this.getCurrentStepDocumentLabel(),
          xmlDetails,
          recordDate: timestamp.toISOString(),
          eventDate: timestamp.toISOString(),
        }),
      );
    } catch (error) {
      console.error('[RFID Simulator] register transaction with photos error', error);
    }
  }

  private async validatePhotoEvidence(panel: EvidencePanel): Promise<void> {
    const photo = panel.file;
    if (!photo) {
      panel.validationStatus = 'failed';
      return;
    }

    try {
      const imageBase64 = await this.fileToBase64(photo);
      const baseImageUrl = `data:${photo.type || 'image/jpeg'};base64,${imageBase64}`;
      console.log('[RFID Simulator] validate-photo request', { fileName: photo.name });
      const response = await firstValueFrom(
        this.transactionsService.validatePhotoWithRoboflow({
          imageBase64,
          fileName: photo.name,
        }),
      );
      const result = await this.roboflowValidationService.analyzeValidation(
        response,
        baseImageUrl,
      );
      const outputImage = result.validatedImage ?? result.outputImage;
      if (outputImage) {
        panel.previewUrl = outputImage;
        panel.validatedFile = await this.dataUrlToFile(outputImage, photo.name);
      }

      panel.validationStatus = outputImage && result.isSuccessful ? 'success' : 'failed';
      this.changeDetectorRef.detectChanges();
    } catch (error) {

      panel.validationStatus = 'failed';
      this.changeDetectorRef.detectChanges();
    }
  }

  private async dataUrlToFile(dataUrl: string, fileName: string): Promise<File | null> {
    if (!dataUrl.startsWith('data:')) {
      return null;
    }
    const response = await fetch(dataUrl);
    const blob = await response.blob();
    const extension = blob.type.includes('png') ? 'png' : 'jpg';
    const baseName = fileName.replace(/\.[^.]+$/, '');
    return new File([blob], `${baseName}-validated.${extension}`, { type: blob.type });
  }

  private fileToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => {
        const result = typeof reader.result === 'string' ? reader.result : '';
        const base64 = result.includes(',') ? result.split(',')[1] : result;
        if (base64) {
          resolve(base64);
          return;
        }
        reject(new Error('Unable to convert file to base64'));
      };
      reader.onerror = () => reject(reader.error ?? new Error('FileReader error'));
      reader.readAsDataURL(file);
    });
  }

  private getCurrentStepBaseEvent(): { id: number; name: string } {
    if (this.activeStep === 'pre-gate') {
      return TRANSACTION_EVENTS.RFID_PRE_GATE_VALIDATION;
    }
    return TRANSACTION_EVENTS.RFID_GATE_IN;
  }

  private async registerStepFailureTransaction(failureIndexes: number[], timestamp = new Date()): Promise<void> {
    const now = timestamp.toISOString();
    const containerId = this.containerId.trim().toUpperCase();
    const event = TRANSACTION_EVENTS.RFID_LABEL_NOT_READ;

    const xmlDetails = this.buildStepFailureXmlDetails(event, failureIndexes);
    const payload = {
      eventId: event.id,
      entityType: containerId,
      entityId: this.getResolvedEntityId(),
      recordDate: now,
      eventDate: now,
      xmlDetails,
      details: `${event.name} for ${containerId}`,
      document: this.getCurrentStepDocumentLabel(),
      status: 1,
      eta: now,
      ship: 'RFID Simulator',
      categoryId: 1,
      locationStatus: 'Validated',
      isReefer: false,
    };

    console.log('[RFID Simulator] register transaction request', payload)

    try {
      await firstValueFrom(this.transactionsService.registerTransaction(payload));
      await this.sendAlertMailForTransaction(xmlDetails, event);
    } catch (err) {
      console.error('[RFID Simulator] register step failure transaction error', err);
    }
  }

  private async registerValidationFailureTransaction(
    failedPanels: EvidencePanel[],
    timestamp = new Date(),
  ): Promise<void> {
    if (failedPanels.length <= 0) {
      return;
    }

    const now = timestamp.toISOString();
    const containerId = this.containerId.trim().toUpperCase();
    const event = TRANSACTION_EVENTS.RFID_LABEL_PLACED_FOLLOWING_PROCEDURE;

    const failedPhotos = failedPanels
      .map((panel) => panel.validatedFile ?? panel.file)
      .filter((file): file is File => Boolean(file));

    const xmlDetails = this.buildValidationFailureXmlDetails(event, failedPanels);
    const payload = {
      eventId: event.id,
      entityType: containerId,
      entityId: this.getResolvedEntityId(),
      recordDate: now,
      eventDate: now,
      xmlDetails,
      details: `Fallo de validación de fotos para ${containerId}`,
      document: this.getCurrentStepDocumentLabel(),
      status: 1,
      eta: now,
      ship: 'RFID Simulator',
      categoryId: 1,
      locationStatus: 'Validated',
      isReefer: false,
    };

    try {
      if (failedPhotos.length > 0) {
        await firstValueFrom(
          this.transactionsService.registerTransactionWithFiles({
            photos: failedPhotos,
            eventId: event.id,
            entityType: containerId,
            entityId: this.getResolvedEntityId(),
            status: 1,
            isReefer: false,
            details: payload.details,
            document: this.getCurrentStepDocumentLabel(),
            xmlDetails,
            recordDate: now,
            eventDate: now,
          }),
        );
        await this.sendAlertMailForTransaction(xmlDetails, event);
        return;
      }

      await firstValueFrom(this.transactionsService.registerTransaction(payload));
      await this.sendAlertMailForTransaction(xmlDetails, event);
    } catch (err) {
      console.error('[RFID Simulator] register validation failure transaction error', err);
    }
  }

  private async registerStepEventTransaction(timestamp = new Date()): Promise<void> {
    const containerId = this.containerId.trim().toUpperCase();
    if (!containerId) {
      return;
    }

    if (this.isLoadingStep) {
      // Comentario solicitado: L1 y L2 no envían embarque, solo alertas de fallo cuando aplique.
      return;
    }

    const now = timestamp.toISOString();
    const event = this.getCurrentStepBaseEvent();

    const payload = {
      eventId: event.id,
      entityType: containerId,
      entityId: this.getResolvedEntityId(),
      recordDate: now,
      eventDate: now,
      xmlDetails: this.buildContainerXmlDetails(event),
      details: `${event.name} for ${containerId}`,
      document: this.getCurrentStepDocumentLabel(),
      status: 1,
      eta: now,
      ship: 'RFID Simulator',
      categoryId: 1,
      locationStatus: 'Validated',
      isReefer: false,
    };

    try {
      await firstValueFrom(this.transactionsService.registerTransaction(payload));
    } catch (err) {
      console.error('[RFID Simulator] register step transaction error', err);
    }
  }

  private getCurrentStepDocumentLabel(): string {
    if (this.activeStep === 'pre-gate') {
      return 'PRE-GATE';
    }
    if (this.activeStep === 'gate-in') {
      return 'GATE IN';
    }
    if (this.activeStep === 'loading-north') {
      return 'L1';
    }
    return 'L2';
  }

  private getCurrentContainerProfile(): ContainerXmlProfile {
    const containerId = this.containerId.trim().toUpperCase();
    return (
      this.simulatorXmlProfileByContainer[containerId] ?? {
        size: '40HC',
        shippingLine: 'MSC',
        originPort: 'ECGYE',
        loadingPort: 'ECGYE',
        dischargePort: 'USMIA',
        destinationPort: 'USMIA',
        goods: 'Carga general',
        booking: 'BK2025-009999',
        originWeight: 20000,
        terminalEntryDate: new Date().toISOString(),
        preNoticeDate: new Date().toISOString(),
        carrier: 'TRANSPORTISTA GENERICO',
        plate: 'ABC999',
        driver: 'Conductor',
        customsSeal: 'SAP999',
        observations: 'Datos de respaldo del simulador',
      }
    );
  }

  private buildContainerXmlDetails(event: { id: number; name: string }): string {
    const containerId = this.containerId.trim().toUpperCase();
    const now = new Date().toISOString();
    const profile = this.getCurrentContainerProfile();
    const seals = this.seals.map((seal, index) => (seal.trim() ? seal.trim().toUpperCase() : `SLL${index + 1}X`));

    return [
      '<Contenedor>',
      `<event_id>${event.id}</event_id>`,
      `<event_name>${event.name}</event_name>`,
      `<document>${this.getCurrentStepDocumentLabel()}</document>`,
      `<entity_id>${this.getResolvedEntityId()}</entity_id>`,
      `<contenedor>${containerId}</contenedor>`,
      `<tamaño>${profile.size}</tamaño>`,
      `<naviera>${profile.shippingLine}</naviera>`,
      `<puerto_origen>${profile.originPort}</puerto_origen>`,
      `<puerto_carga>${profile.loadingPort}</puerto_carga>`,
      `<puerto_descarga>${profile.dischargePort}</puerto_descarga>`,
      `<puerto_destino>${profile.destinationPort}</puerto_destino>`,
      `<mercancía>${profile.goods}</mercancía>`,
      `<booking>${profile.booking}</booking>`,
      `<peso_origen>${profile.originWeight}</peso_origen>`,
      `<fecha_ingreso_terminal>${profile.terminalEntryDate}</fecha_ingreso_terminal>`,
      `<fecha_preaviso>${profile.preNoticeDate}</fecha_preaviso>`,
      `<transportista>${profile.carrier}</transportista>`,
      `<placa>${profile.plate}</placa>`,
      `<conductor>${profile.driver}</conductor>`,
      `<sello-1>${seals[0]}</sello-1>`,
      `<sello-2>${seals[1]}</sello-2>`,
      `<sello-3>${seals[2]}</sello-3>`,
      `<sello-4>${seals[3]}</sello-4>`,
      `<sello_aduana>${profile.customsSeal}</sello_aduana>`,
      `<observaciones>${profile.observations}</observaciones>`,
      `<fecha_registro>${now}</fecha_registro>`,
      '</Contenedor>',
    ].join('');
  }

  private buildValidationFailureXmlDetails(
    event: { id: number; name: string },
    failedPanels: EvidencePanel[],
  ): string {
    const now = new Date().toISOString();
    const containerId = this.containerId.trim().toUpperCase();
    const failedPhotoNames = failedPanels.map((panel) => panel.file?.name ?? panel.label).join('; ');
    const profile = this.getCurrentContainerProfile();
    const seals = this.seals.map((seal, index) => (seal.trim() ? seal.trim().toUpperCase() : `SLL${index + 1}X`));

    // Comentario solicitado: en fallos se envía la misma base de datos del contenedor para History Events.
    return [
      '<Contenedor>',
      `<event_id>${event.id}</event_id>`,
      `<event_name>${event.name}</event_name>`,
      `<document>${this.getCurrentStepDocumentLabel()}</document>`,
      `<entity_id>${this.getResolvedEntityId()}</entity_id>`,
      `<contenedor>${containerId}</contenedor>`,
      `<tamaño>${profile.size}</tamaño>`,
      `<naviera>${profile.shippingLine}</naviera>`,
      `<puerto_origen>${profile.originPort}</puerto_origen>`,
      `<puerto_carga>${profile.loadingPort}</puerto_carga>`,
      `<puerto_descarga>${profile.dischargePort}</puerto_descarga>`,
      `<puerto_destino>${profile.destinationPort}</puerto_destino>`,
      `<mercancía>${profile.goods}</mercancía>`,
      `<booking>${profile.booking}</booking>`,
      `<peso_origen>${profile.originWeight}</peso_origen>`,
      `<fecha_ingreso_terminal>${profile.terminalEntryDate}</fecha_ingreso_terminal>`,
      `<fecha_preaviso>${profile.preNoticeDate}</fecha_preaviso>`,
      `<transportista>${profile.carrier}</transportista>`,
      `<placa>${profile.plate}</placa>`,
      `<conductor>${profile.driver}</conductor>`,
      `<sello-1>${seals[0]}</sello-1>`,
      `<sello-2>${seals[1]}</sello-2>`,
      `<sello-3>${seals[2]}</sello-3>`,
      `<sello-4>${seals[3]}</sello-4>`,
      `<sello_aduana>${profile.customsSeal}</sello_aduana>`,
      `<failed_photos>${failedPanels.length}</failed_photos>`,
      `<failed_photo_names>${failedPhotoNames || 'Sin detalle'}</failed_photo_names>`,
      '<failure_type>NO LEYERON LAS FOTOS DE VALIDACION</failure_type>',
      `<fecha_registro>${now}</fecha_registro>`,
      '</Contenedor>',
    ].join('');
  }

  private buildStepFailureXmlDetails(
    event: { id: number; name: string },
    failureIndexes: number[],
  ): string {
    const failureSet = new Set(failureIndexes);
    const failedSeals = this.seals
      .map((seal, index) => (failureSet.has(index) ? seal.trim().toUpperCase() : null))
      .filter(Boolean)
      .join('; ');

    const now = new Date().toISOString();
    const containerId = this.containerId.trim().toUpperCase();
    const profile = this.getCurrentContainerProfile();
    const seals = this.seals.map((seal, index) => (seal.trim() ? seal.trim().toUpperCase() : `SLL${index + 1}X`));
    const sealNumber = failedSeals || this.failureSealNumbers || 'RFID-44B9AB7';

    // Comentario solicitado: en fallos de lectura se conserva la misma estructura de detalle que eventos exitosos.
    return [
      '<Contenedor>',
      `<event_id>${event.id}</event_id>`,
      `<event_name>${event.name}</event_name>`,
      `<document>${this.getCurrentStepDocumentLabel()}</document>`,
      `<entity_id>${this.getResolvedEntityId()}</entity_id>`,
      `<contenedor>${containerId}</contenedor>`,
      `<tamaño>${profile.size}</tamaño>`,
      `<naviera>${profile.shippingLine}</naviera>`,
      `<puerto_origen>${profile.originPort}</puerto_origen>`,
      `<puerto_carga>${profile.loadingPort}</puerto_carga>`,
      `<puerto_descarga>${profile.dischargePort}</puerto_descarga>`,
      `<puerto_destino>${profile.destinationPort}</puerto_destino>`,
      `<mercancía>${profile.goods}</mercancía>`,
      `<booking>${profile.booking}</booking>`,
      `<peso_origen>${profile.originWeight}</peso_origen>`,
      `<fecha_ingreso_terminal>${profile.terminalEntryDate}</fecha_ingreso_terminal>`,
      `<fecha_preaviso>${profile.preNoticeDate}</fecha_preaviso>`,
      `<transportista>${profile.carrier}</transportista>`,
      `<placa>${profile.plate}</placa>`,
      `<conductor>${profile.driver}</conductor>`,
      `<sello-1>${seals[0]}</sello-1>`,
      `<sello-2>${seals[1]}</sello-2>`,
      `<sello-3>${seals[2]}</sello-3>`,
      `<sello-4>${seals[3]}</sello-4>`,
      `<sello_aduana>${profile.customsSeal}</sello_aduana>`,
      `<failed_seals>${sealNumber}</failed_seals>`,
      '<failure_type>NO LEYERON LOS SELLOS</failure_type>',
      `<location>${this.locationLabel}</location>`,
      '<gps_position>-2.690502, -80.247958</gps_position>',
      '<user>READER 6, AUTOMATIC</user>',
      '<alarm>SENT TO SECURITY ADM</alarm>',
      `<fecha_registro>${now}</fecha_registro>`,
      '</Contenedor>',
    ].join('');
  }


  private async sendAlertMailForTransaction(xmlDetails: string, event: { id: number; name: string }): Promise<void> {
    const containerId = this.containerId.trim().toUpperCase();
    if (!containerId) {
      return;
    }

    try {
      await firstValueFrom(
        this.transactionsService.sendAlertMail({
          xmlDetails,
          containerId,
          eventName: event.name,
          stepLabel: this.getCurrentStepDocumentLabel(),
        }),
      );
    } catch (err) {
      console.error('[RFID Simulator] send alert mail error', err);
    }
  }

  private resetSimulatorWorkflow(): void {
    this.activeStep = 'pre-gate';
    this.completedStepIndexes.clear();
    this.readingStateByStep.clear();
    this.containerId = '';
    this.seals = ['', '', '', ''];
    this.validationState = 'idle';
    this.displayedProcessingMessages = [];
    this.accessPanels.forEach((panel) => {
      panel.previewUrl = null;
      panel.file = null;
      panel.validatedFile = null;
      panel.imageBytes = null;
      panel.validationStatus = 'idle';
    });
    this.containerImage.previewUrl = null;
    this.containerImage.file = null;
    this.containerImage.validatedFile = null;
    this.containerImage.imageBytes = null;
    this.containerImage.validationStatus = 'idle';
    this.resetReadingState();
    this.closePreview();
  }

  private async loadImageBytes(file: File, panel: EvidencePanel): Promise<void> {
    try {
      const resizedBlob = await this.resizeImage(file, 320, 0.45);
      const buffer = await resizedBlob.arrayBuffer();
      panel.imageBytes = Array.from(new Uint8Array(buffer));
    } catch (error) {
      console.error('[RFID Simulator] image resize error', error);
    }
  }

  private resizeImage(file: File, maxSize: number, quality: number): Promise<Blob> {
    return new Promise((resolve, reject) => {
      const img = new Image();
      const objectUrl = URL.createObjectURL(file);

      img.onload = () => {
        const scale = Math.min(maxSize / img.width, maxSize / img.height, 1);
        const width = Math.round(img.width * scale);
        const height = Math.round(img.height * scale);
        const canvas = document.createElement('canvas');
        canvas.width = width;
        canvas.height = height;
        const ctx = canvas.getContext('2d');
        if (!ctx) {
          URL.revokeObjectURL(objectUrl);
          reject(new Error('Canvas context unavailable'));
          return;
        }
        ctx.drawImage(img, 0, 0, width, height);
        canvas.toBlob(
          (blob) => {
            URL.revokeObjectURL(objectUrl);
            if (blob) {
              resolve(blob);
            } else {
              reject(new Error('Failed to create image blob'));
            }
          },
          'image/jpeg',
          quality,
        );
      };

      img.onerror = () => {
        URL.revokeObjectURL(objectUrl);
        reject(new Error('Failed to load image'));
      };

      img.src = objectUrl;
    });
  }

  private clearReadingTimer(): void {
    if (this.readingTimeoutId) {
      window.clearTimeout(this.readingTimeoutId);
      this.readingTimeoutId = undefined;
    }
    this.isReadingProcessing = false;
  }

  private clearProcessingTimers(): void {
    if (this.processingIntervalId) {
      window.clearInterval(this.processingIntervalId);
      this.processingIntervalId = undefined;
    }
    if (this.validationTimeoutId) {
      window.clearTimeout(this.validationTimeoutId);
      this.validationTimeoutId = undefined;
    }
  }
}
