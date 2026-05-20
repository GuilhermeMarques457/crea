import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  AssinaturaDto,
  CreateAssinaturaDto,
  MinhaAssinaturaDto,
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

  minhas() {
    return this.http.get<MinhaAssinaturaDto[]>(`${this.base}/minhas`);
  }

  porEntidade(tipoEntidade: TipoEntidadeAssinatura, entidadeId: string) {
    const params = new HttpParams().set('tipoEntidade', tipoEntidade).set('entidadeId', entidadeId);
    return this.http.get<AssinaturaDto[]>(`${this.base}/por-entidade`, { params });
  }

  assinar(dto: CreateAssinaturaDto) {
    const form = new FormData();
    form.append('tipoEntidade', String(dto.tipoEntidade));
    form.append('entidadeId', dto.entidadeId);
    form.append('imagemAssinatura', dto.imagemAssinatura);
    if (dto.navegador) form.append('navegador', dto.navegador);
    if (dto.sistemaOperacional) form.append('sistemaOperacional', dto.sistemaOperacional);
    if (dto.dispositivo) form.append('dispositivo', dto.dispositivo);
    return this.http.post<AssinaturaDto>(this.base, form);
  }
}
