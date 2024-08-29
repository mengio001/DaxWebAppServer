import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';

@Injectable()
export class CsrfHeaderInterceptor implements HttpInterceptor {

  constructor(private cookieService: CookieService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const csrfToken = this.cookieService.get('CSRF-TOKEN');

    if (csrfToken) {
      request = request.clone({
        headers: request.headers.set("X-CSRF-TOKEN", csrfToken),
      });
    }
    
    return next.handle(request);
  }
}