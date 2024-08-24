import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, filter, map, Observable, of, shareReplay} from 'rxjs';
import { UserClaim } from '../types/userClaim';

const ANONYMOUS: Session = null;
const CACHE_SIZE = 1;

@Injectable({
  providedIn: 'root',
})

export class AuthenticationService {
  private session$: Observable<Session> | null = null;

  constructor(private http: HttpClient) {}

  public getSession(options: { ignoreCache?: boolean } = {}): Observable<Session> {
    const { ignoreCache = false } = options; 
  
    if (!this.session$ || ignoreCache) {
      this.session$ = this.http.get<Session>('/account/user?slide=true').pipe(
        catchError((err) => {
          return of(ANONYMOUS);
        }),
        shareReplay(CACHE_SIZE)
      );
    }

    return this.session$;
  }

  public getIsAuthenticated(ignoreCache: boolean = false) {
    return this.getSession({ ignoreCache: ignoreCache }).pipe(map(UserIsAuthenticated));
  }

  public getIsAnonymous(ignoreCache: boolean = false) {
    return this.getSession({ ignoreCache: ignoreCache }).pipe(map(UserIsAnonymous));
  }

  public getUsername(ignoreCache: boolean = false) {
    return this.getSession({ ignoreCache: ignoreCache }).pipe(
      filter(UserIsAuthenticated),
      map((s) => s.find((c) => c.type === 'given_name')?.value)
    );
  }

  public getLogoutUrl(ignoreCache: boolean = false) {
    return this.getSession({ ignoreCache: ignoreCache }).pipe(
      filter(UserIsAuthenticated),
      map(s => s.find(c => c.type === 'bff:logout_url')?.value)
    );
  }
}

export type Session = UserClaim[] | null;

function UserIsAuthenticated(s: Session): s is UserClaim[] {
  return s !== null;
}

function UserIsAnonymous(s: Session): s is null {
  return s === null;
}