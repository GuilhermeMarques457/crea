import { Component, inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';

export interface AuditoriaDetalhesData {
  dadosAntigos?: string;
  dadosNovos?: string;
}

function parseJson(value: string | undefined): Record<string, unknown> | null {
  if (!value) return null;
  try {
    return JSON.parse(value);
  } catch {
    return null;
  }
}

function formatarJson(value: string | undefined): string {
  if (!value) return '';
  try {
    return JSON.stringify(JSON.parse(value), null, 2);
  } catch {
    return value;
  }
}

function extrairDiferencas(
  antigo: Record<string, unknown> | null,
  novo: Record<string, unknown> | null,
): { antigosDiff: Record<string, unknown>; novosDiff: Record<string, unknown> } {
  const antigosDiff: Record<string, unknown> = {};
  const novosDiff: Record<string, unknown> = {};

  if (!antigo && novo) return { antigosDiff: {}, novosDiff: novo };
  if (antigo && !novo) return { antigosDiff: antigo, novosDiff: {} };
  if (!antigo || !novo) return { antigosDiff: {}, novosDiff: {} };

  const allKeys = new Set([...Object.keys(antigo), ...Object.keys(novo)]);
  for (const key of allKeys) {
    const valAntigo = JSON.stringify(antigo[key]);
    const valNovo = JSON.stringify(novo[key]);
    if (valAntigo !== valNovo) {
      if (key in antigo) antigosDiff[key] = antigo[key];
      if (key in novo) novosDiff[key] = novo[key];
    }
  }

  return { antigosDiff, novosDiff };
}

@Component({
  selector: 'app-auditoria-detalhes-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatIconModule, MatSlideToggleModule],
  templateUrl: `./auditoria-detalhes-dialog.component.html`,
})
export class AuditoriaDetalhesDialogComponent {
  private readonly data = inject<AuditoriaDetalhesData>(MAT_DIALOG_DATA);

  apenaDiferencas = signal(false);

  antigosFormatado = formatarJson(this.data.dadosAntigos);
  novosFormatado = formatarJson(this.data.dadosNovos);

  private readonly diffs = extrairDiferencas(
    parseJson(this.data.dadosAntigos),
    parseJson(this.data.dadosNovos),
  );

  antigosDiffFormatado = Object.keys(this.diffs.antigosDiff).length
    ? JSON.stringify(this.diffs.antigosDiff, null, 2)
    : '';
  novosDiffFormatado = Object.keys(this.diffs.novosDiff).length
    ? JSON.stringify(this.diffs.novosDiff, null, 2)
    : '';

  temDiferencas = !!(this.antigosDiffFormatado || this.novosDiffFormatado);
}
