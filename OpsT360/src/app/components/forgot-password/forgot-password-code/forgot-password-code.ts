import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

type CodeControls = {
  code: FormControl<string>;
};

@Component({
  selector: 't360-forgot-password-code',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './forgot-password-code.html',
})
export class ForgotPasswordCodeComponent {
  @Input() isSubmitting = false;

  @Output() submitCode = new EventEmitter<string>();
  @Output() back = new EventEmitter<void>();

  readonly form: FormGroup<CodeControls>;

  constructor(private readonly fb: FormBuilder) {
    this.form = this.fb.nonNullable.group<CodeControls>({
      code: this.fb.nonNullable.control('', {
        validators: [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(6),
          Validators.pattern(/^[0-9]+$/),
        ],
      }),
    });
  }

  onSubmit(): void {
    if (this.form.invalid || this.isSubmitting) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitCode.emit(this.form.controls.code.value);
  }

  onBack(): void {
    if (!this.isSubmitting) this.back.emit();
  }

  get codeControl(): FormControl<string> {
    return this.form.controls.code;
  }
}
