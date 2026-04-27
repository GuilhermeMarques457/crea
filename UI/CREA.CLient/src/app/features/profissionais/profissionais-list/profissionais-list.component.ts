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
import { ProfissionalService } from '../../../core/services/profissional.service';
import { ProfissionalDto } from '../../../shared/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ToastService } from '../../../core/services/toast.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-profissionais-list',
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
  templateUrl: `./profissionais-list.component.html`,
})
export class ProfissionaisListComponent implements OnInit {
  private readonly service = inject(ProfissionalService);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);

  loading = signal(true);
  profissionais = signal<ProfissionalDto[]>([]);
  search = '';
  columns = ['nome', 'registro', 'especialidade', 'status', 'acoes'];

  filtered = () => {
    const q = this.search.toLowerCase();
    return !q
      ? this.profissionais()
      : this.profissionais().filter(
          (p) =>
            p.nome.toLowerCase().includes(q) ||
            p.cpf.includes(q) ||
            p.numeroRegistro.toLowerCase().includes(q),
        );
  };

  ngOnInit() {
    this.service.listar().subscribe({
      next: (list) => {
        this.profissionais.set(list);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  confirmarExcluir(p: ProfissionalDto) {
    this.dialog
      .open(ConfirmDialogComponent, {
        data: {
          title: 'Excluir Profissional',
          message: `Excluir "${p.nome}"?`,
          confirmLabel: 'Excluir',
        },
      })
      .afterClosed()
      .subscribe((ok) => {
        if (!ok) return;
        this.service.excluir(p.id).subscribe({
          next: () => {
            this.profissionais.update((l) => l.filter((x) => x.id !== p.id));
            this.toast.success('Profissional excluído.');
          },
          error: () => this.toast.error('Erro ao excluir.'),
        });
      });
  }
}
