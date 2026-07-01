import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-home-redirect',
  template: '',
})
export class HomeRedirectComponent implements OnInit {
  constructor(
    private readonly auth: AuthService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.router.navigateByUrl(this.auth.getHomeRoute());
  }
}
