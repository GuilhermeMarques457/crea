import { Component, signal, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { ObraService } from '../../../core/services/obra.service';
import {
  ObraDto,
  StatusObra,
  STATUS_OBRA_LABELS,
  TIPO_OBRA_LABELS,
} from '../../../shared/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-obras-list',
  standalone: true,
  imports: [
    RouterLink,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatMenuModule,
    DatePipe,
    PageHeaderComponent,
    StatusBadgeComponent,
    EmptyStateComponent,
  ],
  templateUrl: `./obra-list.component.html`,
})
export class ObrasListComponent implements OnInit {
  private readonly obraService = inject(ObraService);
  private readonly dialog = inject(MatDialog);
  private readonly toast = inject(ToastService);

  loading = signal(true);
  obras = signal<ObraDto[]>([]);
  search = '';
  filterStatus: StatusObra | null = null;

  statusOptions = Object.entries(STATUS_OBRA_LABELS).map(([v, l]) => ({
    value: Number(v) as StatusObra,
    label: l,
  }));

  filtered = () =>
    this.obras().filter((o) => {
      const q = this.search.toLowerCase();
      const matchSearch =
        !q ||
        o.nome.toLowerCase().includes(q) ||
        o.cidade.toLowerCase().includes(q) ||
        o.nomeProprietario.toLowerCase().includes(q);
      const matchStatus = this.filterStatus === null || o.status === this.filterStatus;
      return matchSearch && matchStatus;
    });

  tipoLabel = (obra: ObraDto) => TIPO_OBRA_LABELS[obra.tipoObra];

  ngOnInit() {
    this.obraService.listarPorPermissaoUsuario().subscribe({
      next: (obras) => {
        this.obras.set(obras);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  confirmarExcluir(obra: ObraDto) {
    this.dialog
      .open(ConfirmDialogComponent, {
        data: {
          title: 'Excluir Obra',
          message: `Deseja excluir a obra "${obra.nome}"? Esta ação não pode ser desfeita.`,
          confirmLabel: 'Excluir',
        },
      })
      .afterClosed()
      .subscribe((ok) => {
        if (!ok) return;
        this.obraService.excluir(obra.id).subscribe({
          next: () => {
            this.obras.update((l) => l.filter((x) => x.id !== obra.id));
            this.toast.success('Obra excluída.');
          },
          error: () => this.toast.error('Erro ao excluir obra.'),
        });
      });
  }
}
