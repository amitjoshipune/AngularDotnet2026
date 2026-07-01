import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';

/** Sends authenticated users from `/` to their role-specific home. */
@Injectable({ providedIn: 'root' })
export class HomeRedirectGuard implements CanActivate {
  constructor(
    private readonly auth: AuthService,
    private readonly router: Router
  ) {}

  canActivate(): UrlTree {
    return this.router.createUrlTree([this.auth.getHomeRoute()]);
  }
}
