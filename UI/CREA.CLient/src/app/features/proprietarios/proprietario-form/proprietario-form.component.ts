import { Component, signal, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ProprietarioService } from '../../../core/services/proprietario.service';
import { UsuarioService } from '../../../core/services/usuario.service';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { ToastService } from '../../../core/services/toast.service';
import { CpfMaskDirective } from '../../../shared/directives/cpf-mask.directive';
import { PhoneBrMaskDirective } from '../../../shared/directives/phone-br-mask.directive';
import { cpfDigitsValidator } from '../../../shared/validators/cpf.validator';
import { phoneBrDigitsValidator } from '../../../shared/validators/phone-br.validator';
import { TipoUsuario, UsuarioDto } from '../../../shared/models/api.models';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-proprietario-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    PageHeaderComponent,
    CpfMaskDirective,
    PhoneBrMaskDirective,
  ],
  templateUrl: `./proprietario-form.component.html`,
})
export class ProprietarioFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(ProprietarioService);
  private readonly usuarioService = inject(UsuarioService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  isEdit = signal(false);
  saving = signal(false);
  loadingUsuarios = signal(true);
  usuariosDisponiveis = signal<UsuarioDto[]>([]);
  private editId: string | null = null;

  form = this.fb.nonNullable.group({
    nome: ['', [Validators.required, Validators.maxLength(200)]],
    cpf: ['', [cpfDigitsValidator(true)]],
    email: ['', [Validators.maxLength(200)]],
    telefone: ['', [phoneBrDigitsValidator(true)]],
    usuarioId: ['', [Validators.required]],
  });

  ngOnInit() {
    this.editId = this.route.snapshot.paramMap.get('id');
    const isEdit = !!this.editId;
    this.isEdit.set(isEdit);

    forkJoin({
      usuarios: this.usuarioService.porTipo(TipoUsuario.Proprietario),
      proprietarios: this.service.listar(),
    }).subscribe({
      next: ({ usuarios, proprietarios }) => {
        const jaVinculados = new Set(
          proprietarios.filter((p) => p.usuarioId).map((p) => p.usuarioId!),
        );
        // When editing, keep the current user in the list even if already linked
        const currentUsuarioId = isEdit
          ? proprietarios.find((p) => p.id === this.editId)?.usuarioId
          : undefined;

        this.usuariosDisponiveis.set(
          usuarios.filter((u) => !jaVinculados.has(u.id) || u.id === currentUsuarioId),
        );
        this.loadingUsuarios.set(false);
      },
      error: () => this.loadingUsuarios.set(false),
    });

    if (isEdit) {
      this.service.obter(this.editId!).subscribe((p) =>
        this.form.patchValue({
          nome: p.nome,
          cpf: p.cpf || '',
          email: p.email || '',
          telefone: p.telefone || '',
          usuarioId: p.usuarioId || '',
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
    const raw = this.form.getRawValue();
    const dto = {
      nome: raw.nome,
      cpf: raw.cpf.trim() || undefined,
      email: raw.email.trim() || undefined,
      telefone: raw.telefone.trim() || undefined,
      usuarioId: raw.usuarioId,
    };
    const msg = this.isEdit() ? 'Proprietário atualizado!' : 'Proprietário cadastrado!';
    const onOk = () => {
      this.toast.success(msg);
      this.voltar();
    };
    const onErr = () => {
      this.toast.error('Erro ao salvar.');
      this.saving.set(false);
    };
    if (this.isEdit()) {
      this.service.atualizar(this.editId!, dto).subscribe({ next: onOk, error: onErr });
    } else {
      this.service.criar(dto).subscribe({ next: onOk, error: onErr });
    }
  }

  voltar() {
    this.router.navigate(['/proprietarios']);
  }
}
