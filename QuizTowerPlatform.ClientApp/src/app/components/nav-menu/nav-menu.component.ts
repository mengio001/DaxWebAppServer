import { Component } from '@angular/core';
import { AuthenticationService } from '../../services/authentication.service';

@Component({
  selector: 'ts-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrl: './nav-menu.component.scss'
})

export class NavMenuComponent {
  public username$ = this.auth.getUsername();
  public authenticated$ = this.auth.getIsAuthenticated();
  public anonymous$ = this.auth.getIsAnonymous();
  public logoutUrl$ = this.auth.getLogoutUrl();

  constructor(private auth: AuthenticationService) {
    auth.getSession();
  }
  
  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
