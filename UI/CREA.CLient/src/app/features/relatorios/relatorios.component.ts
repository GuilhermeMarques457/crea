import { Component, signal, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { DatePipe } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { RelatorioService } from '../../core/services/relatorio.service';
import { ObraService } from '../../core/services/obra.service';
import { ToastService } from '../../core/services/toast.service';
import { SignatureViewDialogComponent } from '../../shared/components/signature-view-dialog/signature-view-dialog.component';
import { AssinaturaDto } from '../../shared/models/api.models';
import { labelTipoAssinante } from '../../shared/utils/assinatura.utils';
import {
  ObraDto,
  RelatorioObraDto,
  STATUS_OBRA_LABELS,
  TIPO_EDIFICACAO_LABELS,
  ATIVIDADE_TECNICA_LABELS,
  POSICAO_OBRA_LABELS,
} from '../../shared/models/api.models';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';

@Component({
  selector: 'app-relatorios',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    DatePipe,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: `./relatorios.component.html`,
})
export class RelatoriosComponent implements OnInit {
  private readonly obraService = inject(ObraService);
  private readonly relatorioService = inject(RelatorioService);
  private readonly fb = inject(FormBuilder);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);

  obras = signal<ObraDto[]>([]);
  loading = signal(false);
  pdfLoading = signal(false);
  relatorio = signal<RelatorioObraDto | null>(null);

  form = this.fb.nonNullable.group({ obraId: ['', Validators.required] });

  tipoEdificacaoLabel = () =>
    this.relatorio()?.tipoEdificacao
      ? TIPO_EDIFICACAO_LABELS[this.relatorio()!.tipoEdificacao!]
      : '–';
  atividadeTecnicaLabel = () =>
    this.relatorio()?.atividadeTecnica
      ? ATIVIDADE_TECNICA_LABELS[this.relatorio()!.atividadeTecnica!]
      : '–';

  statCards = () =>
    this.relatorio()
      ? [
          { label: 'Relato de Visita', value: this.relatorio()!.totalRelatoVisita },
          { label: 'Anexos', value: this.relatorio()!.totalAnexos },
          { label: 'Termo Conclusão', value: this.relatorio()!.possuiTermoConclusao ? '✓' : '–' },
        ]
      : [];

  ngOnInit() {
    this.obraService.listarPorPermissaoUsuario().subscribe((obras) => this.obras.set(obras));
  }

  gerar() {
    if (this.form.invalid) return;
    this.loading.set(true);
    this.relatorio.set(null);
    this.relatorioService.gerarRelatorioObra(this.form.value.obraId!).subscribe({
      next: (r) => {
        this.relatorio.set(r);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  exportarPdf() {
    const obraId = this.form.getRawValue().obraId;
    if (!obraId) return;
    this.pdfLoading.set(true);
    this.relatorioService.baixarPdfObra(obraId).subscribe({
      next: (res) => {
        const blob = res.body;
        if (!blob?.size) {
          this.toast.error('Resposta vazia do servidor ao gerar o PDF.');
          this.pdfLoading.set(false);
          return;
        }
        const cd = res.headers.get('Content-Disposition');
        let fileName = `Relatorio_${obraId.slice(0, 8)}.pdf`;
        if (cd) {
          const utf = /filename\*=UTF-8''([^;\n]+)/i.exec(cd);
          if (utf?.[1]) fileName = decodeURIComponent(utf[1].trim());
          else {
            const m = /filename="?([^";\n]+)"?/i.exec(cd);
            if (m?.[1]) fileName = m[1].trim();
          }
        }
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        a.rel = 'noopener';
        a.click();
        URL.revokeObjectURL(url);
        this.pdfLoading.set(false);
      },
      error: () => {
        this.toast.error('Não foi possível baixar o PDF. Verifique sua conexão e tente de novo.');
        this.pdfLoading.set(false);
      },
    });
  }

  verAssinatura(a: AssinaturaDto) {
    this.dialog.open(SignatureViewDialogComponent, {
      width: '480px',
      data: {
        nomeUsuario: a.nomeUsuario,
        tipoAssinante: labelTipoAssinante(a.tipoAssinante),
        dataAssinatura: a.dataAssinatura,
        imagemAssinatura: a.urlImagemAssinatura,
      },
    });
  }

  labelAssinante(tipo: number): string {
    return labelTipoAssinante(tipo);
  }
}
