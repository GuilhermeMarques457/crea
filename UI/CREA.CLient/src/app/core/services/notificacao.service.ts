import { Injectable, signal, inject } from '@angular/core';
import { TermoConclusaoService } from './termo-conclusao.service';
import { TermoConclusaoDto } from '../../shared/models/api.models';
import { RegistroDiarioService } from './registro-diario.service';
import { forkJoin } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class NotificacaoService {
  private readonly termoService = inject(TermoConclusaoService);
  private readonly registroService = inject(RegistroDiarioService);

  pendentes = signal<TermoConclusaoDto[]>([]);
  count = signal(0);

  carregarMeusPendentes() {
    forkJoin({
      termos: this.termoService.meusPendentes(),
      registros: this.registroService.pendentesAssinatura(),
    }).subscribe({
      next: ({ termos, registros }) => {
        this.pendentes.set(termos);

        const total = termos.length + registros.length;
        this.count.set(total);
      },
    });
  }
}
