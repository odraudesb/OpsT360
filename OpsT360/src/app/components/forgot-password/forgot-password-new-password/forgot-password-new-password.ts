import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

type NewPasswordControls = {
  password: FormControl<string>;
  confirmPassword: FormControl<string>;
};

@Component({
  selector: 't360-forgot-password-new-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './forgot-password-new-password.html',
})
export class ForgotPasswordNewPasswordComponent {
  @Input() isSubmitting = false;

  @Output() submitPassword = new EventEmitter<{ password: string }>();
  @Output() back = new EventEmitter<void>();

  readonly form: FormGroup<NewPasswordControls>;
  showPassword = false;
  showConfirmPassword = false;

  constructor(private readonly fb: FormBuilder) {
    this.form = this.fb.nonNullable.group<NewPasswordControls>(
      {
        password: this.fb.nonNullable.control('', {
          validators: [Validators.required, Validators.minLength(8)],
        }),
        confirmPassword: this.fb.nonNullable.control('', {
          validators: [Validators.required, Validators.minLength(8)],
        }),
      },
      { validators: [ForgotPasswordNewPasswordComponent.passwordsMatch] },
    );
  }

  onSubmit(): void {
    if (this.form.invalid || this.isSubmitting) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitPassword.emit({ password: this.form.controls.password.value });
  }

  onBack(): void {
    if (!this.isSubmitting) this.back.emit();
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPassword(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  static passwordsMatch(control: AbstractControl): ValidationErrors | null {
    const group = control as FormGroup<NewPasswordControls>;
    const p = group.controls.password.value;
    const c = group.controls.confirmPassword.value;
    if (!p || !c) return null;
    return p === c ? null : { passwordMismatch: true };
  }

  get passwordMismatch(): boolean {
    return (
      !!this.form.errors?.['passwordMismatch'] &&
      (this.form.controls.confirmPassword.touched || this.form.controls.confirmPassword.dirty)
    );
  }
}
