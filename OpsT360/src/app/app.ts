import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('trace360-web');

  constructor(private readonly translate: TranslateService) {
    const saved = (localStorage.getItem('t360_lang') as 'es' | 'en') || 'es';
    this.translate.setDefaultLang('es');
    this.translate.use(saved);
  }
}
