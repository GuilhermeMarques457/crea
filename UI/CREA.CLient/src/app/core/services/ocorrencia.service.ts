import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OcorrenciaDto, CreateOcorrenciaDto, TipoOcorrencia } from '../../shared/models/api.models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class OcorrenciaService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/Ocorrencias`;

  porObra(obraId: string) {
    return this.http.get<OcorrenciaDto[]>(`${this.base}/por-obra/${obraId}`);
  }
  porObraTipo(obraId: string, tipo: TipoOcorrencia) {
    return this.http.get<OcorrenciaDto[]>(`${this.base}/por-obra/${obraId}/tipo/${tipo}`);
  }
  obter(id: string) {
    return this.http.get<OcorrenciaDto>(`${this.base}/${id}`);
  }
  criar(dto: CreateOcorrenciaDto) {
    return this.http.post<OcorrenciaDto>(this.base, dto);
  }
  atualizar(id: string, dto: CreateOcorrenciaDto) {
    return this.http.put<void>(`${this.base}/${id}`, dto);
  }
  excluir(id: string) {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
