import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class RoleGuard implements CanActivate {
  constructor(
    private readonly auth: AuthService,
    private readonly router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean | UrlTree {
    const allowedRoles = route.data['roles'] as UserRole[] | undefined;

    if (!this.auth.isAuthenticated()) {
      return this.router.createUrlTree(['/login']);
    }

    if (!allowedRoles?.length) {
      return true;
    }

    const role = this.auth.getUserRole();
    if (role && allowedRoles.includes(role)) {
      return true;
    }

    return this.router.createUrlTree([this.auth.getHomeRoute()]);
  }
}
