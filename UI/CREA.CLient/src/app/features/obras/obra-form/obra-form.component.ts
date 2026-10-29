import { Component, signal, inject, OnInit, computed } from '@angular/core';
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
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ObraService } from '../../../core/services/obra.service';
import { ProfissionalService } from '../../../core/services/profissional.service';
import { ProprietarioService } from '../../../core/services/proprietario.service';
import {
  ProfissionalDto,
  ProprietarioDto,
  ObraDto,
  CreateObraDto,
  TipoEdificacao,
  TIPO_EDIFICACAO_LABELS,
  AtividadeTecnica,
  ATIVIDADE_TECNICA_LABELS,
} from '../../../shared/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { ToastService } from '../../../core/services/toast.service';
import { NotificacaoService } from '../../../core/services/notificacao.service';
import { Location } from '@angular/common';
import moment from 'moment';
import { AuthService } from '../../../core/services/auth.service';
import { merge } from 'rxjs';

@Component({
  selector: 'app-obra-form',
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
    MatCheckboxModule,
    PageHeaderComponent,
  ],
  templateUrl: `./obra-form.component.html`,
})
export class ObraFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly obraService = inject(ObraService);
  private readonly profissionalService = inject(ProfissionalService);
  private readonly proprietarioService = inject(ProprietarioService);
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  private readonly notificacaoService = inject(NotificacaoService);
  private readonly location = inject(Location);

  isProfissional = computed(() => this.auth.currentUser()?.profissionalId != null);
  isEdit = signal(false);
  saving = signal(false);
  profissionais = signal<ProfissionalDto[]>([]);
  proprietarios = signal<ProprietarioDto[]>([]);
  private editId: string | null = null;

  tipoEdificacaoOptions = Object.entries(TIPO_EDIFICACAO_LABELS).map(([v, l]) => ({
    value: Number(v) as TipoEdificacao,
    label: l,
  }));
  atividadeTecnicaOptions = Object.entries(ATIVIDADE_TECNICA_LABELS).map(([v, l]) => ({
    value: Number(v) as AtividadeTecnica,
    label: l,
  }));

  form = this.fb.nonNullable.group({
    localObra: ['', [Validators.maxLength(300)]],
    logradouro: ['', [Validators.required, Validators.maxLength(200)]],
    quadra: ['', [Validators.maxLength(50)]],
    lote: ['', [Validators.maxLength(50)]],
    numero: [0, [Validators.required, Validators.min(0)]],
    proprietarioId: ['', Validators.required],
    empresa: [''],
    numeroCaderneta: [''],
    numeroRT: ['', [Validators.required, Validators.maxLength(50)]],
    tipoEdificacao: [null as TipoEdificacao | null],
    atividadeTecnica: [null as AtividadeTecnica | null],
    dataInicio: [moment() as any, Validators.required],
    areaConstruir: [null as number | null],
    areaRegularizar: [null as number | null],
    areaAmpliar: [null as number | null],
    areaReformar: [null as number | null],
    areaTotalEdificada: [null as number | null],
    valorRecibo: [null as number | null],
    profissionalId: [this.auth.currentUser()?.profissionalId ?? null as any, Validators.required],
  });

  

  ngOnInit() {
    const profissionalId = this.auth.currentUser()?.profissionalId;

    if (profissionalId) {
      this.form.controls.profissionalId.disable();
    }
    
    this.profissionalService
      .listar()
      .subscribe((p: ProfissionalDto[]) => this.profissionais.set(p));
    this.proprietarioService
      .listar()
      .subscribe((list: ProprietarioDto[]) => this.proprietarios.set(list));
    this.editId = this.route.snapshot.paramMap.get('id');
    if (this.editId) {
      this.isEdit.set(true);
      this.obraService.obter(this.editId).subscribe((obra: ObraDto) => {
        this.form.patchValue({
          ...obra,
          dataInicio: moment(obra.dataInicio),
          tipoEdificacao: obra.tipoEdificacao ?? null,
          atividadeTecnica: obra.atividadeTecnica ?? null,
          areaConstruir: obra.areaConstruir ?? null,
          areaRegularizar: obra.areaRegularizar ?? null,
          areaAmpliar: obra.areaAmpliar ?? null,
          areaReformar: obra.areaReformar ?? null,
          areaTotalEdificada: obra.areaTotalEdificada ?? null,
          valorRecibo: obra.valorRecibo ?? null,
        });
      });
    }

    merge(
      this.form.get('areaConstruir')!.valueChanges,
      this.form.get('areaRegularizar')!.valueChanges,
      this.form.get('areaAmpliar')!.valueChanges,
      this.form.get('areaReformar')!.valueChanges
    ).subscribe(() => {
      this.calcularAreaTotal();
    });
  }

  private calcularAreaTotal(): void {
    const areaConstruir = Number(this.form.get('areaConstruir')?.value) || 0;
    const areaRegularizar = Number(this.form.get('areaRegularizar')?.value) || 0;
    const areaAmpliar = Number(this.form.get('areaAmpliar')?.value) || 0;
    const areaReformar = Number(this.form.get('areaReformar')?.value) || 0;
  
    const total =
      areaConstruir +
      areaRegularizar +
      areaAmpliar +
      areaReformar;
  
    this.form.get('areaTotalEdificada')?.setValue(total, {
      emitEvent: false
    });
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const val = this.form.getRawValue();
    const dto: CreateObraDto = {
      ...val,
      localObra: val.localObra.trim() || val.logradouro.trim(),
      dataInicio: val.dataInicio!.toISOString(),
      tipoEdificacao: val.tipoEdificacao ?? undefined,
      atividadeTecnica: val.atividadeTecnica ?? undefined,
      areaConstruir: val.areaConstruir ?? undefined,
      areaRegularizar: val.areaRegularizar ?? undefined,
      areaAmpliar: val.areaAmpliar ?? undefined,
      areaReformar: val.areaReformar ?? undefined,
      areaTotalEdificada: val.areaTotalEdificada ?? undefined,
      valorRecibo: val.valorRecibo ?? undefined,
    };
    const onSuccess = () => {
      this.toast.success(this.isEdit() ? 'Obra atualizada!' : 'Obra cadastrada!');
      if (!this.isEdit()) {
        this.notificacaoService.carregarMeusPendentes();
      }
      this.router.navigate(['/obras']);
    };
    const onError = () => {
      this.toast.error('Erro ao salvar obra.');
      this.saving.set(false);
    };
    if (this.isEdit()) {
      this.obraService.atualizar(this.editId!, dto).subscribe({ next: onSuccess, error: onError });
    } else {
      this.obraService.criar(dto).subscribe({ next: onSuccess, error: onError });
    }
    return;
  }

  voltar() {
    this.location.back();
  }
}
