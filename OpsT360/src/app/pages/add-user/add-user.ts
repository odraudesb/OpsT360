import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

import { CompaniesService } from '../../services/company/companies.service';
import { UsersService } from '../../services/user/users.service';

import type { ApiResponse } from '../../interfaces/api/api-response.interface';
import type { Company } from '../../interfaces/company/company.interface';
import type { CreateUserRequest } from '../../interfaces/users/create-user.interface';

type Step = 'signup' | 'sendCode' | 'verify' | 'done';

@Component({
  selector: 't360-add-user-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './add-user.html',
})
export class AddUserComponent implements OnInit {
  step: Step = 'signup';

  // ✅ para el template (img [src]="icono")
  icono = 'assets/images/iconoT360.png';
  background = 'assets/images/logoLogin.jpeg';

  // companies
  companies: Company[] = [];
  filteredCompanies: Company[] = [];
  isLoadingCompanies = false;
  companyDropdownOpen = false;
  companySearch = '';
  selectedCompany: Company | null = null;

  // forms
  readonly addUserForm: FormGroup;
  readonly verifyForm: FormGroup;

  isSubmitting = false;
  errorMessage: string | null = null;

  private pendingEmail: string | null = null;

  constructor(
    private readonly fb: FormBuilder,
    private readonly router: Router,
    private readonly usersService: UsersService,
    private readonly companiesService: CompaniesService,
  ) {
    this.addUserForm = this.fb.group(
      {
        email: ['', [Validators.required, Validators.email]],
        name: ['', [Validators.required, Validators.minLength(2)]],
        lastName: ['', [Validators.required]],
        companyId: [null, [Validators.required]],
        password: ['', [Validators.required, Validators.minLength(8)]],
        confirmPassword: ['', [Validators.required, Validators.minLength(8)]],
      },
      { validators: [this.validateCompanyDomain.bind(this)] },
    );

    this.verifyForm = this.fb.group({
      code: ['', [Validators.required, Validators.minLength(4)]],
    });
  }

  ngOnInit(): void {
    this.loadCompanies();
  }

  private loadCompanies(): void {
    this.isLoadingCompanies = true;

    this.companiesService.getCompanies().subscribe({
      next: (response: ApiResponse<Company[] | Company>) => {
        const data = response?.data;
        const companyList = Array.isArray(data) ? data : data ? [data] : [];

        this.companies = companyList;
        this.filteredCompanies = this.companies.slice(0, 10);
        this.isLoadingCompanies = false;
        this.addUserForm.updateValueAndValidity();
      },
      error: () => {
        this.isLoadingCompanies = false;
        this.errorMessage = 'Could not load companies.';
      },
    });
  }

  // --- company autocomplete ---
  onCompanyFocus(): void {
    this.companyDropdownOpen = true;
    this.filterCompanies(this.companySearch);
  }

  onCompanySearchChange(value: string): void {
    this.companySearch = value;
    this.companyDropdownOpen = true;
    this.filterCompanies(value);

    // invalida selección hasta elegir item
    this.selectedCompany = null;
    this.addUserForm.patchValue({ companyId: null });
    this.addUserForm.updateValueAndValidity();
  }

  private filterCompanies(term: string): void {
    const q = (term || '').trim().toLowerCase();
    const list = !q
      ? this.companies
      : this.companies.filter((c) => (c.name || '').toLowerCase().includes(q));

    this.filteredCompanies = list.slice(0, 10);
  }

  selectCompany(c: Company): void {
    this.addUserForm.patchValue({ companyId: c.companyId });
    this.companySearch = c.name;
    this.companyDropdownOpen = false;
    this.selectedCompany = c;
    this.addUserForm.updateValueAndValidity();
  }

  trackByCompanyId(index: number, item: Company): number {
    return item.companyId;
  }

  @HostListener('document:click', ['$event'])
  onDocClick(event: MouseEvent): void {
    const target = event.target as HTMLElement | null;
    if (!target) return;
    if (!target.closest('[data-company-autocomplete]')) {
      this.companyDropdownOpen = false;
    }
  }

  // --- FLOW ---
  handleSignup(): void {
    if (this.addUserForm.invalid || this.isSubmitting) {
      this.addUserForm.markAllAsTouched();
      return;
    }

    this.errorMessage = null;

    const raw = this.addUserForm.getRawValue();
    if (raw.password !== raw.confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    this.isSubmitting = true;

    const payload: CreateUserRequest = {
      email: raw.email,
      password: raw.password,
      name: raw.name,
      lastName: raw.lastName,
      companyId: Number(raw.companyId),
    };

    this.usersService.createUser(payload).subscribe({
      next: () => {
        this.pendingEmail = raw.email;
        this.isSubmitting = false;
        this.step = 'sendCode';
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = err?.message || 'Error creating user.';
      },
    });
  }

  // ✅ esto faltaba (tu template lo llama)
  handleSendCode(): void {
    if (!this.pendingEmail || this.isSubmitting) return;

    this.isSubmitting = true;
    this.errorMessage = null;

    // TODO: cuando exista endpoint real:
    // this.usersService.sendVerificationCode({ email: this.pendingEmail }).subscribe(...)
    setTimeout(() => {
      this.isSubmitting = false;
      this.step = 'verify';
    }, 500);
  }

  handleVerify(): void {
    if (this.verifyForm.invalid || this.isSubmitting) {
      this.verifyForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = null;

    const code = String(this.verifyForm.getRawValue().code);

    // TODO: endpoint real verify:
    // this.usersService.verifyEmail({ email: this.pendingEmail!, code }).subscribe(...)
    setTimeout(() => {
      this.isSubmitting = false;
      this.step = 'done';
    }, 600);
  }

  backToLogin(): void {
    this.router.navigate(['/login']);
  }

  private validateCompanyDomain(control: AbstractControl): ValidationErrors | null {
    const group = control as FormGroup;
    const email = String(group.get('email')?.value || '').trim();
    const companyId = Number(group.get('companyId')?.value || 0);

    if (!email || !companyId) return null;

    const emailDomain = this.extractDomain(email);
    const company =
      this.selectedCompany || this.companies.find((item) => item.companyId === companyId) || null;
    const companyDomain = this.extractDomain(company?.email || company?.webUrl || '');

    if (!emailDomain || !companyDomain) return null;
    return emailDomain === companyDomain ? null : { companyDomainMismatch: true };
  }

  private extractDomain(value: string): string | null {
    const trimmed = (value || '').trim().toLowerCase();
    if (!trimmed) return null;

    if (trimmed.includes('@')) {
      const parts = trimmed.split('@');
      return parts.length > 1 ? parts[parts.length - 1] : null;
    }

    try {
      const url = new URL(trimmed.includes('://') ? trimmed : `https://${trimmed}`);
      return url.hostname.replace(/^www\./, '');
    } catch {
      return null;
    }
  }

  // getters
  get emailControl(): FormControl {
    return this.addUserForm.get('email') as FormControl;
  }

  get companyIdControl(): FormControl {
    return this.addUserForm.get('companyId') as FormControl;
  }
}
