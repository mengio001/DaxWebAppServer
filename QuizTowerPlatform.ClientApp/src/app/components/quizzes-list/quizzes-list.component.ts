import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, catchError, filter, Observable, of, Subject, switchMap, tap } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from '../../services/authentication.service';
import { AuthorizationService } from '../../services/authorization.service';
import { IQuizModel } from "../../types/quiz.classes.models";

@Component({
  selector: 'app-quizzes-list',
  templateUrl: './quizzes-list.component.html',
  styleUrl: './quizzes-list.component.scss'
})

export class QuizzesListComponent implements OnInit {
  private readonly quizzes = new BehaviorSubject<IQuizModel[]>([]);
  public readonly quizzes$: Observable<IQuizModel[]> = this.quizzes;

  private readonly errors = new BehaviorSubject<string>('');
  public readonly error$: Observable<string> = this.errors;

  public authenticated$ = this.auth.getIsAuthenticated();
  public anonymous$ = this.auth.getIsAnonymous();
  public canDeleteQuiz$ = this.authz.canDeleteQuiz();
  public canCreateQuiz$ = this.authz.canCreateQuiz();
  public quizId: string = '';

  public constructor(private http: HttpClient, private auth: AuthenticationService, private authz: AuthorizationService, private router: Router) { }

  public ngOnInit(): void {
    this.authenticated$
      .pipe(
        filter(isAuthenticated => isAuthenticated),
        tap(() => {
          this.fetchQuizzes();
        })
      ).subscribe();
  }

  public startQuiz(id: number): void {
    this.router.navigate([`/start-quiz/${id}`]);
  }

  public deleteQuiz(id: number): void {
    this.http.delete(`/api/Quiz/DeleteQuiz/${id}`)
      .pipe(catchError(this.showError))
      .subscribe(() => {
        const delQuiz = this.quizzes.getValue().filter((x) => x.Id !== id);
        this.quizzes.next(delQuiz);
      });
  }

  private fetchQuizzes(): void {
    this.http
      .get<IQuizModel[]>('/api/Quiz/GetQuizzes')
      .pipe(catchError(this.showError))
      .subscribe((osQuizzes) => {
        this.quizzes.next(osQuizzes);
      });
  }

  private readonly showError = (err: HttpErrorResponse) => {
    if (err.status !== 401) {
      this.errors.next(err.message);
    }
    throw err;
  }

  navigateToQuizDetails(id: number) {
    this.router.navigate([`/super-user/q/${id}`]);
  }

  goToAddQuestions(quizId: number, quizName: string): void {
    this.router.navigate(['/add-questions', quizId], {
      queryParams: { name: quizName }
    });
  }
}