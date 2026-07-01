import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login/login.component';
import { AdminhomeComponent } from './components/dashboard/adminhome/adminhome.component';
import { TesterhomeComponent } from './components/dashboard/testerhome/testerhome.component';
import { ManagerhomeComponent } from './components/dashboard/managerhome/managerhome.component';
import { UsersComponent } from './components/users/users.component';
import { AuthGuard } from './core/guards/auth.guard';
import { HomeRedirectGuard } from './core/guards/home-redirect.guard';
import { LoginGuard } from './core/guards/login.guard';
import { RoleGuard } from './core/guards/role.guard';
import { HomeRedirectComponent } from './components/home-redirect/home-redirect.component';
import { ItemsComponent } from './components/items/items.component';
import { ShoppingHomeComponent } from './components/shopping-buddy/shopping-home/shopping-home.component';
import { FindBuddyComponent } from './components/shopping-buddy/find-buddy/find-buddy.component';
import { BookBuddyComponent } from './components/shopping-buddy/book-buddy/book-buddy.component';
import { SafetyLegalComponent } from './components/shopping-buddy/safety-legal/safety-legal.component';

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
    path: 'admin',
    component: AdminhomeComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'tester',
    component: TesterhomeComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Tester'] },
  },
  {
    path: 'manager',
    component: ManagerhomeComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Manager'] },
  },
  {
    path: 'users-demo',
    component: UsersComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'items',
    component: ItemsComponent,
    canActivate: [AuthGuard],
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
