import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

import { APP_ROUTES } from '../../core/constants/app-routes.constants';
import { T360LauncherComponent } from '../../components/launcher/t360-launcher/t360-launcher';
import { AlertDetailComponent } from './components/alert-detail/alert-detail.component';
import { CargoSecurityControlComponent } from './components/cargo-security-control/cargo-security-control.component';
import { HistoryEventsComponent } from './components/history-events/history-events.component';
import { ItemsModalComponent } from './components/items-modal/items-modal.component';
import { ReeferTraceComponent } from './components/reefer-trace/reefer-trace.component';
import { ScrollTopButtonComponent } from './components/scroll-top-button/scroll-top-button.component';
import { ShipmentDocumentationComponent } from './components/shipment-documentation/shipment-documentation.component';
import { TrackAndTraceSectionComponent } from './components/track-and-trace-section/track-and-trace-section.component';
import type { ContainerDetailTab } from './container-detail-tab.type';
import type { ItemsModalPayload } from './items-modal-payload.type';

@Component({
  selector: 't360-container-detail-page',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    T360LauncherComponent,
    AlertDetailComponent,
    TrackAndTraceSectionComponent,
    ShipmentDocumentationComponent,
    CargoSecurityControlComponent,
    ReeferTraceComponent,
    HistoryEventsComponent,
    ItemsModalComponent,
    ScrollTopButtonComponent,
  ],
  templateUrl: './container-detail.html',
})
export class ContainerDetailComponent implements OnInit, AfterViewInit, OnDestroy {
  readonly containersListRoute = ['/', APP_ROUTES.CONTAINERS_EXPORTATION];

  // Header
  sectionLabel = 'Container Id:';
  sectionLink = '';
  showNotificationDot = true;
  isLauncherOpen = false;

  // Search Ctrl+F style
  searchTerm = '';
  searchCount = 0;
  activeMatchIndex = 0;
  private searchMarks: HTMLElement[] = [];

  private readonly hitBaseClasses = ['t360-search-hit', 'bg-amber-200', 'px-0.5', 'rounded'];
  private readonly hitActiveAdd = ['bg-amber-500', 'text-slate-900', 'ring-2', 'ring-blue-400'];

  containerId = '';
  activeTab: ContainerDetailTab = 'trackAndTrace';
  hasAlert = false;

  // ✅ focus para History Events (viene desde Last Event)
  focusTxId: number | null = null;

  isItemsModalOpen = false;
  itemsModalTitle = '';
  itemsModalColumns: string[] = [];
  itemsModalRows: Record<string, unknown>[] = [];

  entityId: number | null = null;
  initialTxId: number | null = null;

  showScrollTop = false;
  private viewReady = false;
  private pendingScrollId: string | null = null;
  private pendingTab: ContainerDetailTab | null = null;
  private hasAutoScrolled = false;
  private autoScrollAttempts = 0;
  private autoScrollTimer: ReturnType<typeof setTimeout> | null = null;
  private hasAutoTabClick = false;

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
  ) { }

  ngOnInit(): void {
    this.containerId = this.route.snapshot.paramMap.get('containerId') ?? '';
    this.sectionLink = this.containerId;

    this.initialTxId = Number(this.route.snapshot.queryParamMap.get('tx') ?? 0) || null;

    const rawEntityId = Number(this.route.snapshot.queryParamMap.get('entityId') ?? 0);
    this.entityId = Number.isFinite(rawEntityId) && rawEntityId > 0 ? rawEntityId : null;

    this.route.queryParamMap.pipe(takeUntil(this.destroy$)).subscribe((qp) => {
      const tab = qp.get('tab') as ContainerDetailTab | null;
      const section = qp.get('section');
      const search = qp.get('search');
      this.hasAlert = this.isAlertParam(qp.get('alert')) || this.isAlertStatus(qp.get('status'));

      // ✅ focusTx (para abrir+scrollear dentro de History Events)
      const ft = Number(qp.get('focusTx') ?? 0);
      this.focusTxId = Number.isFinite(ft) && ft > 0 ? ft : null;

      if (tab && this.isValidTab(tab)) {
        this.activeTab = tab;
        this.pendingTab = tab;
        this.hasAutoTabClick = false;
      }

      if (search && search.trim()) {
        this.searchTerm = search.trim();
        this.searchCount = 0;
        this.activeMatchIndex = 0;
        if (this.viewReady) {
          setTimeout(() => this.applySearchHighlights(), 0);
        }
      } else if (this.searchTerm) {
        this.clearSearch();
      }

      if (section && section.trim()) {
        this.pendingScrollId = section.trim();
        this.hasAutoScrolled = false;
        this.autoScrollAttempts = 0;
        if (this.autoScrollTimer) {
          clearTimeout(this.autoScrollTimer);
          this.autoScrollTimer = null;
        }
        if (this.viewReady) {
          if (this.pendingTab) {
            const tabToClick = this.pendingTab;
            const sectionId = this.pendingScrollId ?? undefined;
            this.onTabClick(tabToClick, sectionId);
            this.pendingTab = null;

            if (!this.hasAutoTabClick && sectionId) {
              this.hasAutoTabClick = true;
              setTimeout(() => {
                this.onTabClick(tabToClick, sectionId);
              }, 200);
            }
          } else {
            this.attemptAutoScroll();
          }
        }
      }
    });

    window.addEventListener('scroll', this.onScroll, { passive: true });
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
    if (this.pendingScrollId) {
      if (this.pendingTab) {
        const tab = this.pendingTab;
        const sectionId = this.pendingScrollId;
        this.onTabClick(tab, sectionId);
        this.pendingTab = null;

        if (!this.hasAutoTabClick) {
          this.hasAutoTabClick = true;
          setTimeout(() => {
            this.onTabClick(tab, sectionId);
          }, 200);
        }
      } else {
        this.attemptAutoScroll();
      }
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.clearHighlights();
    if (this.autoScrollTimer) clearTimeout(this.autoScrollTimer);
    window.removeEventListener('scroll', this.onScroll);
  }

  private onScroll = (): void => {
    this.showScrollTop = window.scrollY > 450;
  };

  // Launcher
  toggleLauncher(): void {
    this.isLauncherOpen = !this.isLauncherOpen;
  }

  closeLauncher(): void {
    this.isLauncherOpen = false;
  }

  openAlertDetail(): void {
    if (!this.hasAlert) return;
    this.onTabClick('alertDetail', 'alert-detail-title');
  }

  // Search
  onSearchInput(ev: Event): void {
    const value = (ev.target as HTMLInputElement | null)?.value ?? '';
    this.searchTerm = value;
    this.applySearchHighlights();
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.searchCount = 0;
    this.activeMatchIndex = 0;
    this.clearHighlights();
  }

  focusNextMatch(): void {
    if (!this.searchMarks.length) return;
    this.activeMatchIndex = (this.activeMatchIndex + 1) % this.searchMarks.length;
    this.focusMatch(this.activeMatchIndex);
  }

  onEntityIdResolved(id: number): void {
    this.entityId = id;
  }

  onContentChanged(): void {
    if (this.searchTerm.trim()) setTimeout(() => this.applySearchHighlights(), 0);
    if (this.pendingScrollId && !this.hasAutoScrolled) {
      setTimeout(() => this.attemptAutoScroll(), 0);
    }
  }

  private isAlertParam(value: string | null): boolean {
    return value === '1' || value === 'true';
  }

  private isAlertStatus(value: string | null): boolean {
    return (value ?? '').toLowerCase() === 'alert';
  }

  private focusMatch(index: number): void {
    for (const m of this.searchMarks) {
      m.classList.remove(...this.hitActiveAdd);
      if (!m.classList.contains('bg-amber-200')) m.classList.add('bg-amber-200');
    }

    const el = this.searchMarks[index];
    if (!el) return;

    el.classList.remove('bg-amber-200');
    el.classList.add(...this.hitActiveAdd);

    el.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  private applySearchHighlights(): void {
    this.clearHighlights();

    const term = (this.searchTerm || '').trim();
    if (!term) {
      this.searchCount = 0;
      this.activeMatchIndex = 0;
      return;
    }

    const root = document.getElementById('container-detail-content');
    if (!root) return;

    const escaped = term.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    const re = new RegExp(escaped, 'gi');

    const nodes: Text[] = [];
    const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT, {
      acceptNode: (node) => {
        const text = node.nodeValue ?? '';
        if (!text.trim()) return NodeFilter.FILTER_REJECT;

        const parent = (node as Text).parentElement;
        if (!parent) return NodeFilter.FILTER_REJECT;

        if (parent.closest('mark.t360-search-hit')) return NodeFilter.FILTER_REJECT;
        if (parent.closest('[data-search-ignore="true"]')) return NodeFilter.FILTER_REJECT;
        if (parent.tagName === 'SCRIPT' || parent.tagName === 'STYLE') return NodeFilter.FILTER_REJECT;

        return NodeFilter.FILTER_ACCEPT;
      },
    });

    let current: Node | null;
    while ((current = walker.nextNode())) nodes.push(current as Text);

    for (const textNode of nodes) {
      const text = textNode.nodeValue ?? '';
      re.lastIndex = 0;
      if (!re.test(text)) continue;
      re.lastIndex = 0;

      const frag = document.createDocumentFragment();
      let last = 0;

      let match: RegExpExecArray | null;
      while ((match = re.exec(text))) {
        const start = match.index;
        const end = start + match[0].length;

        if (start > last) frag.appendChild(document.createTextNode(text.slice(last, start)));

        const mark = document.createElement('mark');
        mark.className = this.hitBaseClasses.join(' ');
        mark.textContent = text.slice(start, end);
        frag.appendChild(mark);

        last = end;
      }

      if (last < text.length) frag.appendChild(document.createTextNode(text.slice(last)));
      textNode.parentNode?.replaceChild(frag, textNode);
    }

    this.searchMarks = Array.from(root.querySelectorAll('mark.t360-search-hit')) as HTMLElement[];
    this.searchCount = this.searchMarks.length;

    if (this.searchCount) {
      this.activeMatchIndex = 0;
      this.focusMatch(0);
    }
  }

  private clearHighlights(): void {
    const root = document.getElementById('container-detail-content');
    if (!root) return;

    const marks = Array.from(root.querySelectorAll('mark.t360-search-hit'));
    for (const m of marks) {
      const text = document.createTextNode(m.textContent ?? '');
      m.parentNode?.replaceChild(text, m);
    }
    root.normalize();
    this.searchMarks = [];
  }

  scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  onTabClick(tab: ContainerDetailTab, sectionId?: string): void {
    this.activeTab = tab;
    if (!sectionId) return;

    this.pendingScrollId = sectionId;
    this.hasAutoScrolled = false;
    this.autoScrollAttempts = 0;
    if (this.autoScrollTimer) {
      clearTimeout(this.autoScrollTimer);
      this.autoScrollTimer = null;
    }

    if (this.viewReady) {
      this.attemptAutoScroll();
    }
  }

  private isValidTab(tab: string): tab is ContainerDetailTab {
    return (
      tab === 'trackAndTrace' ||
      tab === 'shipmentDocumentation' ||
      tab === 'cargoSecurityControl' ||
      tab === 'reeferTrace' ||
      tab === 'historyEvents' ||
      tab === 'alertDetail'
    );
  }

  // ✅ Scroll robusto: si llega "-section" o "-title", prueba ambos
  private scrollToSection(id: string): boolean {
    const el = this.resolveScrollElement(id);
    if (!el) return false;
    this.scrollToElement(el);
    return true;
  }

  // ✅ Scroll robusto: si llega "-section" o "-title", prueba ambos
  private resolveScrollElement(id: string): HTMLElement | null {
    const candidates: string[] = [id];

    if (id.endsWith('-section')) {
      candidates.unshift(id.replace(/-section$/, '-title'));
    } else if (id.endsWith('-title')) {
      candidates.push(id.replace(/-title$/, '-section'));
    }

    return (
      candidates
        .map((cid) => document.getElementById(cid))
        .find((x): x is HTMLElement => !!x) ?? null
    );
  }

  private scrollToElement(el: HTMLElement): void {
    const headerOffset = 90;

    requestAnimationFrame(() => {
      requestAnimationFrame(() => {
        const top = el.getBoundingClientRect().top + window.scrollY - headerOffset;
        window.scrollTo({ top: Math.max(top, 0), behavior: 'smooth' });
      });
    });
  }

  private isElementInView(el: HTMLElement): boolean {
    const rect = el.getBoundingClientRect();
    const viewHeight = window.innerHeight || document.documentElement.clientHeight;
    return rect.top < viewHeight && rect.bottom > 0;
  }

  private attemptAutoScroll(): void {
    if (!this.pendingScrollId) return;
    const el = this.resolveScrollElement(this.pendingScrollId);
    if (!el) {
      this.scheduleAutoScrollRetry();
      return;
    }
    this.scrollToElement(el);
    this.scheduleAutoScrollVerify();
  }

  private scheduleAutoScrollRetry(): void {
    if (this.autoScrollAttempts >= 10 || this.hasAutoScrolled) return;
    this.autoScrollAttempts += 1;
    if (this.autoScrollTimer) clearTimeout(this.autoScrollTimer);
    this.autoScrollTimer = setTimeout(() => {
      this.attemptAutoScroll();
    }, 200);
  }

  private scheduleAutoScrollVerify(): void {
    if (this.autoScrollAttempts >= 10 || this.hasAutoScrolled) return;
    this.autoScrollAttempts += 1;
    if (this.autoScrollTimer) clearTimeout(this.autoScrollTimer);
    this.autoScrollTimer = setTimeout(() => {
      if (!this.pendingScrollId) return;
      const el = this.resolveScrollElement(this.pendingScrollId);
      if (!el) {
        this.scheduleAutoScrollRetry();
        return;
      }

      if (this.isElementInView(el)) {
        this.hasAutoScrolled = true;
        return;
      }

      this.scrollToElement(el);
      this.scheduleAutoScrollVerify();
    }, 250);
  }

  // Items modal
  openItemsModal(payload: ItemsModalPayload): void {
    this.itemsModalTitle = payload.title;

    const items = Array.isArray(payload.items) ? payload.items : [];
    const rows: Record<string, unknown>[] = items.map((r) =>
      this.isRecord(r) ? (r as Record<string, unknown>) : { Value: r },
    );
    this.itemsModalRows = rows;

    const cols: string[] = [];
    for (const row of rows) for (const k of Object.keys(row)) if (!cols.includes(k)) cols.push(k);
    this.itemsModalColumns = cols.length ? cols : ['Value'];

    this.isItemsModalOpen = true;
  }

  closeItemsModal(): void {
    this.isItemsModalOpen = false;
    this.itemsModalTitle = '';
    this.itemsModalColumns = [];
    this.itemsModalRows = [];
  }

  private isRecord(x: unknown): x is Record<string, unknown> {
    return !!x && typeof x === 'object' && !Array.isArray(x);
  }
}
