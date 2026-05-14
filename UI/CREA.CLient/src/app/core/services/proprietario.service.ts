import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProprietarioDto, CreateProprietarioDto } from '../../shared/models/api.models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ProprietarioService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/Proprietarios`;

  listar() {
    return this.http.get<ProprietarioDto[]>(this.base);
  }
  obter(id: string) {
    return this.http.get<ProprietarioDto>(`${this.base}/${id}`);
  }
  criar(dto: CreateProprietarioDto) {
    return this.http.post<ProprietarioDto>(this.base, dto);
  }
  atualizar(id: string, dto: CreateProprietarioDto) {
    return this.http.put<void>(`${this.base}/${id}`, dto);
  }
  excluir(id: string) {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
