import { NgClass } from '@angular/common';
import { Component, signal, inject, computed, OnInit, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { filter } from 'rxjs/operators';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatBadgeModule } from '@angular/material/badge';
import { TipoUsuario } from '../../shared/models/api.models';
import { AuthService } from '../../core/services/auth.service';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  roles?: TipoUsuario[];
  badge?: boolean;
}

const NAV_ITEMS: NavItem[] = [];

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

  readonly mobileMenuOpen = signal(false);

  userName = computed(() => this.auth.currentUser()?.nome ?? '');
  userEmail = computed(() => this.auth.currentUser()?.email ?? '');
  userInitial = computed(() => this.userName().charAt(0).toUpperCase());
  userType = computed(() => this.auth.currentUser()?.tipoUsuario);

  visibleItems = computed(() =>
    NAV_ITEMS.filter((i) => !i.roles || i.roles.includes(this.userType()!)),
  );

  ngOnInit() {
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
