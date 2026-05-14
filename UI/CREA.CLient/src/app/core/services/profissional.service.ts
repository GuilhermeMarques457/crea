import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProfissionalDto, CreateProfissionalDto } from '../../shared/models/api.models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ProfissionalService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/Profissionais`;

  listar() {
    return this.http.get<ProfissionalDto[]>(this.base);
  }
  obter(id: string) {
    return this.http.get<ProfissionalDto>(`${this.base}/${id}`);
  }
  criar(dto: CreateProfissionalDto) {
    return this.http.post<ProfissionalDto>(this.base, dto);
  }
  atualizar(id: string, dto: CreateProfissionalDto) {
    return this.http.put<void>(`${this.base}/${id}`, dto);
  }
  excluir(id: string) {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
  porRegistro(numeroRegistro: string) {
    return this.http.get<ProfissionalDto>(`${this.base}/por-registro/${numeroRegistro}`);
  }
  porTipo(tipoRegistro: string) {
    return this.http.get<ProfissionalDto[]>(`${this.base}/por-tipo/${tipoRegistro}`);
  }
}
