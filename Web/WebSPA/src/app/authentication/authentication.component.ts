import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../shared/services/authentication.service'

@Component({
  selector: 'app-authentication',
  templateUrl: './authentication.component.html',
  styleUrls: ['./authentication.component.css']
})
export class AuthenticationComponent implements OnInit {

  isConnected : boolean;

  loginUser : string;
  loginPassword : string;
  errorMessage : string;

  constructor(private service: AuthenticationService) {
  }

  ngOnInit() {
    this.refreshState()
    this.service.getAuthentifiedChangeEmitter()
      .subscribe(state => this.logOnResult(state));
  }

  refreshState() {
    this.isConnected = this.service.isAuthentified();
    this.loginUser = '';
    this.loginPassword = '';
    this.errorMessage = '';
  }

  logOn() {
    this.service.login(this.loginUser, this.loginPassword)
  }

  logOnResult(state : string) {
    if (state == this.service.eventLoginFail) {
      this.errorMessage = "Echec de l'authentification";
    }
    this.isConnected = (state == this.service.eventLoginOk);
  }
  
  logOff() {
    this.service.logoff();
    this.refreshState();
  }
}
