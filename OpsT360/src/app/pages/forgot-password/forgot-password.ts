import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { APP_ROUTES } from '../../core/constants/app-routes.constants';

import { ForgotPasswordEmailComponent } from '../../components/forgot-password/forgot-password-email/forgot-password-email';
import { ForgotPasswordCodeComponent } from '../../components/forgot-password/forgot-password-code/forgot-password-code';
import { ForgotPasswordNewPasswordComponent } from '../../components/forgot-password/forgot-password-new-password/forgot-password-new-password';

type Step = 'email' | 'code' | 'newPassword';

@Component({
  selector: 't360-forgot-password-page',
  standalone: true,
  imports: [
    CommonModule,
    ForgotPasswordEmailComponent,
    ForgotPasswordCodeComponent,
    ForgotPasswordNewPasswordComponent,
  ],
  templateUrl: './forgot-password.html',
})
export class ForgotPasswordComponent {
  // Reusa tus assets del login (ajusta si tus rutas son otras)
  background = 'assets/images/logoLogin.jpeg';
  icono = 'assets/images/iconoT360.png';

  isSubmitting = false;

  step: Step = 'email';
  email = '';
  code = '';

  constructor(private readonly router: Router) { }

  // Paso 1
  handleEmail(email: string): void {
    this.email = email;
    this.step = 'code';
  }

  backFromEmail(): void {
    this.router.navigateByUrl('/' + APP_ROUTES.LOGIN);
  }

  // Paso 2
  handleCode(code: string): void {
    this.code = code;
    this.step = 'newPassword';
  }

  backFromCode(): void {
    this.step = 'email';
  }

  // Paso 3
  handleNewPassword(payload: { password: string }): void {
    // Aquí lo recomendado es llamar a tu endpoint:
    // request reset password con { email: this.email, code: this.code, password: payload.password }
    // Por ahora solo regresa al login:
    this.router.navigateByUrl('/' + APP_ROUTES.LOGIN);
  }

  backFromNewPassword(): void {
    this.step = 'code';
  }
}
