import { Component, signal, inject, OnInit, computed } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatBadgeModule } from '@angular/material/badge';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { ObraService } from '../../../core/services/obra.service';
import { RelatoVisitaService } from '../../../core/services/registro-diario.service';
import { AnexoService } from '../../../core/services/anexo.service';
import { TermoConclusaoService } from '../../../core/services/termo-conclusao.service';
import { AssinaturaService } from '../../../core/services/assinatura.service';
import {
  ObraDto,
  RelatoVisitaDto,
  AnexoDto,
  TermoConclusaoDto,
  AssinaturaDto,
  StatusObra,
  STATUS_OBRA_LABELS,
  TIPO_OBRA_LABELS,
  TIPO_EDIFICACAO_LABELS,
  ATIVIDADE_TECNICA_LABELS,
  POSICAO_OBRA_LABELS,
  TipoUsuario,
  TipoEntidadeAssinatura,
  TipoAssinante,
} from '../../../shared/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { SignaturePadDialogComponent } from '../../../shared/components/signature-pad-dialog/signature-pad-dialog.component';
import { SignatureViewDialogComponent } from '../../../shared/components/signature-view-dialog/signature-view-dialog.component';
import { ToastService } from '../../../core/services/toast.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificacaoService } from '../../../core/services/notificacao.service';
import {
  EntidadeAnexosDialogComponent,
  EntidadeAnexosDialogData,
} from '../../../shared/components/entidade-anexos-dialog/entidade-anexos-dialog.component';
import {
  labelTipoAssinante,
  possuiAssinatura,
  tipoAssinanteDoUsuario,
  usuarioPodeAssinarEntidade,
} from '../../../shared/utils/assinatura.utils';

@Component({
  selector: 'app-obra-detail',
  standalone: true,
  imports: [
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    MatMenuModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatBadgeModule,
    MatTooltipModule,
    DatePipe,
    PageHeaderComponent,
    StatusBadgeComponent,
    EmptyStateComponent,
  ],
  templateUrl: `./obra-detail.component.html`,
})
export class ObraDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly obraService = inject(ObraService);
  private readonly registroService = inject(RelatoVisitaService);
  private readonly anexoService = inject(AnexoService);
  private readonly termoService = inject(TermoConclusaoService);
  private readonly assinaturaService = inject(AssinaturaService);
  private readonly toast = inject(ToastService);
  private readonly auth = inject(AuthService);
  private readonly notificacaoService = inject(NotificacaoService);
  private readonly dialog = inject(MatDialog);

  readonly TipoAssinante = TipoAssinante;

  loading = signal(true);
  obra = signal<ObraDto | null>(null);
  registros = signal<RelatoVisitaDto[]>([]);
  anexos = signal<AnexoDto[]>([]);
  termo = signal<TermoConclusaoDto | null>(null);
  assinaturasObra = signal<AssinaturaDto[]>([]);
  userType = computed(() => this.auth.currentUser()?.tipoUsuario);

  tipoLabel = () => (this.obra() ? TIPO_OBRA_LABELS[this.obra()!.tipoObra] : '');
  tipoEdificacaoLabel = () =>
    this.obra()?.tipoEdificacao ? TIPO_EDIFICACAO_LABELS[this.obra()!.tipoEdificacao!] : '–';
  atividadeTecnicaLabel = () =>
    this.obra()?.atividadeTecnica ? ATIVIDADE_TECNICA_LABELS[this.obra()!.atividadeTecnica!] : '–';
  posicaoObraLabel = (r: RelatoVisitaDto) =>
    r.posicaoObra ? POSICAO_OBRA_LABELS[r.posicaoObra] : '';

  obraAssinadaProfissional = computed(() =>
    possuiAssinatura(this.assinaturasObra(), TipoAssinante.Profissional),
  );
  obraAssinadaCrea = computed(() =>
    possuiAssinatura(this.assinaturasObra(), TipoAssinante.UsuarioCrea),
  );

  statusOptions = Object.entries(STATUS_OBRA_LABELS).map(([v, l]) => ({
    value: Number(v) as StatusObra,
    label: l,
  }));

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    const assinar = this.route.snapshot.queryParamMap.get('assinar');
    const registroId = this.route.snapshot.queryParamMap.get('r');
    this.obraService.obter(id).subscribe({
      next: (obra) => {
        this.obra.set(obra);
        this.loading.set(false);
        this.loadRelated(id, assinar, registroId);
      },
      error: () => {
        this.loading.set(false);
        this.router.navigate(['/obras']);
      },
    });
  }

  private loadRelated(id: string, assinar?: string | null, registroId?: string | null) {
    this.registroService.porObra(id).subscribe((r) => {
      this.registros.set(r);
      if (assinar === 'registro' && registroId) {
        const target = r.find((reg) => reg.id === registroId);
        if (target && this.podeAssinarRegistro(target)) this.assinarRegistro(target);
      }
    });
    this.anexoService.porObra(id).subscribe((a) => this.anexos.set(a));
    this.termoService.porObra(id).subscribe({
      next: (t) => {
        this.termo.set(t);
        if (assinar === 'termo' && t && this.podeAssinarTermo()) this.assinarTermo();
      },
      error: () => {},
    });
    this.assinaturaService.porEntidade(TipoEntidadeAssinatura.Obra, id).subscribe({
      next: (list) => {
        console.log('Assinaturas da obra:', list);
        this.assinaturasObra.set(list);
        if (assinar === 'obra' && this.podeAssinarObra()) this.assinarObra();
      },
      error: () => this.assinaturasObra.set([]),
    });
  }

  mudarStatus(status: StatusObra) {
    this.obraService.atualizarStatus(this.obra()!.id, status).subscribe({
      next: () => {
        this.obra.update((o) => (o ? { ...o, status } : o));
        this.toast.success('Status atualizado.');
      },
      error: () => this.toast.error('Erro ao atualizar status.'),
    });
  }

  podeAssinarObra(): boolean {
    const tipo = this.userType();
    if (!tipo || !usuarioPodeAssinarEntidade(tipo, TipoEntidadeAssinatura.Obra)) return false;
    const papel = tipoAssinanteDoUsuario(tipo);
    if (!papel) return false;
    return !possuiAssinatura(this.assinaturasObra(), papel);
  }

  podeAssinarTermo(): boolean {
    const termo = this.termo();
    const tipo = this.userType();
    if (!termo || termo.concluido || !tipo) return false;
    if (!usuarioPodeAssinarEntidade(tipo, TipoEntidadeAssinatura.TermoConclusao)) return false;
    const papel = tipoAssinanteDoUsuario(tipo);
    if (!papel) return false;
    if (papel === TipoAssinante.Profissional) return !termo.assinadoPeloProfissional;
    if (papel === TipoAssinante.Proprietario) return !termo.assinadoPeloProprietario;
    return false;
  }

  podeAssinarRegistro(r: RelatoVisitaDto): boolean {
    const tipo = this.userType();
    if (!tipo || !usuarioPodeAssinarEntidade(tipo, TipoEntidadeAssinatura.RelatoVisita))
      return false;
    const papel = tipoAssinanteDoUsuario(tipo);
    if (!papel) return false;
    if (papel === TipoAssinante.Profissional) return !r.assinadoPeloProfissional;
    if (papel === TipoAssinante.Proprietario) return !r.assinadoPeloProprietario;
    return false;
  }

  uploadAnexo(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    this.anexoService.upload(file, { obraId: this.obra()!.id }).subscribe({
      next: (anexo) => {
        this.anexos.update((a) => [...a, anexo]);
        this.toast.success('Arquivo enviado.');
      },
      error: () => this.toast.error('Erro ao enviar arquivo.'),
    });
  }

  assinarEntidade(
    tipoEntidade: TipoEntidadeAssinatura,
    entidadeId: string,
    onSuccess?: () => void,
  ) {
    const dialogRef = this.dialog.open(SignaturePadDialogComponent, {
      width: '560px',
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((arquivo: File | null) => {
      if (!arquivo) return;
      this.assinaturaService
        .assinar({
          tipoEntidade,
          entidadeId,
          imagemAssinatura: arquivo,
          navegador: typeof navigator !== 'undefined' ? navigator.userAgent : undefined,
        })
        .subscribe({
          next: () => {
            this.toast.success('Assinatura registrada com sucesso!');
            this.notificacaoService.carregarMeusPendentes();
            onSuccess?.();
          },
          error: () => this.toast.error('Erro ao registrar assinatura.'),
        });
    });
  }

  assinarObra() {
    const id = this.obra()?.id;
    if (!id) return;
    this.assinarEntidade(TipoEntidadeAssinatura.Obra, id, () => {
      this.assinaturaService.porEntidade(TipoEntidadeAssinatura.Obra, id).subscribe((list) => {
        this.assinaturasObra.set(list);
      });
    });
  }

  assinarTermo() {
    const t = this.termo();
    if (!t) return;
    this.assinarEntidade(TipoEntidadeAssinatura.TermoConclusao, t.id, () => {
      this.termoService.porObra(t.obraId).subscribe((updated) => this.termo.set(updated));
    });
  }

  assinarRegistro(r: RelatoVisitaDto) {
    this.assinarEntidade(TipoEntidadeAssinatura.RelatoVisita, r.id, () => {
      this.registroService.porObra(r.obraId).subscribe((list) => this.registros.set(list));
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

  labelAssinante(tipo: TipoAssinante): string {
    return labelTipoAssinante(tipo);
  }

  copiarLink(tipo: 'obra' | 'registro' | 'termo', registroId?: string): void {
    const obraId = this.obra()!.id;
    let url = `${window.location.origin}/obras/${obraId}?assinar=${tipo}`;
    if (tipo === 'registro' && registroId) url += `&r=${registroId}`;
    navigator.clipboard.writeText(url).then(
      () => this.toast.success('Link de assinatura copiado!'),
      () => this.toast.error('Não foi possível copiar o link.'),
    );
  }

  qtdAnexosRegistro(r: RelatoVisitaDto): number {
    return r.quantidadeAnexos ?? 0;
  }

  abrirAnexosRegistro(r: RelatoVisitaDto) {
    const dataFmt = new Date(r.data).toLocaleDateString('pt-BR');
    this.abrirAnexosDialog({
      tipo: 'registro',
      entidadeId: r.id,
      titulo: `Registro #${r.numeroSequencial} — ${dataFmt}`,
    });
  }

  private abrirAnexosDialog(data: EntidadeAnexosDialogData) {
    this.dialog.open(EntidadeAnexosDialogComponent, {
      width: '520px',
      maxWidth: '95vw',
      data,
    });
  }
}
