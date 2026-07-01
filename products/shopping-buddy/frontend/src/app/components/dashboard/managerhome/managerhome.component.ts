import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-managerhome',
  templateUrl: './managerhome.component.html',
  styleUrls: ['./managerhome.component.css'],
})
export class ManagerhomeComponent {
  constructor(readonly auth: AuthService, private readonly router: Router) {}

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
