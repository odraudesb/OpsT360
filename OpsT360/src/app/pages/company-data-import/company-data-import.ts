import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { T360HeaderComponent } from '../../components/header/t360-header/t360-header';
import { APP_ROUTES } from '../../core/constants/app-routes.constants';

type ImportStep = 1 | 2 | 3 | 4;

type MappingField = {
  source: string;
  target: string;
  required?: boolean;
  mapped?: boolean;
};

@Component({
  selector: 't360-company-data-import-page',
  standalone: true,
  imports: [CommonModule, FormsModule, T360HeaderComponent],
  templateUrl: './company-data-import.html',
})
export class CompanyDataImportComponent {
  step: ImportStep = 1;
  selectedCompanyType = '';
  hasHeaders = true;
  skipDuplicates = true;
  overwriteAll = false;

  readonly steps: { id: ImportStep; label: string }[] = [
    { id: 1, label: 'Select file' },
    { id: 2, label: 'Fileds mapping' },
    { id: 3, label: 'Review & Import' },
    { id: 4, label: 'Results' },
  ];

  readonly mappingFields: MappingField[] = [
    { source: 'ID COMPANY', target: 'ID COMPANY', required: true, mapped: true },
    { source: 'COMPANY NAME', target: 'COMPANY NAME', required: true },
    { source: 'ADDRESS', target: 'ADDRESS' },
    { source: 'COMPANY TYPE', target: 'COMPANY TYPE' },
    { source: 'E-MAIL', target: 'E-MAIL' },
    { source: 'PHONE NUMBER', target: 'PHONE NUMBER' },
  ];

  readonly previewRows = [
    {
      companyName: 'INFRAPORTUS',
      address: 'WY SE',
      employees: 100,
      email: 'info@infraportus.com',
      web: 'infraportus.com',
      companyId: '1709071382001',
      phone: '+593 412313',
    },
    {
      companyName: 'ACME',
      address: 'FL NE',
      employees: 100,
      email: 'info@acme.com',
      web: 'acme.com',
      companyId: '54555',
      phone: '+1 672983945',
    },
  ];

  readonly importSummary = {
    success: 41,
    updated: 11,
    errors: 7,
    total: 59,
  };

  readonly importErrors = [
    { line: 15, company: 'INFRAPORTUS', error: 'ID COMPANY duplicated' },
    { line: 23, company: 'ACME', error: 'Formato de email inválido' },
  ];

  constructor(private readonly router: Router) {}

  nextStep(): void {
    if (this.step < 4) {
      this.step = (this.step + 1) as ImportStep;
    }
  }

  prevStep(): void {
    if (this.step > 1) {
      this.step = (this.step - 1) as ImportStep;
    }
  }

  goToStep(step: ImportStep): void {
    this.step = step;
  }

  viewCompanies(): void {
    this.router.navigate([`/${APP_ROUTES.COMPANIES}`]);
  }
}
