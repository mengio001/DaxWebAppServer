import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, catchError, filter, Observable, of, Subject, switchMap, tap } from 'rxjs';
import { Router } from '@angular/router';
import { AuthenticationService } from '../../services/authentication.service';

@Component({
  selector: 'app-quizzes-list',
  templateUrl: './quizzes-list.component.html',
  styleUrl: './quizzes-list.component.scss'
})

export class QuizzesListComponent implements OnInit {
  private readonly quizzes = new BehaviorSubject<Quiz[]>([]);
  public readonly quizzes$: Observable<Quiz[]> = this.quizzes;

  private readonly errors = new BehaviorSubject<string>('');
  public readonly error$: Observable<string> = this.errors;

  public authenticated$ = this.auth.getIsAuthenticated();
  public anonymous$ = this.auth.getIsAnonymous();

  public date = (new Date()).toISOString().split('T')[0];
  public name = "";  

  public constructor(private http: HttpClient, private auth: AuthenticationService, private router: Router) 
  {
  }

  public ngOnInit(): void {
    this.authenticated$
      .pipe(
        filter(isAuthenticated => isAuthenticated),
        tap(() => {
          this.fetchQuiz();
        })
    ).subscribe();
  }  

  public createQuiz(): void {
  }

  public deleteQuiz(id: number): void {
  }

  private fetchQuiz(): void {
  }

  private readonly showError = (err: HttpErrorResponse) => {
    if (err.status !== 401) {
      this.errors.next(err.message);
    }
    throw err;
  }

  navigateToQuizDetails(id: number) {
    this.router.navigate([`/quiz/${id}`]);
  }
}

interface Quiz {
  id: number;
  name: string;
  date: string;
  user: string;
}