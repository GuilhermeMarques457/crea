import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ObraDto, CreateObraDto, StatusObra, TipoUsuario } from '../../shared/models/api.models';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class ObraService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/Obras`;
  private readonly auth = inject(AuthService);

  listarPorPermissaoUsuario() {
    return this.auth.currentUser()?.tipoUsuario == TipoUsuario.ResponsavelTecnico
      ? this.minhas()
      : this.listar();
  }

  listar() {
    return this.http.get<ObraDto[]>(this.base);
  }
  obter(id: string) {
    return this.http.get<ObraDto>(`${this.base}/${id}`);
  }
  criar(dto: CreateObraDto) {
    return this.http.post<ObraDto>(this.base, dto);
  }
  atualizar(id: string, dto: CreateObraDto) {
    return this.http.put<void>(`${this.base}/${id}`, dto);
  }
  excluir(id: string) {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
  porStatus(status: StatusObra) {
    return this.http.get<ObraDto[]>(`${this.base}/por-status/${status}`);
  }
  porProfissional(profissionalId: string) {
    return this.http.get<ObraDto[]>(`${this.base}/por-profissional/${profissionalId}`);
  }
  minhas() {
    return this.http.get<ObraDto[]>(`${this.base}/minhas`);
  }
  atualizarStatus(id: string, status: StatusObra) {
    return this.http.patch<void>(`${this.base}/${id}/status`, status);
  }
}
