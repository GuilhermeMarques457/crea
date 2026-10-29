import { Component, inject } from '@angular/core';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface SignatureViewDialogData {
  nomeUsuario: string;
  tipoAssinante: string;
  dataAssinatura: string;
  imagemAssinatura?: string | null;
  hashAssinatura: string;
  ipAssinante: string;
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
      <div class="grid gap-4 rounded-xl border border-slate-200 bg-white p-4 sm:grid-cols-[1fr_1.4fr]">
        <div class="flex min-h-24 items-center justify-center border-b border-slate-100 sm:border-b-0 sm:border-r sm:pr-4">
          @if (data.imagemAssinatura) {
            <img [src]="data.imagemAssinatura" alt="Assinatura" class="max-w-full max-h-36 object-contain" />
          } @else {
            <span class="text-2xl font-semibold italic text-slate-800">{{ nomeCurto }}</span>
          }
        </div>
        <dl class="space-y-2 text-xs text-slate-600">
          <div><dt class="font-semibold">Nome</dt><dd>{{ nomeCurto }}</dd></div>
          <div><dt class="font-semibold">IP</dt><dd>{{ data.ipAssinante || 'Não disponível' }}</dd></div>
          <div><dt class="font-semibold">Hash de validação</dt><dd class="break-all font-mono">{{ data.hashAssinatura }}</dd></div>
        </dl>
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

  get nomeCurto(): string {
    const partes = this.data.nomeUsuario.trim().split(/\s+/).filter(Boolean);
    return partes.length < 2 ? partes[0] ?? '' : `${partes[0]} ${partes[partes.length - 1]}`;
  }
}
