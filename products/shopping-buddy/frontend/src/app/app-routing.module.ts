import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { VerifyEmailComponent } from './components/auth/verify-email/verify-email.component';
import { ForgotPasswordComponent } from './components/auth/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './components/auth/reset-password/reset-password.component';
import { ForgotLoginIdComponent } from './components/auth/forgot-login-id/forgot-login-id.component';
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
  { path: 'register', component: RegisterComponent, canActivate: [LoginGuard] },
  { path: 'verify-email', component: VerifyEmailComponent, canActivate: [LoginGuard] },
  { path: 'forgot-password', component: ForgotPasswordComponent, canActivate: [LoginGuard] },
  { path: 'reset-password', component: ResetPasswordComponent, canActivate: [LoginGuard] },
  { path: 'forgot-login-id', component: ForgotLoginIdComponent, canActivate: [LoginGuard] },
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
