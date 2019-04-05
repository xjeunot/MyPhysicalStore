import { Observable, from } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpInterceptor } from '@angular/common/http';
import { HttpRequest } from '@angular/common/http';
import { HttpHandler } from '@angular/common/http';
import { HttpEvent } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { AuthenticationService } from '../services/authentication.service';
import { environmentServer } from '../../../environments/environment'

@Injectable()
export class CustomTokenHttpInterceptor implements HttpInterceptor {

  constructor(private authService: AuthenticationService) {}

  public intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return from(this.handleAccess(request, next));
  }

  private async handleAccess(request: HttpRequest<any>, next: HttpHandler): Promise<HttpEvent<any>>
  {
    let token : string = await this.authService.getAccessToken();
    let changedRequest = request;

    // HttpHeader object immutable - copy values
    const headerSettings: {[name: string]: string | string[]; } = {};
    for (const key of request.headers.keys()) {
      headerSettings[key] = request.headers.getAll(key);
    }

    // Change Header for All is Not IdentityServer.
    let apiConfig = environmentServer.serverIdentity;
    if (request.url.indexOf(apiConfig) == -1) {
      headerSettings['Authorization'] = 'Bearer ' + token;
      headerSettings['Content-Type'] = 'application/json';
    }

    const newHeader = new HttpHeaders(headerSettings);

    changedRequest = request.clone({
      headers: newHeader});
      return next.handle(changedRequest).toPromise();
    }
  }