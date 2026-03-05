// src/app/pages/login/login.ts
import { CommonModule } from '@angular/common';
import { Component, Input, inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';

import { APP_ROUTES } from '../../core/constants/app-routes.constants';
import { DEFAULT_DEVICE_INFO } from '../../core/constants/device-info';
import { extractTokenFromLoginResponse, saveAuthToken } from '../../core/utils/auth-storage';
import { AuthService } from '../../services/auth/auth.service';
import { LoginRequest } from '../../interfaces/auth/login-request.interface';

type LoginFormControls = {
  email: FormControl<string>;
  password: FormControl<string>;
};

@Component({
  selector: 't360-login-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, TranslateModule],
  templateUrl: './login.html',
})
export class LoginComponent {
  protected readonly APP_ROUTES = APP_ROUTES;

  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly translate = inject(TranslateService);

  readonly loginForm: FormGroup<LoginFormControls> = this.fb.nonNullable.group<LoginFormControls>({
    email: this.fb.nonNullable.control('', Validators.required),
    password: this.fb.nonNullable.control('', Validators.required),
  });

  @Input() logo = 'assets/images/logo.png';
  @Input() logoT360 = 'assets/images/logo-360.png';
  @Input() icono = 'assets/images/iconoT360.png';
  @Input() background = 'assets/images/logoLogin.jpeg';

  isSubmitting = false;
  loginErrorMessage: string | null = null;

  showPassword = false;
  currentLang: 'es' | 'en' = 'es';

  constructor() {
    const saved = (localStorage.getItem('t360_lang') as 'es' | 'en') || 'es';
    this.currentLang = saved;

    this.translate.setDefaultLang('es');
    this.translate.use(saved);
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  toggleLanguage(): void {
    this.currentLang = this.currentLang === 'es' ? 'en' : 'es';
    this.translate.use(this.currentLang);
    localStorage.setItem('t360_lang', this.currentLang);
  }

  async handleSubmit(): Promise<void> {
    if (this.loginForm.invalid || this.isSubmitting) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.loginErrorMessage = null;

    const { email, password } = this.loginForm.getRawValue();

    const payload: LoginRequest = {
      username: email, // aquí viaja "admin" o el usuario nuevo
      password,
      ip: DEFAULT_DEVICE_INFO.ip,
      device: DEFAULT_DEVICE_INFO.device,
    };

    // ✅ opcional: limpia token viejo antes de iniciar sesión (ya no debería ser necesario con el interceptor)
    // this.authService.logout();

    try {
      const response = await firstValueFrom(this.authService.login(payload));
      const token = extractTokenFromLoginResponse(response);
      if (!token) {
        this.loginErrorMessage = this.translate.instant('LOGIN.INVALID_CREDENTIALS');
        return;
      }

      saveAuthToken(token);
      await this.router.navigateByUrl('/' + APP_ROUTES.HOME);
    } catch {
      this.loginErrorMessage = this.translate.instant('LOGIN.INVALID_CREDENTIALS');
    } finally {
      this.isSubmitting = false;
    }
  }

  get emailControl(): FormControl<string> {
    return this.loginForm.controls.email;
  }

  get passwordControl(): FormControl<string> {
    return this.loginForm.controls.password;
  }
}
