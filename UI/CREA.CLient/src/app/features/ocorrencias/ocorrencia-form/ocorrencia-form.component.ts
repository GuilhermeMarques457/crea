import { Component, signal, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { forkJoin, of } from 'rxjs';
import { finalize, switchMap } from 'rxjs/operators';
import { OcorrenciaService } from '../../../core/services/ocorrencia.service';
import { AnexoService } from '../../../core/services/anexo.service';
import {
  AnexoDto,
  TipoOcorrencia,
  TIPO_OCORRENCIA_LABELS,
} from '../../../shared/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ToastService } from '../../../core/services/toast.service';
import moment from 'moment';

@Component({
  selector: 'app-ocorrencia-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    PageHeaderComponent,
  ],
  templateUrl: `./ocorrencia-form.component.html`,
})
export class OcorrenciaFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(OcorrenciaService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  private readonly anexoService = inject(AnexoService);
  private readonly dialog = inject(MatDialog);

  isEdit = signal(false);
  saving = signal(false);
  readonly anexos = signal<AnexoDto[]>([]);
  readonly arquivosPendentes = signal<File[]>([]);
  private obraId!: string;
  private editId: string | null = null;

  tipoOptions = Object.entries(TIPO_OCORRENCIA_LABELS).map(([v, l]) => ({
    value: Number(v) as TipoOcorrencia,
    label: l,
  }));

  form = this.fb.nonNullable.group({
    dataOcorrencia: [null as any, Validators.required],
    tipo: [TipoOcorrencia.Tecnica, Validators.required],
    titulo: ['', [Validators.required, Validators.maxLength(200)]],
    descricao: ['', Validators.required],
    providencias: [''],
  });

  ngOnInit() {
    this.obraId = this.route.snapshot.paramMap.get('obraId')!;
    this.editId = this.route.snapshot.paramMap.get('id');
    if (this.editId) {
      this.isEdit.set(true);
      this.service.obter(this.editId).subscribe((o) => {
        this.form.patchValue({ ...o, dataOcorrencia: moment(o.dataOcorrencia) });
        this.obraId = o.obraId;
      });
      this.anexoService.porOcorrencia(this.editId).subscribe((list) => this.anexos.set(list));
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
      forkJoin(files.map((f) => this.anexoService.upload(f, { ocorrenciaId: this.editId! })))
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
    const dto = { ...val, obraId: this.obraId, dataOcorrencia: val.dataOcorrencia!.toISOString() };
    const onErr = () => {
      this.toast.error('Erro ao salvar ocorrência.');
      this.saving.set(false);
    };

    if (this.isEdit()) {
      this.service
        .atualizar(this.editId!, dto)
        .pipe(finalize(() => this.saving.set(false)))
        .subscribe({
          next: () => {
            this.toast.success('Ocorrência salva!');
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
            pendentes.map((f) => this.anexoService.upload(f, { ocorrenciaId: created.id })),
          );
        }),
        finalize(() => this.saving.set(false)),
      )
      .subscribe({
        next: () => {
          this.arquivosPendentes.set([]);
          this.toast.success('Ocorrência salva!');
          this.voltar();
        },
        error: onErr,
      });
  }

  voltar() {
    this.router.navigate(['/obras', this.obraId]);
  }
}
