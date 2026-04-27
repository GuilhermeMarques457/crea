import { Component, signal, inject, computed, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
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
  { label: 'Dashboard', icon: 'dashboard', route: '/dashboard' },
  { label: 'Obras', icon: 'construction', route: '/obras' },
  {
    label: 'Profissionais',
    icon: 'engineering',
    route: '/profissionais',
    roles: [TipoUsuario.Admin],
  },
  {
    label: 'Pendências',
    icon: 'pending_actions',
    route: '/pendencias',
    roles: [TipoUsuario.Admin, TipoUsuario.ResponsavelTecnico],
    badge: true,
  },
  { label: 'Relatórios', icon: 'assessment', route: '/relatorios' },
  { label: 'Usuários', icon: 'group', route: '/usuarios', roles: [TipoUsuario.Admin] },
  { label: 'Auditoria', icon: 'security', route: '/auditoria', roles: [TipoUsuario.Admin] },
];

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, MatIconModule, MatTooltipModule, MatBadgeModule],
  templateUrl: `./sidebar.component.html`,
})
export class SidebarComponent implements OnInit {
  private readonly auth = inject(AuthService);
  readonly notificacaoService = inject(NotificacaoService);

  userName = computed(() => this.auth.currentUser()?.nome ?? '');
  userEmail = computed(() => this.auth.currentUser()?.email ?? '');
  userInitial = computed(() => this.userName().charAt(0).toUpperCase());
  userType = computed(() => this.auth.currentUser()?.tipoUsuario);

  pendingCount = this.notificacaoService.count;

  visibleItems = computed(() =>
    NAV_ITEMS.filter((i) => !i.roles || i.roles.includes(this.userType()!)),
  );

  ngOnInit() {
    this.notificacaoService.carregarMeusPendentes();
  }

  logout() {
    this.auth.logout();
  }
}
