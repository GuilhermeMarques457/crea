import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  AssinaturaDto,
  CreateAssinaturaDto,
  PendenteAssinaturaDto,
  TipoEntidadeAssinatura,
} from '../../shared/models/api.models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AssinaturaService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/Assinaturas`;

  pendentes() {
    return this.http.get<PendenteAssinaturaDto[]>(`${this.base}/pendentes`);
  }

  porEntidade(tipoEntidade: TipoEntidadeAssinatura, entidadeId: string) {
    const params = new HttpParams()
      .set('tipoEntidade', tipoEntidade)
      .set('entidadeId', entidadeId);
    return this.http.get<AssinaturaDto[]>(`${this.base}/por-entidade`, { params });
  }

  assinar(dto: CreateAssinaturaDto) {
    return this.http.post<AssinaturaDto>(this.base, dto);
  }
}
