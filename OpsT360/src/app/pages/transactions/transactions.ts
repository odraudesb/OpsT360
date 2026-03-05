import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 't360-transactions-page',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './transactions.html',
})
export class TransactionsComponent {}
