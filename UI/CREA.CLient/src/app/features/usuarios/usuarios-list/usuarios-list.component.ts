import { BreakpointObserver } from '@angular/cdk/layout';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs/operators';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { forkJoin } from 'rxjs';
import { UsuarioService } from '../../../core/services/usuario.service';
import { ProprietarioService } from '../../../core/services/proprietario.service';
import { UsuarioDto, TIPO_USUARIO_LABELS, TipoUsuario } from '../../../shared/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-usuarios-list',
  standalone: true,
  imports: [
    RouterLink,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatProgressSpinnerModule,
    MatMenuModule,
    MatTableModule,
    MatTooltipModule,
    DatePipe,
    PageHeaderComponent,
    EmptyStateComponent,
  ],
  templateUrl: `./usuarios-list.component.html`,
})
export class UsuariosListComponent implements OnInit {
  private readonly service = inject(UsuarioService);
  private readonly proprietarioService = inject(ProprietarioService);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);
  private readonly breakpoint = inject(BreakpointObserver);

  private readonly isNarrow = toSignal(
    this.breakpoint.observe('(max-width: 767px)').pipe(map((r) => r.matches)),
    { initialValue: false },
  );

  loading = signal(true);
  usuarios = signal<UsuarioDto[]>([]);
  /** IDs de usuários do tipo Proprietário que ainda não têm cadastro de proprietário vinculado */
  semCadastroIds = signal<Set<string>>(new Set());
  search = '';

  displayedColumns = computed(() =>
    this.isNarrow() ? ['nome', 'status', 'acoes'] : ['nome', 'tipo', 'status', 'criadoEm', 'acoes'],
  );

  tipoLabel = (u: UsuarioDto) => TIPO_USUARIO_LABELS[u.tipoUsuario];

  semCadastroCount = computed(
    () =>
      this.usuarios().filter(
        (u) => u.tipoUsuario === TipoUsuario.Proprietario && this.semCadastroIds().has(u.id),
      ).length,
  );

  semCadastro = (u: UsuarioDto) =>
    u.tipoUsuario === TipoUsuario.Proprietario && this.semCadastroIds().has(u.id);

  filtered = () => {
    const q = this.search.toLowerCase();
    return !q
      ? this.usuarios()
      : this.usuarios().filter(
          (u) => u.nome.toLowerCase().includes(q) || u.email.toLowerCase().includes(q),
        );
  };

  ngOnInit() {
    forkJoin({
      usuarios: this.service.listar(),
      proprietarios: this.proprietarioService.listar(),
    }).subscribe({
      next: ({ usuarios, proprietarios }) => {
        this.usuarios.set(usuarios);
        const vinculados = new Set(
          proprietarios.filter((p) => p.usuarioId).map((p) => p.usuarioId!),
        );
        const semCadastro = new Set(
          usuarios
            .filter((u) => u.tipoUsuario === TipoUsuario.Proprietario && !vinculados.has(u.id))
            .map((u) => u.id),
        );
        this.semCadastroIds.set(semCadastro);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  confirmarExcluir(u: UsuarioDto) {
    this.dialog
      .open(ConfirmDialogComponent, {
        data: {
          title: 'Excluir Usuário',
          message: `Excluir "${u.nome}"?`,
          confirmLabel: 'Excluir',
        },
      })
      .afterClosed()
      .subscribe((ok) => {
        if (!ok) return;
        this.service.excluir(u.id).subscribe({
          next: () => {
            this.usuarios.update((l) => l.filter((x) => x.id !== u.id));
            this.toast.success('Usuário excluído.');
          },
          error: () => this.toast.error('Erro ao excluir.'),
        });
      });
  }
}
