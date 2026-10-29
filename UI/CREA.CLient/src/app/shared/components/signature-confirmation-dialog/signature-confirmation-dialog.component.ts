import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';

export interface SignatureConfirmationData {
  nomeAssinatura: string;
}

@Component({
  selector: 'app-signature-confirmation-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatIconModule],
  template: `
    <h2 mat-dialog-title class="flex items-center gap-2">
      <mat-icon class="text-blue-600">verified</mat-icon>
      Confirmar assinatura
    </h2>
    <mat-dialog-content>
      <p class="text-sm text-slate-600">O documento será assinado digitalmente em nome de:</p>
      <div class="my-4 rounded-xl border border-blue-100 bg-blue-50 px-5 py-4 text-center">
        <span class="text-2xl font-semibold italic text-slate-800">{{ data.nomeAssinatura }}</span>
      </div>
      <p class="text-xs text-slate-500">
        A data, a hora, o IP e o hash de validação serão registrados junto à assinatura.
      </p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancelar</button>
      <button mat-flat-button color="primary" (click)="confirmar()">
        <mat-icon>check</mat-icon> Assinar como {{ data.nomeAssinatura }}
      </button>
    </mat-dialog-actions>
  `,
})
export class SignatureConfirmationDialogComponent {
  readonly data = inject<SignatureConfirmationData>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject(MatDialogRef<SignatureConfirmationDialogComponent>);

  confirmar(): void {
    this.dialogRef.close(true);
  }
}
