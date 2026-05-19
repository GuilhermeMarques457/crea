import { NgClass } from '@angular/common';
import { Component, signal, inject, computed, OnInit, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { filter } from 'rxjs/operators';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatBadgeModule } from '@angular/material/badge';
import { AuthService } from '../../core/services/auth.service';
import { NotificacaoService } from '../../core/services/notificacao.service';
import { TipoUsuario } from '../../shared/models/api.models';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  roles?: TipoUsuario[];
  badge?: boolean;
}

const NAV_ITEMS: NavItem[] = [
  {
    label: 'Dashboard',
    icon: 'dashboard',
    route: '/dashboard',
    roles: [TipoUsuario.Admin, TipoUsuario.ResponsavelTecnico, TipoUsuario.Operacional],
  },
  {
    label: 'Obras',
    icon: 'construction',
    route: '/obras',
    roles: [
      TipoUsuario.Admin,
      TipoUsuario.ResponsavelTecnico,
      TipoUsuario.Operacional,
      TipoUsuario.UsuarioCrea,
    ],
  },
  {
    label: 'Profissionais',
    icon: 'engineering',
    route: '/profissionais',
    roles: [TipoUsuario.Admin],
  },
  {
    label: 'Proprietários',
    icon: 'person',
    route: '/proprietarios',
    roles: [TipoUsuario.Admin],
  },
  {
    label: 'Pendências',
    icon: 'pending_actions',
    route: '/pendencias',
    roles: [TipoUsuario.ResponsavelTecnico, TipoUsuario.UsuarioCrea, TipoUsuario.Proprietario],
    badge: true,
  },
  {
    label: 'Relatórios',
    icon: 'assessment',
    route: '/relatorios',
    roles: [TipoUsuario.Admin, TipoUsuario.ResponsavelTecnico, TipoUsuario.Operacional],
  },
  { label: 'Usuários', icon: 'group', route: '/usuarios', roles: [TipoUsuario.Admin] },
  { label: 'Auditoria', icon: 'security', route: '/auditoria', roles: [TipoUsuario.Admin] },
];

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [NgClass, RouterLink, RouterLinkActive, MatIconModule, MatTooltipModule, MatBadgeModule],
  templateUrl: `./sidebar.component.html`,
  host: {
    class: 'contents',
  },
})
export class SidebarComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  readonly notificacaoService = inject(NotificacaoService);

  readonly mobileMenuOpen = signal(false);

  userName = computed(() => this.auth.currentUser()?.nome ?? '');
  userEmail = computed(() => this.auth.currentUser()?.email ?? '');
  userInitial = computed(() => this.userName().charAt(0).toUpperCase());
  userType = computed(() => this.auth.currentUser()?.tipoUsuario);

  pendingCount = this.notificacaoService.count;

  visibleItems = computed(() =>
    NAV_ITEMS.filter((i) => !i.roles || i.roles.includes(this.userType()!)),
  );

  ngOnInit() {
    const tipo = this.userType();
    if (
      tipo === TipoUsuario.Proprietario ||
      tipo === TipoUsuario.UsuarioCrea
    ) {
      this.notificacaoService.carregarMeusPendentes();
    } else if (tipo !== undefined) {
      this.notificacaoService.carregarMeusPendentes();
    }

    this.router.events
      .pipe(
        filter((e): e is NavigationEnd => e instanceof NavigationEnd),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(() => this.mobileMenuOpen.set(false));
  }

  toggleMobileMenu(): void {
    this.mobileMenuOpen.update((v) => !v);
  }

  closeMobileMenu(): void {
    this.mobileMenuOpen.set(false);
  }

  logout() {
    this.auth.logout();
  }
}
