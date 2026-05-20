import { Component, signal, inject, OnInit, computed } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatDialog } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { DatePipe } from '@angular/common';
import { forkJoin, of } from 'rxjs';
import { finalize, switchMap } from 'rxjs/operators';
import { RelatoVisitaService } from '../../../core/services/registro-diario.service';
import { AnexoService } from '../../../core/services/anexo.service';
import { AssinaturaService } from '../../../core/services/assinatura.service';
import { AuthService } from '../../../core/services/auth.service';
import {
  AnexoDto,
  AssinaturaDto,
  PosicaoObra,
  POSICAO_OBRA_LABELS,
  TipoEntidadeAssinatura,
  TipoAssinante,
  TipoUsuario,
} from '../../../shared/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { SignaturePadDialogComponent } from '../../../shared/components/signature-pad-dialog/signature-pad-dialog.component';
import { SignatureViewDialogComponent } from '../../../shared/components/signature-view-dialog/signature-view-dialog.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ToastService } from '../../../core/services/toast.service';
import {
  labelTipoAssinante,
  tipoAssinanteDoUsuario,
  usuarioPodeAssinarEntidade,
} from '../../../shared/utils/assinatura.utils';
import moment from 'moment';

@Component({
  selector: 'app-registro-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatProgressSpinnerModule,
    MatCheckboxModule,
    MatSelectModule,
    MatTooltipModule,
    DatePipe,
    PageHeaderComponent,
  ],
  templateUrl: `./relato-form.component.html`,
})
export class RelatoVisitaFormComponent implements OnInit {
  readonly TipoAssinante = TipoAssinante;
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(RelatoVisitaService);
  private readonly assinaturaService = inject(AssinaturaService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  private readonly auth = inject(AuthService);
  private readonly dialog = inject(MatDialog);
  private readonly anexoService = inject(AnexoService);

  isEdit = signal(false);
  saving = signal(false);
  readonly anexos = signal<AnexoDto[]>([]);
  readonly assinaturas = signal<AssinaturaDto[]>([]);
  readonly arquivosPendentes = signal<File[]>([]);

  podeAssinar = computed(() => {
    const tipo = this.auth.currentUser()?.tipoUsuario;
    if (!tipo || !this.editId) return false;
    if (!usuarioPodeAssinarEntidade(tipo, TipoEntidadeAssinatura.RelatoVisita)) return false;
    const papel = tipoAssinanteDoUsuario(tipo);
    if (!papel) return false;
    return !this.assinaturas().some((a) => a.tipoAssinante === papel);
  });

  private obraId!: string;
  private editId: string | null = null;

  posicaoObraOptions = Object.entries(POSICAO_OBRA_LABELS).map(([v, l]) => ({
    value: Number(v) as PosicaoObra,
    label: l,
  }));

  form = this.fb.nonNullable.group({
    data: [null as any, Validators.required],
    atividades: ['', Validators.required],
    equipePresente: ['', [Validators.required, Validators.maxLength(500)]],
    condicaoClimatica: [''],
    observacoes: [''],
    servicosPreliminar: [false],
    fundacao: [false],
    alvenarias: [false],
    superestrutura: [false],
    cobertura: [false],
    esquadriasInstalacoesEletricasHidraulicas: [false],
    revestimentoForroParePiso: [false],
    pintura: [false],
    servicosComplementares: [false],
    posicaoObra: [null as PosicaoObra | null],
    decisoesTecnicas: [''],
  });

  ngOnInit() {
    this.obraId =
      this.route.snapshot.paramMap.get('obraId') ?? this.route.snapshot.queryParams['obraId'];
    this.editId = this.route.snapshot.paramMap.get('id');
    if (this.editId) {
      this.isEdit.set(true);
      this.service.obter(this.editId).subscribe((r) => {
        this.form.patchValue({
          ...r,
          data: moment(r.data),
          posicaoObra: r.posicaoObra ?? null,
        });
        this.obraId = r.obraId;
        this.assinaturas.set(r.assinaturas ?? []);
      });
      this.anexoService.porRegistro(this.editId).subscribe((list) => this.anexos.set(list));
    }
  }

  readonly acceptAnexos =
    'image/jpeg,image/png,image/gif,application/pdf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document';

  onAnexosSelecionados(event: Event) {
    const input = event.target as HTMLInputElement;
    const files = input.files ? Array.from(input.files) : [];
    input.value = '';
    if (!files.length) return;

    if (this.isEdit() && this.editId) {
      this.saving.set(true);
      forkJoin(files.map((f) => this.anexoService.upload(f, { registroDiarioId: this.editId! })))
        .pipe(finalize(() => this.saving.set(false)))
        .subscribe({
          next: (criados) => this.anexos.update((a) => [...a, ...criados]),
          error: () => this.toast.error('Erro ao enviar um ou mais anexos.'),
        });
      return;
    }

    this.arquivosPendentes.update((p) => [...p, ...files]);
  }

  removerPendente(index: number) {
    this.arquivosPendentes.update((p) => p.filter((_, i) => i !== index));
  }

  confirmarExcluirAnexo(a: AnexoDto) {
    this.dialog
      .open(ConfirmDialogComponent, {
        data: {
          title: 'Remover anexo',
          message: `Remover "${a.nomeArquivoOriginal}"?`,
          confirmLabel: 'Remover',
        },
      })
      .afterClosed()
      .subscribe((ok) => {
        if (!ok) return;
        this.anexoService.excluir(a.id).subscribe({
          next: () => {
            this.anexos.update((list) => list.filter((x) => x.id !== a.id));
            this.toast.success('Anexo removido.');
          },
          error: () => this.toast.error('Erro ao remover anexo.'),
        });
      });
  }

  tamanhoKb(bytes: number): string {
    return (bytes / 1024).toFixed(1);
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const val = this.form.getRawValue();
    const dto = {
      ...val,
      obraId: this.obraId,
      data: val.data!.toISOString(),
      posicaoObra: val.posicaoObra ?? undefined,
    };
    const onErr = () => {
      this.toast.error('Erro ao salvar registro.');
      this.saving.set(false);
    };

    if (this.isEdit()) {
      this.service
        .atualizar(this.editId!, dto)
        .pipe(finalize(() => this.saving.set(false)))
        .subscribe({
          next: () => {
            this.toast.success('Registro salvo!');
            this.voltar();
          },
          error: onErr,
        });
      return;
    }

    this.service
      .criar(dto)
      .pipe(
        switchMap((created) => {
          const pendentes = this.arquivosPendentes();
          if (!pendentes.length) return of(null);
          return forkJoin(
            pendentes.map((f) => this.anexoService.upload(f, { registroDiarioId: created.id })),
          );
        }),
        finalize(() => this.saving.set(false)),
      )
      .subscribe({
        next: () => {
          this.arquivosPendentes.set([]);
          this.toast.success('Registro salvo!');
          this.voltar();
        },
        error: onErr,
      });
  }

  voltar() {
    this.router.navigate(['/obras', this.obraId]);
  }

  labelAssinante(tipo: TipoAssinante): string {
    return labelTipoAssinante(tipo);
  }

  assinadoPelo(tipo: TipoAssinante): boolean {
    return this.assinaturas().some((a) => a.tipoAssinante === tipo);
  }

  assinar() {
    if (!this.editId) return;
    const dialogRef = this.dialog.open(SignaturePadDialogComponent, {
      width: '560px',
      disableClose: true,
    });
    dialogRef.afterClosed().subscribe((arquivo: File | null) => {
      if (!arquivo) return;
      this.assinaturaService
        .assinar({
          tipoEntidade: TipoEntidadeAssinatura.RelatoVisita,
          entidadeId: this.editId!,
          imagemAssinatura: arquivo,
          navegador: typeof navigator !== 'undefined' ? navigator.userAgent : undefined,
        })
        .subscribe({
          next: () => {
            this.toast.success('Assinatura registrada!');
            this.assinaturaService
              .porEntidade(TipoEntidadeAssinatura.RelatoVisita, this.editId!)
              .subscribe((list) => this.assinaturas.set(list));
          },
          error: () => this.toast.error('Erro ao registrar assinatura.'),
        });
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
}
