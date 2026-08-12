import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import {
  LoginDto,
  LoginResponseDto,
  EsqueciSenhaDto,
  TrocarSenhaDto,
  CreateUsuarioDto,
  UsuarioDto,
} from '../../shared/models/api.models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly _session = signal<LoginResponseDto | null>(this.loadSession());

  readonly session = this._session.asReadonly();
  readonly isAuthenticated = computed(() => !!this._session());
  readonly currentUser = computed(() => this._session());

  esqueciSenha(dto: EsqueciSenhaDto) {
    return this.http.post<{ mensagem: string }>(
      `${environment.apiUrl}/api/Auth/esqueci-senha`,
      dto,
    );
  }

  trocarSenha(dto: TrocarSenhaDto) {
    return this.http.post<{ mensagem: string }>(
      `${environment.apiUrl}/api/Auth/trocar-senha`,
      dto,
    );
  }

  login(dto: LoginDto) {
    return this.http.post<LoginResponseDto>(`${environment.apiUrl}/api/Auth/login`, dto).pipe(
      tap((res) => {
        localStorage.setItem('session', JSON.stringify(res));
        this._session.set(res);
      }),
    );
  }

  registrar(dto: CreateUsuarioDto) {
    return this.http.post<UsuarioDto>(`${environment.apiUrl}/api/Auth/registrar`, dto);
  }

  getPerfil() {
    return this.http.get<UsuarioDto>(`${environment.apiUrl}/api/Auth/perfil`);
  }

  logout() {
    localStorage.removeItem('session');
    this._session.set(null);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return this._session()?.token ?? null;
  }

  private loadSession(): LoginResponseDto | null {
    try {
      const raw = localStorage.getItem('session');
      return raw ? JSON.parse(raw) : null;
    } catch {
      return null;
    }
  }
}
