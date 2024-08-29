import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router'
import { BehaviorSubject, catchError, filter, Observable, of, Subject, switchMap, tap, combineLatest } from 'rxjs';
import { AuthenticationService } from '../../services/authentication.service';
import { IQuizModel, IQuestionModel } from "../../types/quiz.classes.models";
import { QuizRepositoryService } from '../../services/quiz-repository.service'

@Component({
  selector: 'app-super-user-quiz',
  templateUrl: './super-user-quiz.component.html',
  styleUrl: './super-user-quiz.component.scss'
})

export class SuperUserQuizComponent implements OnInit {
  private readonly quizItem = new BehaviorSubject<IQuizModel>(<IQuizModel>{});
  public readonly quizItem$: Observable<IQuizModel> = this.quizItem;

  private readonly errors = new BehaviorSubject<string>('');
  public readonly error$: Observable<string> = this.errors;

  public authenticated$ = this.auth.getIsAuthenticated();
  public anonymous$ = this.auth.getIsAnonymous();

  public quizId: number = -1;
  public startTowerQuiz: boolean = false;

  constructor(private router: Router, private route: ActivatedRoute, private http: HttpClient, private auth: AuthenticationService, private quizRepository: QuizRepositoryService, private fb: FormBuilder) 
  {}

  public ngOnInit(): void {
    this.authenticated$
      .pipe(
        filter(isAuthenticated => isAuthenticated),
        tap(() => {
          this.get();
        })
      ).subscribe();
  }

  public get() {
    combineLatest([
      this.route.paramMap,
      this.route.queryParams
    ]).subscribe(([paramMap, queryParams]) => {
      const id = paramMap.get('id');
      this.startTowerQuiz = paramMap.get('start') ?? queryParams['start'] ?? '';
      if (id) {
        this.quizId = +id;
        this.fetchQuizById(this.quizId);
      }
    });
  }

  private fetchQuizById(id: number): void {
    this.quizRepository.getQuizById(id).subscribe({
      next: (response: IQuizModel) => {
        this.quizItem.next(response);
      },
      error: (error) => {
        console.error('Error fetching quiz:', error);
      },
      complete: () => {
        console.log('Quiz fetching complete');
      }
    });
  }

  private readonly showError = (err: HttpErrorResponse) => {
    if (err.status !== 401) {
      this.errors.next(err.message);
    }
    throw err;
  }
}