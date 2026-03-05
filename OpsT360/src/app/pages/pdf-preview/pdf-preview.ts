import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DomSanitizer, SafeResourceUrl, SafeUrl } from '@angular/platform-browser';

type StoredPdfPayload = {
  base64?: string;
  mimeType?: string;
  fileName?: string;
};

@Component({
  selector: 't360-pdf-preview',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pdf-preview.html',
})
export class PdfPreviewComponent implements OnInit, OnDestroy {
  fileName = 'document.pdf';
  mimeType = 'application/pdf';
  isImage = false;

  loading = true;
  errorMsg = '';

  iframeSrc: SafeResourceUrl | null = null;
  imageSrc: SafeUrl | null = null;

  private objectUrl: string | null = null;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly sanitizer: DomSanitizer,
  ) { }

  ngOnInit(): void {
    const key = this.route.snapshot.queryParamMap.get('key') ?? '';
    if (!key) return this.fail('Missing key.');

    const raw = sessionStorage.getItem(key);
    if (!raw) return this.fail('Document not found (missing session storage).');

    let payload: StoredPdfPayload | null = null;
    try {
      payload = JSON.parse(raw) as StoredPdfPayload;
    } catch {
      return this.fail('Invalid document payload.');
    }

    const base64 = this.normalizeBase64(payload?.base64 ?? '');
    const mimeType = payload?.mimeType ?? 'application/pdf';
    const name = this.sanitizeFileName(payload?.fileName ?? 'document', mimeType);

    if (!base64) return this.fail('This document has no content available (missing base64).');

    this.fileName = name;
    this.mimeType = mimeType;
    this.isImage = this.isImageMimeType(mimeType);

    try {
      const bytes = this.base64ToUint8Array(base64);

      // Blob URL para ver en iframe/img (visor nativo)
      const blob = new Blob([bytes as unknown as BlobPart], { type: mimeType });
      this.objectUrl = URL.createObjectURL(blob);

      if (this.isImage) {
        this.imageSrc = this.sanitizer.bypassSecurityTrustUrl(this.objectUrl);
        this.iframeSrc = null;
      } else {
        this.iframeSrc = this.sanitizer.bypassSecurityTrustResourceUrl(this.objectUrl);
        this.imageSrc = null;
      }

      document.title = this.fileName;
      this.loading = false;
    } catch (e) {
      console.error('[PdfPreview] build blob error', e);
      this.fail('Unable to render preview.');
    }
  }

  ngOnDestroy(): void {
    if (this.objectUrl) {
      URL.revokeObjectURL(this.objectUrl);
      this.objectUrl = null;
    }
  }

  // ✅ Diskette propio: descarga con nombre correcto
  download(): void {
    if (this.loading || this.errorMsg || !this.objectUrl) return;

    const a = document.createElement('a');
    a.href = this.objectUrl;
    a.download = this.fileName; // ✅ ESTE ES EL NOMBRE DEL API
    a.rel = 'noopener';
    document.body.appendChild(a);
    a.click();
    a.remove();
  }

  private fail(message: string): void {
    this.loading = false;
    this.errorMsg = message;
    this.iframeSrc = null;
    this.imageSrc = null;
  }

  private normalizeBase64(b64: string): string {
    const s = (b64 ?? '').trim();
    const idx = s.indexOf('base64,');
    return idx >= 0 ? s.slice(idx + 'base64,'.length).trim() : s;
  }

  private base64ToUint8Array(base64: string): Uint8Array {
    const bin = atob(base64);
    const len = bin.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) bytes[i] = bin.charCodeAt(i);
    return bytes;
  }

  private sanitizeFileName(name: string, mimeType: string): string {
    const trimmed = (name ?? '').trim() || 'document';
    const safe = trimmed.replace(/[<>:"/\\|?*\u0000-\u001F]/g, '_');
    if (this.hasExtension(safe)) return safe;

    const ext = this.guessExtensionFromMime(mimeType);
    return ext ? `${safe}.${ext}` : safe;
  }

  private hasExtension(name: string): boolean {
    const base = name.split('/').pop() ?? name;
    const lastDot = base.lastIndexOf('.');
    return lastDot > 0 && lastDot < base.length - 1;
  }

  private guessExtensionFromMime(mimeType: string): string | null {
    const normalized = (mimeType ?? '').toLowerCase();
    const map: Record<string, string> = {
      'application/pdf': 'pdf',
      'image/jpeg': 'jpg',
      'image/jpg': 'jpg',
      'image/png': 'png',
      'image/gif': 'gif',
      'image/bmp': 'bmp',
      'image/webp': 'webp',
      'image/tiff': 'tiff',
    };
    return map[normalized] ?? null;
  }

  private isImageMimeType(mimeType: string): boolean {
    return (mimeType ?? '').toLowerCase().startsWith('image/');
  }
}
