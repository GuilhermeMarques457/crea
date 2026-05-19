import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AnexoDto } from '../../shared/models/api.models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AnexoService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/Anexos`;

  porObra(obraId: string) {
    return this.http.get<AnexoDto[]>(`${this.base}/por-obra/${obraId}`);
  }

  porRegistro(registroDiarioId: string) {
    return this.http.get<AnexoDto[]>(`${this.base}/por-registro/${registroDiarioId}`);
  }

  excluir(id: string) {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  upload(
    arquivo: File,
    params: { obraId?: string; registroDiarioId?: string } = {},
  ) {
    const formData = new FormData();
    formData.append('arquivo', arquivo);
    let query = '';
    if (params.obraId) query += `obraId=${params.obraId}&`;
    if (params.registroDiarioId) query += `registroDiarioId=${params.registroDiarioId}&`;
    return this.http.post<AnexoDto>(`${this.base}/upload?${query}`, formData);
  }
}
