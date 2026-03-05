import { CommonModule } from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { T360HeaderComponent } from '../../components/header/t360-header/t360-header';
import { clampPage, getResponsivePageSize } from '../../core/utils/pagination';
import { APP_ROUTES } from '../../core/constants/app-routes.constants';

type CompanyRow = {
  id: string;
  companyName: string;
  address: string;
  companyType: string;
  email: string;
  web: string;
  companyId: string;
  phoneNumber: string;
};

type NewCompanyForm = {
  companyName: string;
  companyId: string;
  companyType: string;
  phoneType: string;
  phoneNumber: string;
  country: string;
  region: string;
  city: string;
  industry: string;
  web: string;
  zipCode: string;
  emailDomain: string;
  isActive: boolean;
};

@Component({
  selector: 't360-companies-page',
  standalone: true,
  imports: [CommonModule, FormsModule, T360HeaderComponent],
  templateUrl: './companies.html',
})
export class CompaniesComponent {
  pageSize = getResponsivePageSize();
  currentPage = 1;
  searchTerm = '';

  showDataTransferCard = false;
  showNewCompanyModal = false;
  selectedRowForActions: CompanyRow | null = null;
  actionsCardPosition: { top: string; left: string } = { top: '0px', left: '0px' };

  newCompanyForm: NewCompanyForm = {
    companyName: '',
    companyId: '',
    companyType: 'Exporter',
    phoneType: 'Type',
    phoneNumber: '',
    country: 'United States',
    region: 'South',
    city: 'Miami',
    industry: 'Retail',
    web: '',
    zipCode: '',
    emailDomain: '',
    isActive: true,
  };

  companies: CompanyRow[] = [
    {
      id: '1',
      companyName: 'INFRAPORTUS',
      address: 'WY SE',
      companyType: 'Customer',
      email: 'info@infraportus.com',
      web: 'infraportus.com',
      companyId: '1709071382001',
      phoneNumber: '+593 412313',
    },
    {
      id: '2',
      companyName: 'ACME',
      address: 'FL NE',
      companyType: 'Vendor',
      email: 'info@acme.com',
      web: 'acme.com',
      companyId: '54555',
      phoneNumber: '+1 672983945',
    },
  ];

  get filteredCompanies(): CompanyRow[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      return this.companies;
    }

    return this.companies.filter((company) =>
      [
        company.companyName,
        company.address,
        company.companyType,
        company.email,
        company.web,
        company.companyId,
        company.phoneNumber,
      ]
        .join(' ')
        .toLowerCase()
        .includes(term),
    );
  }

  get paginatedCompanies(): CompanyRow[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredCompanies.slice(start, start + this.pageSize);
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.filteredCompanies.length / this.pageSize));
  }

  @HostListener('window:resize')
  onResize(): void {
    const nextSize = getResponsivePageSize();
    if (nextSize !== this.pageSize) {
      this.pageSize = nextSize;
    }

    this.currentPage = clampPage(this.currentPage, this.totalPages);
  }

  @HostListener('document:click')
  onDocumentClick(): void {
    this.closeRowActions();
    this.showDataTransferCard = false;
  }

  constructor(private readonly router: Router) {}

  onSearchChange(): void {
    this.currentPage = 1;
  }

  openNewCompanyModal(): void {
    this.showNewCompanyModal = true;
  }

  closeNewCompanyModal(): void {
    this.showNewCompanyModal = false;
  }

  saveNewCompany(): void {
    this.closeNewCompanyModal();
  }

  toggleDataTransferCard(event: MouseEvent): void {
    event.stopPropagation();
    this.showDataTransferCard = !this.showDataTransferCard;
  }

  closeDataTransferCard(event?: MouseEvent): void {
    event?.stopPropagation();
    this.showDataTransferCard = false;
  }


  openCompanyImportPage(event: MouseEvent): void {
    event.stopPropagation();
    this.showDataTransferCard = false;
    this.router.navigate([`/${APP_ROUTES.COMPANY_DATA_IMPORT}`]);
  }

  toggleRowActions(event: MouseEvent, row: CompanyRow): void {
    event.stopPropagation();

    if (this.selectedRowForActions?.id === row.id) {
      this.closeRowActions();
      return;
    }

    const target = event.currentTarget as HTMLElement;
    const rect = target.getBoundingClientRect();

    this.actionsCardPosition = {
      top: `${rect.bottom + 8}px`,
      left: `${Math.max(16, rect.left - 180)}px`,
    };

    this.selectedRowForActions = row;
  }

  closeRowActions(event?: MouseEvent): void {
    event?.stopPropagation();
    this.selectedRowForActions = null;
  }

  previousPage(): void {
    this.currentPage = Math.max(1, this.currentPage - 1);
  }

  nextPage(): void {
    this.currentPage = Math.min(this.totalPages, this.currentPage + 1);
  }
}
