import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { LogAuditoriaDto, PagedResult } from '../../shared/models/api.models';
import { environment } from '../../../environments/environment';

export interface AuditoriaFiltros {
  page: number;
  pageSize: number;
  entidade?: string;
  acao?: string;
  usuarioId?: string;
  inicio?: string;
  fim?: string;
}

@Injectable({ providedIn: 'root' })
export class AuditoriaService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/Auditoria`;

  getPaged(filtros: AuditoriaFiltros) {
    let params = new HttpParams().set('page', filtros.page).set('pageSize', filtros.pageSize);

    if (filtros.entidade) params = params.set('entidade', filtros.entidade);
    if (filtros.acao) params = params.set('acao', filtros.acao);
    if (filtros.usuarioId) params = params.set('usuarioId', filtros.usuarioId);
    if (filtros.inicio) params = params.set('inicio', filtros.inicio);
    if (filtros.fim) params = params.set('fim', filtros.fim);

    return this.http.get<PagedResult<LogAuditoriaDto>>(this.base, { params });
  }
}
