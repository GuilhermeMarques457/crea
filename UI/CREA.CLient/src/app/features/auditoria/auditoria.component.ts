import { Component, signal, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import {
  AuditoriaDetalhesDialogComponent,
  AuditoriaDetalhesData,
} from './auditoria-detalhes/auditoria-detalhes-dialog.component';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule } from '@angular/material/dialog';
import { DatePipe, SlicePipe } from '@angular/common';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { AuditoriaService } from '../../core/services/auditoria.service';
import { PagedResult, LogAuditoriaDto } from '../../shared/models/api.models';

@Component({
  selector: 'app-auditoria',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatTableModule,
    MatPaginatorModule,
    MatChipsModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    DatePipe,
    PageHeaderComponent,
    SlicePipe,
  ],
  templateUrl: './auditoria.component.html',
})
export class AuditoriaComponent implements OnInit {
  private readonly auditoriaService = inject(AuditoriaService);
  private readonly dialog = inject(MatDialog);
  private readonly fb = inject(FormBuilder);

  loading = signal(false);
  result = signal<PagedResult<LogAuditoriaDto> | null>(null);

  page = signal(1);
  pageSize = signal(20);

  displayedColumns = [
    'dataAcao',
    'nomeUsuario',
    'acao',
    'entidade',
    'entidadeId',
    'enderecoIp',
    'detalhes',
  ];

  entidades = [
    'Obra',
    'RegistroDiario',
    'Ocorrencia',
    'Profissional',
    'Usuario',
    'Anexo',
    'TermoConclusao',
    'AssinaturaDigital',
  ];
  acoes = ['Criação', 'Atualização', 'Exclusão'];

  form = this.fb.group({
    entidade: [''],
    acao: [''],
    inicio: [this.getTodayStart()],
    fim: [this.getTomorrowStart()],
  });

  private format(date: Date): string {
    const pad = (n: number) => n.toString().padStart(2, '0');

    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  }

  private getTodayStart(): string {
    const d = new Date();
    d.setHours(0, 0, 0, 0);
    return this.format(d);
  }

  private getTomorrowStart(): string {
    const d = new Date();
    d.setDate(d.getDate() + 1);
    d.setHours(0, 0, 0, 0);
    return this.format(d);
  }

  ngOnInit() {
    this.carregar();
  }

  carregar() {
    this.loading.set(true);
    const v = this.form.value;
    console.log(v.inicio);
    this.auditoriaService
      .getPaged({
        page: this.page(),
        pageSize: this.pageSize(),
        entidade: v.entidade || undefined,
        acao: v.acao || undefined,
        inicio: v.inicio || undefined,
        fim: v.fim || undefined,
      })
      .subscribe({
        next: (r) => {
          console.log(r);
          this.result.set(r);
          this.loading.set(false);
        },
        error: () => this.loading.set(false),
      });
  }

  filtrar() {
    this.page.set(1);
    this.carregar();
  }

  limparFiltros() {
    this.form.reset();
    this.page.set(1);
    this.carregar();
  }

  onPage(event: PageEvent) {
    this.page.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.carregar();
  }

  abrirDetalhes(log: LogAuditoriaDto) {
    this.dialog.open(AuditoriaDetalhesDialogComponent, {
      data: {
        dadosAntigos: log.dadosAntigos,
        dadosNovos: log.dadosNovos,
      } as AuditoriaDetalhesData,
      width: '700px',
    });
  }

  acaoColor(acao: string): string {
    switch (acao) {
      case 'Criação':
        return 'accent';
      case 'Exclusão':
        return 'warn';
      default:
        return 'primary';
    }
  }
}
