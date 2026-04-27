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
import { OcorrenciaService } from '../../../core/services/ocorrencia.service';
import { TipoOcorrencia, TIPO_OCORRENCIA_LABELS } from '../../../shared/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
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

  isEdit = signal(false);
  saving = signal(false);
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
    }
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const val = this.form.getRawValue();
    const dto = { ...val, obraId: this.obraId, dataOcorrencia: val.dataOcorrencia!.toISOString() };
    const onOk = () => {
      this.toast.success('Ocorrência salva!');
      this.voltar();
    };
    const onErr = () => {
      this.toast.error('Erro ao salvar ocorrência.');
      this.saving.set(false);
    };
    if (this.isEdit()) {
      this.service.atualizar(this.editId!, dto).subscribe({ next: onOk, error: onErr });
    } else {
      this.service.criar(dto).subscribe({ next: onOk, error: onErr });
    }
    return;
  }

  voltar() {
    this.router.navigate(['/obras', this.obraId]);
  }
}
