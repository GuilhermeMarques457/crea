import { Component, OnInit, inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AnexoService } from '../../../core/services/anexo.service';
import { AnexoDto } from '../../models/api.models';

export interface EntidadeAnexosDialogData {
  tipo: 'registro';
  entidadeId: string;
  titulo: string;
}

@Component({
  selector: 'app-entidade-anexos-dialog',
  standalone: true,
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
  ],
  templateUrl: './entidade-anexos-dialog.component.html',
})
export class EntidadeAnexosDialogComponent implements OnInit {
  readonly data = inject<EntidadeAnexosDialogData>(MAT_DIALOG_DATA);
  private readonly anexoService = inject(AnexoService);

  readonly loading = signal(true);
  readonly anexos = signal<AnexoDto[]>([]);

  ngOnInit() {
    const req = this.anexoService.porRegistro(this.data.entidadeId)
    req.subscribe({
      next: (list) => {
        this.anexos.set(list);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  tamanhoKb(bytes: number): string {
    return (bytes / 1024).toFixed(1);
  }
}
