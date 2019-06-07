import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { PersistanceService } from '../services/persistance.service'
import { environment } from '../../../environments/environment'
import { AuthentificationToken } from '../models/authentification-token'
import { EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  public eventLoginOk = 'EventLoginOk';
  public eventLoginFail = 'EventLoginFail';
  public eventLogoff = 'EventLogoff';

  keyPersistanceToken : string = 'AuthentificationToken';
  keyPersistanceExpiration : string = 'AuthentificationExpirationDate';

  apiConfig = environment.identity;

  isAuthentifiedChange: EventEmitter<string> = new EventEmitter();

  constructor(private persister: PersistanceService, private http: HttpClient) {
  }

  public reset() {
    // Reset Token.
    let authentificationToken : AuthentificationToken = null;
    this.persister.set(this.keyPersistanceToken, authentificationToken);

    // Reset Expiration Date.
    let expirationDate = new Date();
    this.persister.set(this.keyPersistanceExpiration, expirationDate);
  }

  public isAuthentified() : boolean {
    let authentificationToken : AuthentificationToken = this.persister.get(this.keyPersistanceToken);
    if (authentificationToken != null) {
      let expirationDateString : string = this.persister.get(this.keyPersistanceExpiration);
      var currentDate = new Date();
      var expirationDate : Date = new Date(expirationDateString);
      return ((expirationDate != null) && (expirationDate.getTime() > currentDate.getTime()))
    }
  }

  public getAccessToken() : string {
    let authentificationToken : AuthentificationToken = this.persister.get(this.keyPersistanceToken);

    // In case Token is expired.
    if ( ! this.isAuthentified()) {
      this.reset();
      return null;
    }

    // Return Token.
    return authentificationToken.access_token;
  }

  public login(username : string, password : string) : void {
    // Prepare Object Token.
    let authentificationToken : AuthentificationToken = null;

    // ByPass if is authentified.
    if (this.isAuthentified()) return;

    // Prepare Header.
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type':  'application/x-www-form-urlencoded'
      })
    };

    // Prepare Args.
    let authorizationUrl = this.apiConfig.token;
    let grant_type = 'password';
    let client_id = 'client';
    let client_password = "secret";
    let scope = 'customer store';

    // Prepare URL.
    let postMessage = 'grant_type=' + grant_type +
      '&client_id='+ client_id +
      '&client_secret=' + client_password + 
      '&username='+ username +
      '&password=' + password +
      '&scope=' + scope;
    
    // Call.
    this.http.post<AuthentificationToken>(authorizationUrl, postMessage, httpOptions)
      .toPromise()
      .then(response => {

        authentificationToken = response;

        this.reset();

        let currentDate = new Date();
        let expirationDate = new Date(currentDate.getTime() + 1000 * authentificationToken.expires_in);
        console.log("Token expire in " + expirationDate);
  
        this.persister.set(this.keyPersistanceToken, authentificationToken);
        this.persister.set(this.keyPersistanceExpiration, expirationDate);

        this.emitAuthentifiedChangeEvent(this.eventLoginOk);
      })
      .catch((error: any) => {
        console.log(error);
        this.emitAuthentifiedChangeEvent(this.eventLoginFail);
      });
  }

  public logoff() {
    // Prepare Header.
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type':  'application/x-www-form-urlencoded'
      })
    };

    // Prepare Args.
    let authorizationUrl = this.apiConfig.revoke;
    let token = this.getAccessToken();

    // Prepare URL.
    let postMessage = 'token=' + token;

    // Call.
    this.http.post(authorizationUrl, postMessage, httpOptions)
      .subscribe(
        (data: any) => console.log(data),
        (error : any) => console.log(error)
      );
      
    // Reset Internal Token.
    this.reset();

    // Emit Event.
    this.emitAuthentifiedChangeEvent(this.eventLogoff);
  }

  emitAuthentifiedChangeEvent(eventType : string) {
    this.isAuthentifiedChange.emit(eventType);
  }

  getAuthentifiedChangeEmitter() {
    return this.isAuthentifiedChange;
  }
}
