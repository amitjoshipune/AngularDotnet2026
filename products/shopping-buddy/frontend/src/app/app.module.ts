import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { Test1Component } from './components/rxjstest/test1/test1.component';
import { AdminhomeComponent } from './components/dashboard/adminhome/adminhome.component';
import { TesterhomeComponent } from './components/dashboard/testerhome/testerhome.component';
import { ManagerhomeComponent } from './components/dashboard/managerhome/managerhome.component';
import { GeneralmessageComponent } from './components/shared/generalmessage/generalmessage.component';
import { ErrormessageComponent } from './components/shared/errormessage/errormessage.component';
import { AlltestsComponent } from './components/tests/alltests/alltests.component';
import { ViewtestComponent } from './components/tests/viewtest/viewtest.component';
import { ViewresultsComponent } from './components/results/viewresults/viewresults.component';
import { ViewresultComponent } from './components/results/viewresult/viewresult.component';
import { MarkresultComponent } from './components/results/markresult/markresult.component';
import { TestprogressComponent } from './components/reports/admin/testprogress/testprogress.component';
import { QscoreComponent } from './components/reports/admin/qscore/qscore.component';
import { MytestsComponent } from './components/reports/tester/mytests/mytests.component';
import { MyresultsComponent } from './components/reports/tester/myresults/myresults.component';
import { MyaisComponent } from './components/reports/tester/myais/myais.component';
import { UserComponent } from './components/user/user.component';
import { UsersComponent } from './components/users/users.component';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { VerifyEmailComponent } from './components/auth/verify-email/verify-email.component';
import { ForgotPasswordComponent } from './components/auth/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './components/auth/reset-password/reset-password.component';
import { ForgotLoginIdComponent } from './components/auth/forgot-login-id/forgot-login-id.component';
import { HomeRedirectComponent } from './components/home-redirect/home-redirect.component';
import { ItemsComponent } from './components/items/items.component';
import { ShoppingHomeComponent } from './components/shopping-buddy/shopping-home/shopping-home.component';
import { FindBuddyComponent } from './components/shopping-buddy/find-buddy/find-buddy.component';
import { BookBuddyComponent } from './components/shopping-buddy/book-buddy/book-buddy.component';
import { SafetyLegalComponent } from './components/shopping-buddy/safety-legal/safety-legal.component';
import { MyBookingsComponent } from './components/shopping-buddy/my-bookings/my-bookings.component';
import { BuddyIncomingComponent } from './components/shopping-buddy/buddy-incoming/buddy-incoming.component';
import { JwtInterceptor } from './core/interceptors/jwt.interceptor';
import { MockApiInterceptor } from './core/interceptors/mock-api.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    Test1Component,
    AdminhomeComponent,
    TesterhomeComponent,
    ManagerhomeComponent,
    GeneralmessageComponent,
    ErrormessageComponent,
    AlltestsComponent,
    ViewtestComponent,
    ViewresultsComponent,
    ViewresultComponent,
    MarkresultComponent,
    TestprogressComponent,
    QscoreComponent,
    MytestsComponent,
    MyresultsComponent,
    MyaisComponent,
    UserComponent,
    UsersComponent,
    LoginComponent,
    RegisterComponent,
    VerifyEmailComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
    ForgotLoginIdComponent,
    HomeRedirectComponent,
    ItemsComponent,
    ShoppingHomeComponent,
    FindBuddyComponent,
    BookBuddyComponent,
    SafetyLegalComponent,
    MyBookingsComponent,
    BuddyIncomingComponent,
  ],
  imports: [BrowserModule, CommonModule, FormsModule, ReactiveFormsModule, HttpClientModule, AppRoutingModule],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: MockApiInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
