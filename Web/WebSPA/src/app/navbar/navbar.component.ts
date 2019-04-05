import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../shared/services/authentication.service'

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})

export class NavbarComponent implements OnInit {

  navbarOpen = false;
  isConnected = false;

  authenticationLabelName : string;

  constructor(private service: AuthenticationService) { }

  ngOnInit() {
    this.isConnected = this.service.isAuthentified();
    this.service.getAuthentifiedChangeEmitter()
      .subscribe(state => this.ConnectedChange(state));
    this.setupAuthenticationLabelName();
  }

  toggleNavbar() {
    this.navbarOpen = !this.navbarOpen;
  }

  ConnectedChange(state : string) {
    this.isConnected = (state == this.service.eventLoginOk);
    this.setupAuthenticationLabelName();
  }

  setupAuthenticationLabelName() {
    if (this.isConnected) {
      this.authenticationLabelName = "LogOff";
    }
    else {
      this.authenticationLabelName = "Login";
    }
  }
}
