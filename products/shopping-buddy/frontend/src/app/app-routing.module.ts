import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login/login.component';
import { AdminhomeComponent } from './components/dashboard/adminhome/adminhome.component';
import { AuthGuard } from './core/guards/auth.guard';
import { HomeRedirectGuard } from './core/guards/home-redirect.guard';
import { LoginGuard } from './core/guards/login.guard';
import { RoleGuard } from './core/guards/role.guard';
import { HomeRedirectComponent } from './components/home-redirect/home-redirect.component';
import { ShoppingHomeComponent } from './components/shopping-buddy/shopping-home/shopping-home.component';
import { FindBuddyComponent } from './components/shopping-buddy/find-buddy/find-buddy.component';
import { BookBuddyComponent } from './components/shopping-buddy/book-buddy/book-buddy.component';
import { SafetyLegalComponent } from './components/shopping-buddy/safety-legal/safety-legal.component';
import { MyBookingsComponent } from './components/shopping-buddy/my-bookings/my-bookings.component';
import { BuddyIncomingComponent } from './components/shopping-buddy/buddy-incoming/buddy-incoming.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent, canActivate: [LoginGuard] },
  { path: 'shopping/safety', component: SafetyLegalComponent },
  {
    path: 'shopping',
    component: ShoppingHomeComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'shopping/find',
    component: FindBuddyComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'shopping/book/:id',
    component: BookBuddyComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'shopping/my-bookings',
    component: MyBookingsComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'shopping/buddy-requests',
    component: BuddyIncomingComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'admin',
    component: AdminhomeComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: '',
    pathMatch: 'full',
    component: HomeRedirectComponent,
    canActivate: [AuthGuard, HomeRedirectGuard],
  },
  {
    path: '**',
    component: HomeRedirectComponent,
    canActivate: [AuthGuard, HomeRedirectGuard],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
