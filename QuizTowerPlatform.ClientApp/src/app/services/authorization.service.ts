import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of, shareReplay, switchMap, throwError } from 'rxjs';
import { AuthenticationService } from './authentication.service';

@Injectable({
  providedIn: 'root',
})
export class AuthorizationService {
  public username$ = this.auth.getUsername();
  public authenticated$ = this.auth.getIsAuthenticated();
  public anonymous$ = this.auth.getIsAnonymous();
  public logoutUrl$ = this.auth.getLogoutUrl();

  private roleClaimSubject = new BehaviorSubject<string>('');
  public roleClaim$ = this.roleClaimSubject.asObservable().pipe(shareReplay(1));

  constructor(private auth: AuthenticationService) {}

  private loadUserClaims(): Observable<void> {
    return this.auth.getSession({ ignoreCache: false }).pipe(
      map((session) => {
        if (session && Array.isArray(session)) {
          const roleClaim = session.find((claim) => claim.type === 'role');
          const role = roleClaim ? roleClaim.value : '';
          this.roleClaimSubject.next(role);
        } else {
          this.roleClaimSubject.next('');
        }
      }),
      catchError((error) => {
        console.error('Error in loadUserClaims:', error);
        this.roleClaimSubject.next('');
        return throwError(() => error);
      })
    );
  }

  private getUserClaims(): Observable<void> {
    return this.roleClaimSubject.getValue()
      ? of(undefined)
      : this.loadUserClaims(); // Note: load claims if not loaded yet
  }

  canReadQuizDetails(): Observable<boolean> {
    return this.getUserClaims().pipe(
      switchMap(() => this.roleClaim$),
      map((role) => {
        return role === 'PayingUser';
      }),
      catchError((error) => {
        console.error('Error in canReadQuizDetails:', error);
        return of(false);
      })
    );
  }

  canCreateQuiz(): Observable<boolean> {
    return this.getUserClaims().pipe(
      switchMap(() => this.roleClaim$),
      map((role) => {
        return role === 'QuizMaster';
      }),
      catchError((error) => {
        console.error('Error in canCreateQuiz:', error);
        return of(false);
      })
    );
  }
}
