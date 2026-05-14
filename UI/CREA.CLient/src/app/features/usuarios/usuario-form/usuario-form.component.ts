import { Component, signal, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { UsuarioService } from '../../../core/services/usuario.service';
import { TipoUsuario, TIPO_USUARIO_LABELS } from '../../../shared/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-usuario-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatProgressSpinnerModule,
    PageHeaderComponent,
  ],
  templateUrl: './usuario-form.component.html',
})
export class UsuarioFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(UsuarioService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  isEdit = signal(false);
  saving = signal(false);
  private editId: string | null = null;

  tiposUsuario = [
    { value: TipoUsuario.Operacional, label: TIPO_USUARIO_LABELS[TipoUsuario.Operacional] },
    {
      value: TipoUsuario.ResponsavelTecnico,
      label: TIPO_USUARIO_LABELS[TipoUsuario.ResponsavelTecnico],
    },
    { value: TipoUsuario.Admin, label: TIPO_USUARIO_LABELS[TipoUsuario.Admin] },
  ];

  form = this.fb.nonNullable.group({
    nome: ['', [Validators.required, Validators.maxLength(150)]],
    email: ['', [Validators.required, Validators.email]],
    senha: ['', [Validators.required, Validators.minLength(6)]],
    tipoUsuario: [TipoUsuario.Operacional, Validators.required],
    ativo: [true],
  });

  ngOnInit() {
    this.editId = this.route.snapshot.paramMap.get('id');
    if (this.editId) {
      this.isEdit.set(true);
      this.form.controls.senha.clearValidators();
      this.form.controls.senha.updateValueAndValidity();
      this.service.obter(this.editId).subscribe((u) =>
        this.form.patchValue({
          nome: u.nome,
          email: u.email,
          tipoUsuario: u.tipoUsuario,
          ativo: u.ativo,
        }),
      );
    }
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const v = this.form.getRawValue();

    if (this.isEdit()) {
      this.service
        .atualizar(this.editId!, {
          nome: v.nome,
          email: v.email,
          tipoUsuario: v.tipoUsuario,
          ativo: v.ativo,
        })
        .subscribe({
          next: () => {
            this.toast.success('Usuário atualizado!');
            this.voltar();
          },
          error: () => {
            this.toast.error('Erro ao salvar.');
            this.saving.set(false);
          },
        });
    } else {
      this.service
        .criar({
          nome: v.nome,
          email: v.email,
          senha: v.senha,
          tipoUsuario: v.tipoUsuario,
        })
        .subscribe({
          next: () => {
            this.toast.success('Usuário cadastrado!');
            this.voltar();
          },
          error: () => {
            this.toast.error('Erro ao salvar.');
            this.saving.set(false);
          },
        });
    }
  }

  voltar() {
    this.router.navigate(['/usuarios']);
  }
}
