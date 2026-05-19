import { Injectable, signal, inject } from '@angular/core';
import { AssinaturaService } from './assinatura.service';

@Injectable({ providedIn: 'root' })
export class NotificacaoService {
  private readonly assinaturaService = inject(AssinaturaService);

  count = signal(0);

  carregarMeusPendentes() {
    this.assinaturaService.pendentes().subscribe({
      next: (list) => this.count.set(list.length),
      error: () => this.count.set(0),
    });
  }
}
