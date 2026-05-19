import { Component, signal, inject, OnInit, computed } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { NotificacaoService } from '../../core/services/notificacao.service';
import { AssinaturaService } from '../../core/services/assinatura.service';
import { ToastService } from '../../core/services/toast.service';
import { EmptyStateComponent } from '../../shared/components/empty-state/empty-state.component';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { SignaturePadDialogComponent } from '../../shared/components/signature-pad-dialog/signature-pad-dialog.component';
import {
  PendenteAssinaturaDto,
  TipoEntidadeAssinatura,
} from '../../shared/models/api.models';
import {
  labelTipoAssinante,
  TIPO_ENTIDADE_LABELS,
} from '../../shared/utils/assinatura.utils';

@Component({
  selector: 'app-pendencias',
  standalone: true,
  imports: [
    NgTemplateOutlet,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    DatePipe,
    PageHeaderComponent,
    EmptyStateComponent,
  ],
  templateUrl: './pendencias.component.html',
})
export class PendenciasComponent implements OnInit {
  private readonly assinaturaService = inject(AssinaturaService);
  private readonly notificacaoService = inject(NotificacaoService);
  private readonly toast = inject(ToastService);
  private readonly dialog = inject(MatDialog);

  readonly TipoEntidadeAssinatura = TipoEntidadeAssinatura;

  loading = signal(true);
  pendentes = signal<PendenteAssinaturaDto[]>([]);

  obrasPendentes = computed(() =>
    this.pendentes().filter((p) => p.tipoEntidade === TipoEntidadeAssinatura.Obra),
  );
  termosPendentes = computed(() =>
    this.pendentes().filter((p) => p.tipoEntidade === TipoEntidadeAssinatura.TermoConclusao),
  );
  registrosPendentes = computed(() =>
    this.pendentes().filter((p) => p.tipoEntidade === TipoEntidadeAssinatura.RelatoVisita),
  );

  ngOnInit() {
    this.carregar();
  }

  carregar() {
    this.loading.set(true);
    this.assinaturaService.pendentes().subscribe({
      next: (list) => {
        this.pendentes.set(list);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.toast.error('Erro ao carregar pendências.');
      },
    });
  }

  iconeEntidade(tipo: TipoEntidadeAssinatura): string {
    switch (tipo) {
      case TipoEntidadeAssinatura.Obra:
        return 'construction';
      case TipoEntidadeAssinatura.RelatoVisita:
        return 'edit_note';
      case TipoEntidadeAssinatura.TermoConclusao:
        return 'description';
      default:
        return 'pending_actions';
    }
  }

  corEntidade(tipo: TipoEntidadeAssinatura): string {
    switch (tipo) {
      case TipoEntidadeAssinatura.Obra:
        return 'text-amber-600';
      case TipoEntidadeAssinatura.RelatoVisita:
        return 'text-blue-500';
      case TipoEntidadeAssinatura.TermoConclusao:
        return 'text-orange-500';
      default:
        return 'text-slate-500';
    }
  }

  labelEntidade(tipo: TipoEntidadeAssinatura): string {
    return TIPO_ENTIDADE_LABELS[tipo];
  }

  labelAssinante(p: PendenteAssinaturaDto): string {
    return labelTipoAssinante(p.tipoAssinante);
  }

  linkObraId(p: PendenteAssinaturaDto): string | null {
    if (p.tipoEntidade === TipoEntidadeAssinatura.Obra) return p.entidadeId;
    return null;
  }

  assinar(pendente: PendenteAssinaturaDto) {
    const dialogRef = this.dialog.open(SignaturePadDialogComponent, {
      width: '560px',
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((imagemAssinatura: string | null) => {
      if (!imagemAssinatura) return;

      const navegador = typeof navigator !== 'undefined' ? navigator.userAgent : undefined;

      this.assinaturaService
        .assinar({
          tipoEntidade: pendente.tipoEntidade,
          entidadeId: pendente.entidadeId,
          imagemAssinatura,
          navegador,
        })
        .subscribe({
          next: () => {
            this.toast.success('Assinatura registrada com sucesso!');
            this.pendentes.update((list) =>
              list.filter(
                (p) =>
                  !(
                    p.tipoEntidade === pendente.tipoEntidade &&
                    p.entidadeId === pendente.entidadeId &&
                    p.tipoAssinante === pendente.tipoAssinante
                  ),
              ),
            );
            this.notificacaoService.carregarMeusPendentes();
          },
          error: () => this.toast.error('Erro ao registrar assinatura.'),
        });
    });
  }
}
