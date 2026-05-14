import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RelatorioObraDto } from '../../shared/models/api.models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RelatorioService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/Relatorios`;

  gerarRelatorioObra(obraId: string) {
    return this.http.get<RelatorioObraDto>(`${this.base}/obra/${obraId}`);
  }
}
