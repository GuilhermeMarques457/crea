import { Component, inject } from '@angular/core';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface SignatureViewDialogData {
  nomeUsuario: string;
  tipoAssinante: string;
  dataAssinatura: string;
  imagemAssinatura: string;
}

@Component({
  selector: 'app-signature-view-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatIconModule],
  template: `
    <h2 mat-dialog-title class="flex items-center gap-2">
      <mat-icon class="text-green-600">verified</mat-icon>
      Assinatura de {{ data.nomeUsuario }}
    </h2>
    <mat-dialog-content>
      <p class="text-xs text-slate-500 mb-3">
        {{ data.tipoAssinante }} — {{ data.dataAssinatura }}
      </p>
      <div class="flex justify-center rounded-xl border border-slate-200 bg-white p-4">
        <img
          [src]="data.imagemAssinatura"
          alt="Assinatura"
          class="max-w-full max-h-52 object-contain"
        />
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-flat-button mat-dialog-close>Fechar</button>
    </mat-dialog-actions>
  `,
})
export class SignatureViewDialogComponent {
  data = inject<SignatureViewDialogData>(MAT_DIALOG_DATA);
  dialogRef = inject(MatDialogRef<SignatureViewDialogComponent>);
}
