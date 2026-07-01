import { Injectable } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { delay, mergeMap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { buildMockLoginResponse } from '../data/mock-auth.data';
import { LoginRequest } from '../models/auth.models';

/**
 * Simulates ASP.NET Core auth endpoints when `environment.useMockAuth` is true.
 * Disable mock mode and run `ng serve` with proxy.conf.json to hit a real .NET API on localhost:5180.
 */
@Injectable()
export class MockApiInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (!environment.useMockAuth || !req.url.includes('/api/auth/')) {
      return next.handle(req);
    }

    if (req.method === 'POST' && req.url.endsWith('/auth/login')) {
      const body = req.body as LoginRequest;
      const response = buildMockLoginResponse(body);

      if (!response) {
        return of(null).pipe(
          delay(400),
          mergeMap(() =>
            throwError(
              () =>
                new HttpErrorResponse({
                  status: 401,
                  error: { message: 'Invalid email or password.' },
                })
            )
          )
        );
      }

      return of(new HttpResponse({ status: 200, body: response })).pipe(delay(500));
    }

    return next.handle(req);
  }
}
