import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router'
import { BehaviorSubject, catchError, filter, Observable, of, Subject, switchMap, tap, combineLatest } from 'rxjs';
import { AuthenticationService } from '../../services/authentication.service';
import { IQuizModel, IQuestionModel } from "../../types/quiz.classes.models";
import { QuizRepositoryService } from '../../services/quiz-repository.service'

@Component({
  selector: 'app-add-questions',
  templateUrl: './add-questions.component.html',
  styleUrl: './add-questions.component.scss'
})
export class AddQuestionsComponent implements OnInit {
  private readonly questionList = new BehaviorSubject<IQuestionModel[]>([]);
  public readonly questionList$: Observable<IQuestionModel[]> = this.questionList;

  private readonly errors = new BehaviorSubject<string>('');
  public readonly error$: Observable<string> = this.errors;

  public authenticated$ = this.auth.getIsAuthenticated();
  public anonymous$ = this.auth.getIsAnonymous();

  public questionsForm: FormGroup;
  public saving: boolean = false;
  public isExpanded = false;
  public quizId: number = -1;
  public quizName: string = "";

  public constructor(private router: Router, private route: ActivatedRoute, private http: HttpClient, private auth: AuthenticationService, private quizRepository: QuizRepositoryService, private fb: FormBuilder) {
    this.questionsForm = this.fb.group({
      QuestionName: ['', Validators.required],
      FirstOption: ['', Validators.required],
      SecondOption: ['', Validators.required],
      ThirdOption: ['', Validators.required],
      FourthOption: ['', Validators.required],
      CorrectAnswer: ['', Validators.required],
      CorrectAnswerPoints: [0, Validators.required]
    });
  }

  public ngOnInit(): void {
    this.authenticated$
      .pipe(
        filter(isAuthenticated => isAuthenticated),
        tap(() => {
          this.get();
          this.isExpanded = true;
        })
      ).subscribe();
  }

  get() {
    combineLatest([
      this.route.paramMap,
      this.route.queryParams
    ]).subscribe(([paramMap, queryParams]) => {
      const id = paramMap.get('id');
      this.quizName = paramMap.get('name') ?? queryParams['name'] ?? '';
      if (id) {
        this.quizId = +id;
        this.fetchQuizById(this.quizId);
      }
    });
  }

  cancel(): void {
    this.questionsForm.reset();
    window.scrollTo(0, 0);
    this.saving = false;
    this.isExpanded = false
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  public addQuestion(quizQuestion: IQuestionModel): void {
    if (this.quizId === -1) {
      return;
    }

    this.saving = true;
    this.http
      .post<IQuestionModel>('/api/Question/AddQuestion', {
        Id: quizQuestion.Id,
        QuestionName: quizQuestion.QuestionName,
        FirstOption: quizQuestion.FirstOption,
        SecondOption: quizQuestion.SecondOption,
        ThirdOption: quizQuestion.ThirdOption,
        FourthOption: quizQuestion.FourthOption,
        CorrectAnswer: quizQuestion.CorrectAnswer,
        CorrectAnswerPoints: +quizQuestion.CorrectAnswerPoints,
        QuizId: this.quizId,
      })
      .pipe(catchError(this.showError))
      .subscribe({
        next: () => {
          this.fetchQuizById(this.quizId);
          this.questionsForm.reset();
          window.scrollTo(0, 0);
          this.saving = false;
        },
        error: (error) => {
          console.error('Error adding question:', error);
          this.saving = false;
        }
      });
  }

  private fetchQuizById(id: number): void {
    this.quizRepository.getQuizById(id).subscribe({
      next: (response: IQuizModel) => {
        this.questionList.next(response.QuizQuestions);
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
