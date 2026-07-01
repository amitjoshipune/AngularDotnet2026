import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-testerhome',
  templateUrl: './testerhome.component.html',
  styleUrls: ['./testerhome.component.css'],
})
export class TesterhomeComponent {
  constructor(
    readonly auth: AuthService,
    private readonly router: Router
  ) {}

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
