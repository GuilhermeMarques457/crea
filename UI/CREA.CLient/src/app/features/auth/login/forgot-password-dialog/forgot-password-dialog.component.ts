import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef,
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';

export interface ForgotPasswordDialogData {
  email?: string;
}

@Component({
  selector: 'app-forgot-password-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
  ],
  templateUrl: './forgot-password-dialog.component.html',
})
export class ForgotPasswordDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);
  private readonly dialogRef = inject(MatDialogRef<ForgotPasswordDialogComponent>);
  private readonly data = inject<ForgotPasswordDialogData>(MAT_DIALOG_DATA);

  loading = signal(false);

  form = this.fb.nonNullable.group({
    email: [this.data.email ?? '', [Validators.required, Validators.email]],
  });

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.auth.esqueciSenha(this.form.getRawValue()).subscribe({
      next: (res) => {
        this.toast.success(res.mensagem);
        this.dialogRef.close();
      },
      error: () => {
        this.toast.error('Não foi possível enviar a solicitação. Tente novamente.');
        this.loading.set(false);
      },
    });
  }
}
