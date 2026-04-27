import { Component, signal, inject, OnInit } from '@angular/core';
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
import { MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { UsuarioService } from '../../../core/services/usuario.service';
import { UsuarioDto, TIPO_USUARIO_LABELS } from '../../../shared/models/api.models';
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
    DatePipe,
    PageHeaderComponent,
    EmptyStateComponent,
  ],
  templateUrl: `./usuarios-list.component.html`,
})
export class UsuariosListComponent implements OnInit {
  private readonly service = inject(UsuarioService);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);

  loading = signal(true);
  usuarios = signal<UsuarioDto[]>([]);
  search = '';
  columns = ['nome', 'tipo', 'status', 'criadoEm', 'acoes'];

  tipoLabel = (u: UsuarioDto) => TIPO_USUARIO_LABELS[u.tipoUsuario];

  filtered = () => {
    const q = this.search.toLowerCase();
    return !q
      ? this.usuarios()
      : this.usuarios().filter(
          (u) => u.nome.toLowerCase().includes(q) || u.email.toLowerCase().includes(q),
        );
  };

  ngOnInit() {
    this.service.listar().subscribe({
      next: (list) => {
        this.usuarios.set(list);
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
