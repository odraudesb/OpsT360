import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 't360-scroll-top-button',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './scroll-top-button.component.html',
})
export class ScrollTopButtonComponent {
  @Input() show = false;
  @Output() scrollTop = new EventEmitter<void>();

  onClick(): void {
    this.scrollTop.emit();
  }
}
