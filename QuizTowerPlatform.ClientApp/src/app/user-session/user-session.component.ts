import { Component } from '@angular/core';
import { AuthenticationService, Session } from '../services/authentication.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-user-session',
  templateUrl: './user-session.component.html',
  styleUrl: './user-session.component.scss'
})
export class UserSessionComponent {
  public session$: Observable<Session>;
  public isAuthenticated$: Observable<boolean>;
  public isAnonymous$: Observable<boolean>;

  constructor(auth: AuthenticationService) {
    this.session$ = auth.getSession();
    this.isAuthenticated$ = auth.getIsAuthenticated();
    this.isAnonymous$ = auth.getIsAnonymous();
  }
}
