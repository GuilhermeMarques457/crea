import { Component, signal, inject, OnInit, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ObraService } from '../../core/services/obra.service';
import {
  ObraDto,
  StatusObra,
  STATUS_OBRA_LABELS,
  TipoUsuario,
} from '../../shared/models/api.models';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';
import { AuthService } from '../../core/services/auth.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    PageHeaderComponent,
    StatusBadgeComponent,
    DatePipe,
  ],
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent implements OnInit {
  private readonly obraService = inject(ObraService);
  readonly auth = inject(AuthService);

  loading = signal(true);
  obras = signal<ObraDto[]>([]);
  userId = computed(() => this.auth.currentUser()?.usuarioId ?? '');

  recentObras = () => this.obras().slice(0, 5);

  stats = () => {
    const o = this.obras();
    return [
      {
        label: 'Total de Obras',
        value: o.length,
        icon: 'construction',
        bg: 'bg-blue-50',
        color: 'text-blue-600',
        border: 'border-blue-500',
      },
      {
        label: 'Em Andamento',
        value: o.filter((x) => x.status === StatusObra.EmAndamento).length,
        icon: 'pending',
        bg: 'bg-green-50',
        color: 'text-green-600',
        border: 'border-green-500',
      },
      {
        label: 'Pausadas',
        value: o.filter((x) => x.status === StatusObra.Pausada).length,
        icon: 'pause_circle',
        bg: 'bg-yellow-50',
        color: 'text-yellow-600',
        border: 'border-yellow-500',
      },
      {
        label: 'Concluídas',
        value: o.filter((x) => x.status === StatusObra.Concluida).length,
        icon: 'check_circle',
        bg: 'bg-purple-50',
        color: 'text-purple-600',
        border: 'border-purple-500',
      },
    ];
  };

  ngOnInit() {
    if (this.auth.currentUser()?.tipoUsuario == TipoUsuario.ResponsavelTecnico) {
      this.obraService.minhas().subscribe({
        next: (obras) => {
          this.obras.set(obras);
          this.loading.set(false);
        },
        error: () => this.loading.set(false),
      });
    } else {
      this.obraService.listar().subscribe({
        next: (obras) => {
          this.obras.set(obras);
          this.loading.set(false);
        },
        error: () => this.loading.set(false),
      });
    }
  }
}
