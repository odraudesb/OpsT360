import { Injectable } from '@angular/core';

export type RoboflowPrediction = {
  x: number;
  y: number;
  width: number;
  height: number;
  class: string;
  confidence?: number;
};

type RoboflowPredictionBlock = {
  predictions?: RoboflowPrediction[];
};

type RoboflowWorkflowOutput = {
  predictions?: RoboflowPredictionBlock | RoboflowPrediction[];
  output_image?: {
    type?: string;
    value?: string;
  };
};

type RoboflowWorkflowResponse = {
  outputs?: RoboflowWorkflowOutput[];
};

type BoxTuple = [number, number, number, number];

export type ValidationBox = {
  box: BoxTuple;
  area: number;
};

export type ValidationEvidenceResult = {
  detectedLabels: ValidationBox[];
  isSuccessful: boolean;
  validatedImage: string | null;
  outputImage: string | null;
};

@Injectable({ providedIn: 'root' })
export class RoboflowValidationService {
  private readonly blueRatioThreshold = 0.3;
  private readonly areaSimilarityThreshold = 0.4;
  private readonly panelMin = 0.4;
  private readonly panelMax = 0.6;
  private readonly panelOverlapTolerance = 0.03;
  private readonly panelShrinkPx = 2;

  processResponse(response: unknown): ValidationBox[] {
    const predictions = this.extractPredictions(response);
    return predictions.map((prediction) => this.buildBox(prediction));
  }

  async isValidationSuccessful(response: unknown, baseImage?: string): Promise<boolean> {
    const result = await this.analyzeValidation(response, baseImage);
    return result.isSuccessful;
  }

  async analyzeValidation(response: unknown, baseImage?: string): Promise<ValidationEvidenceResult> {
    const normalizedBaseImage = this.normalizeBaseImage(baseImage);
    const predictions = this.extractPredictions(response);
    if (!predictions.length || !normalizedBaseImage) {
      return {
        detectedLabels: [],
        isSuccessful: false,
        validatedImage: null,
        outputImage: this.extractOutputImage(response),
      };
    }

    const imageElement = await this.loadImage(normalizedBaseImage);
    const canvas = document.createElement('canvas');
    canvas.width = imageElement.width;
    canvas.height = imageElement.height;
    const ctx = canvas.getContext('2d');
    if (!ctx) {
      return {
        detectedLabels: [],
        isSuccessful: false,
        validatedImage: null,
        outputImage: this.extractOutputImage(response),
      };
    }
    ctx.drawImage(imageElement, 0, 0);
    const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);

    const validLabelBoxes = predictions
      .filter((prediction) => prediction.class === 'stickerOK')
      .map((prediction) => this.buildBox(prediction))
      .filter((box) => this.isBlueLabel(imageData, box.box));

    const finalBoxes = this.reduceLabelBoxes(validLabelBoxes);
    const accessPanels = predictions
      .filter((prediction) => prediction.class === 'access_panel')
      .map((prediction) => ({
        box: this.buildBox(prediction).box,
        confidence: prediction.confidence ?? 0,
      }));
    const mainPanel = accessPanels.length
      ? accessPanels.reduce((best, current) =>
        current.confidence > best.confidence ? current : best,
      )
      : null;

    if (!mainPanel || !finalBoxes.length) {
      return {
        detectedLabels: finalBoxes,
        isSuccessful: false,
        validatedImage: await this.buildValidatedImage(response, normalizedBaseImage),
        outputImage: this.extractOutputImage(response),
      };
    }

    const panelBox = this.shrinkBox(mainPanel.box, this.panelShrinkPx);
    const lowerBound = this.panelMin - this.panelOverlapTolerance;
    const upperBound = this.panelMax + this.panelOverlapTolerance;

    const isSuccessful = finalBoxes.every((box) => {
      const panelOverlap = this.intersectionArea(box.box, panelBox);
      const panelRatio = box.area ? panelOverlap / box.area : 0;
      return panelRatio >= lowerBound && panelRatio <= upperBound;
    });

    return {
      detectedLabels: finalBoxes,
      isSuccessful,
      validatedImage: await this.buildValidatedImage(response, normalizedBaseImage),
      outputImage: this.extractOutputImage(response),
    };
  }

  extractOutputImage(response: unknown): string | null {
    if (!response || typeof response !== 'object') {
      return null;
    }
    const responseObject = response as RoboflowWorkflowResponse;
    const outputs = responseObject.outputs ?? (Array.isArray(response) ? response : [response]);
    const outputImage = this.findOutputImage(outputs);
    if (!outputImage?.value) {
      return null;
    }
    return this.normalizeOutputImage(outputImage);
  }

  async buildValidatedImage(response: unknown, baseImage?: string): Promise<string | null> {
    const outputImage = this.extractOutputImage(response);
    const normalizedBaseImage = this.normalizeBaseImage(baseImage) ?? outputImage;
    if (!normalizedBaseImage) {
      return null;
    }

    const predictions = this.extractPredictions(response);
    if (!predictions.length) {
      return normalizedBaseImage;
    }

    const image = await this.loadImage(normalizedBaseImage);
    const canvas = document.createElement('canvas');
    canvas.width = image.width;
    canvas.height = image.height;
    const ctx = canvas.getContext('2d');
    if (!ctx) {
      return outputImage;
    }
    ctx.drawImage(image, 0, 0);
    const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);

    const validLabelBoxes = predictions
      .filter((prediction) => prediction.class === 'stickerOK')
      .map((prediction) => this.buildBox(prediction))
      .filter((box) => this.isBlueLabel(imageData, box.box));

    const finalBoxes = this.reduceLabelBoxes(validLabelBoxes);
    const accessPanels = predictions
      .filter((prediction) => prediction.class === 'access_panel')
      .map((prediction) => ({
        box: this.buildBox(prediction).box,
        confidence: prediction.confidence ?? 0,
      }));
    const mainPanel = accessPanels.length
      ? accessPanels.reduce((best, current) =>
        current.confidence > best.confidence ? current : best,
      )
      : null;

    if (mainPanel) {
      for (const panel of accessPanels) {
        this.drawBox(ctx, panel.box, '#2563eb', 'access_panel', panel.box[0], panel.box[1]);
      }
    }

    if (mainPanel && finalBoxes.length) {
      const panelBox = this.shrinkBox(mainPanel.box, this.panelShrinkPx);
      const lowerBound = this.panelMin - this.panelOverlapTolerance;
      const upperBound = this.panelMax + this.panelOverlapTolerance;

      for (const box of finalBoxes) {
        const panelOverlap = this.intersectionArea(box.box, panelBox);
        const panelRatio = box.area ? panelOverlap / box.area : 0;
        const isValid = panelRatio >= lowerBound && panelRatio <= upperBound;
        const label = `${isValid ? 'OK' : 'BAD'} (${Math.round(panelRatio * 100)}% panel)`;
        this.drawBox(
          ctx,
          box.box,
          isValid ? '#22c55e' : '#ef4444',
          `stickerOK ${label}`,
          box.box[0],
          box.box[3] + 20,
        );
      }
    }

    return canvas.toDataURL('image/jpeg', 0.92);
  }

  private findOutputImage(outputs: unknown[]): { type?: string; value?: string } | null {
    for (const output of outputs) {
      const candidate = output as RoboflowWorkflowOutput;
      if (candidate?.output_image?.value) {
        return candidate.output_image;
      }
    }
    return null;
  }

  private normalizeOutputImage(outputImage: { type?: string; value?: string }): string | null {
    if (!outputImage.value) {
      return null;
    }
    if (outputImage.value.startsWith('data:')) {
      return outputImage.value;
    }
    if (outputImage.type === 'base64' || this.looksLikeBase64(outputImage.value)) {
      return `data:image/jpeg;base64,${outputImage.value}`;
    }
    return outputImage.value;
  }

  private normalizeBaseImage(baseImage?: string): string | null {
    if (!baseImage) {
      return null;
    }
    if (baseImage.startsWith('data:')) {
      return baseImage;
    }
    if (this.looksLikeBase64(baseImage)) {
      return `data:image/jpeg;base64,${baseImage}`;
    }
    return baseImage;
  }

  private looksLikeBase64(value: string): boolean {
    if (!value || value.length < 32) {
      return false;
    }
    return /^[A-Za-z0-9+/]+={0,2}$/.test(value);
  }

  private extractPredictions(response: unknown): RoboflowPrediction[] {
    if (!response || typeof response !== 'object') {
      return [];
    }
    const responseObject = response as RoboflowWorkflowResponse;
    const outputs = responseObject.outputs ?? (Array.isArray(response) ? response : [response]);
    for (const output of outputs) {
      const candidate = output as RoboflowWorkflowOutput;
      const predictionsBlock = candidate?.predictions;

      if (Array.isArray(predictionsBlock)) {
        return predictionsBlock;
      }

      if (predictionsBlock && typeof predictionsBlock === 'object') {
        return (predictionsBlock as RoboflowPredictionBlock).predictions ?? [];
      }
    }

    return [];
  }

  private buildBox(prediction: RoboflowPrediction): ValidationBox {
    const x1 = Math.round(prediction.x - prediction.width / 2);
    const y1 = Math.round(prediction.y - prediction.height / 2);
    const x2 = Math.round(prediction.x + prediction.width / 2);
    const y2 = Math.round(prediction.y + prediction.height / 2);

    return {
      box: [x1, y1, x2, y2],
      area: Math.max(0, x2 - x1) * Math.max(0, y2 - y1),
    };
  }

  private reduceLabelBoxes(validBoxes: ValidationBox[]): ValidationBox[] {
    if (validBoxes.length <= 1) {
      return validBoxes;
    }
    if (validBoxes.length === 2) {
      const [first, second] = validBoxes;
      const minArea = Math.min(first.area, second.area);
      const maxArea = Math.max(first.area, second.area);
      if (minArea / maxArea >= this.areaSimilarityThreshold) {
        return validBoxes;
      }
      return [first.area >= second.area ? first : second];
    }

    return [...validBoxes].sort((a, b) => b.area - a.area).slice(0, 2);
  }

  private isBlueLabel(imageData: ImageData, box: BoxTuple): boolean {
    const [x1, y1, x2, y2] = box;
    const startX = Math.max(0, x1);
    const startY = Math.max(0, y1);
    const endX = Math.min(imageData.width, x2);
    const endY = Math.min(imageData.height, y2);

    if (endX <= startX || endY <= startY) {
      return false;
    }

    let bluePixels = 0;
    let totalPixels = 0;

    const data = imageData.data;

    for (let y = startY; y < endY; y += 1) {
      for (let x = startX; x < endX; x += 1) {
        const idx = (y * imageData.width + x) * 4;
        const r = data[idx];
        const g = data[idx + 1];
        const b = data[idx + 2];

        const { h, s, v } = this.rgbToHsv(r, g, b);
        const hueOpenCv = h / 2;

        totalPixels += 1;
        // Python reference uses OpenCV HSV scale (H: 0-179), so 95-140 in cv2 == 190-280 in 0-360.
        if (hueOpenCv >= 95 && hueOpenCv <= 140 && s >= 0.235 && v >= 0.196) {
          bluePixels += 1;
        }
      }
    }

    if (!totalPixels) {
      return false;
    }

    return bluePixels / totalPixels >= this.blueRatioThreshold;
  }

  private rgbToHsv(r: number, g: number, b: number): { h: number; s: number; v: number } {
    const rNorm = r / 255;
    const gNorm = g / 255;
    const bNorm = b / 255;

    const max = Math.max(rNorm, gNorm, bNorm);
    const min = Math.min(rNorm, gNorm, bNorm);
    const delta = max - min;

    let h = 0;

    if (delta !== 0) {
      if (max === rNorm) {
        h = ((gNorm - bNorm) / delta) % 6;
      } else if (max === gNorm) {
        h = (bNorm - rNorm) / delta + 2;
      } else {
        h = (rNorm - gNorm) / delta + 4;
      }

      h *= 60;
      if (h < 0) {
        h += 360;
      }
    }

    const s = max === 0 ? 0 : delta / max;
    const v = max;

    return { h, s, v };
  }

  private intersectionArea(boxA: BoxTuple, boxB: BoxTuple): number {
    const x1 = Math.max(boxA[0], boxB[0]);
    const y1 = Math.max(boxA[1], boxB[1]);
    const x2 = Math.min(boxA[2], boxB[2]);
    const y2 = Math.min(boxA[3], boxB[3]);
    if (x2 <= x1 || y2 <= y1) {
      return 0;
    }
    return (x2 - x1) * (y2 - y1);
  }
  private shrinkBox(box: BoxTuple, pixels: number): BoxTuple {
    return [
      box[0] + pixels,
      box[1] + pixels,
      box[2] - pixels,
      box[3] - pixels,
    ];
  }

  private drawBox(
    ctx: CanvasRenderingContext2D,
    box: BoxTuple,
    color: string,
    label: string,
    labelX: number,
    labelY: number,
  ): void {
    const [x1, y1, x2, y2] = box;
    ctx.strokeStyle = color;
    ctx.lineWidth = 8;
    ctx.strokeRect(x1, y1, x2 - x1, y2 - y1);
    ctx.font = '16px sans-serif';
    ctx.fillStyle = color;
    ctx.fillText(label, labelX, Math.max(labelY, 20));
  }

  private loadImage(src: string): Promise<HTMLImageElement> {
    return new Promise((resolve, reject) => {
      const img = new Image();
      img.onload = () => resolve(img);
      img.onerror = () => reject(new Error('Failed to load image'));
      img.src = src;
    });
  }

}
