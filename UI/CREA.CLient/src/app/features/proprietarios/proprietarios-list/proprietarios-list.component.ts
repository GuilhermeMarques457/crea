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
import { MatDialog } from '@angular/material/dialog';
import { ProprietarioService } from '../../../core/services/proprietario.service';
import { ProprietarioDto } from '../../../shared/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-proprietarios-list',
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
    PageHeaderComponent,
    EmptyStateComponent,
  ],
  templateUrl: `./proprietarios-list.component.html`,
})
export class ProprietariosListComponent implements OnInit {
  private readonly service = inject(ProprietarioService);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);
  private readonly breakpoint = inject(BreakpointObserver);

  private readonly isNarrow = toSignal(
    this.breakpoint.observe('(max-width: 767px)').pipe(map((r) => r.matches)),
    { initialValue: false },
  );

  loading = signal(true);
  proprietarios = signal<ProprietarioDto[]>([]);
  search = '';

  displayedColumns = computed(() =>
    this.isNarrow()
      ? ['nome', 'status', 'acoes']
      : ['nome', 'documento', 'contato', 'status', 'acoes'],
  );

  filtered = () => {
    const q = this.search.toLowerCase();
    return !q
      ? this.proprietarios()
      : this.proprietarios().filter(
          (p) =>
            p.nome.toLowerCase().includes(q) ||
            p.cpf.toLowerCase().includes(q) ||
            p.email.toLowerCase().includes(q) ||
            p.telefone.toLowerCase().includes(q),
        );
  };

  ngOnInit() {
    this.service.listar().subscribe({
      next: (list: ProprietarioDto[]) => {
        this.proprietarios.set(list);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  confirmarExcluir(p: ProprietarioDto) {
    this.dialog
      .open(ConfirmDialogComponent, {
        data: {
          title: 'Excluir Proprietário',
          message: `Excluir "${p.nome}"?`,
          confirmLabel: 'Excluir',
        },
      })
      .afterClosed()
      .subscribe((ok) => {
        if (!ok) return;
        this.service.excluir(p.id).subscribe({
          next: () => {
            this.proprietarios.update((l) => l.filter((x) => x.id !== p.id));
            this.toast.success('Proprietário excluído.');
          },
          error: () => this.toast.error('Erro ao excluir.'),
        });
      });
  }
}
