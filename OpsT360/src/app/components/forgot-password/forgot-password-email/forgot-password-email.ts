import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

type EmailControls = {
  email: FormControl<string>;
};

@Component({
  selector: 't360-forgot-password-email',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './forgot-password-email.html',
})
export class ForgotPasswordEmailComponent {
  @Input() isSubmitting = false;

  @Output() submitEmail = new EventEmitter<string>();
  @Output() back = new EventEmitter<void>();

  readonly form: FormGroup<EmailControls>;

  constructor(private readonly fb: FormBuilder) {
    this.form = this.fb.nonNullable.group<EmailControls>({
      email: this.fb.nonNullable.control('', {
        validators: [Validators.required, Validators.email],
      }),
    });
  }

  onSubmit(): void {
    if (this.form.invalid || this.isSubmitting) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitEmail.emit(this.form.controls.email.value);
  }

  onBack(): void {
    if (!this.isSubmitting) this.back.emit();
  }

  get emailControl(): FormControl<string> {
    return this.form.controls.email;
  }
}
