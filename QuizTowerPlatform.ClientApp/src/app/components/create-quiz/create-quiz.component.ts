import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router'
import { BehaviorSubject, catchError, filter, Observable, of, Subject, switchMap, tap } from 'rxjs';
import { AuthenticationService } from '../../services/authentication.service';
import { IQuizModel } from "../../types/quiz.classes.models";
import { QuizRepositoryService } from '../../services/quiz-repository.service'
import { QUIZ_GENRES } from '../../shared/quiz-genres.constant';

@Component({
  selector: 'app-create-quiz',
  templateUrl: './create-quiz.component.html',
  styleUrl: './create-quiz.component.scss'
})
export class CreateQuizComponent implements OnInit {

  private readonly errors = new BehaviorSubject<string>('');
  public readonly error$: Observable<string> = this.errors;

  public authenticated$ = this.auth.getIsAuthenticated();
  public anonymous$ = this.auth.getIsAnonymous();
  
  public quizForm: FormGroup;
  saving: boolean = false;
  quizGenres = QUIZ_GENRES;

  public constructor(private router: Router, private http: HttpClient, private auth: AuthenticationService, private quizRepository: QuizRepositoryService, private fb: FormBuilder) {
      this.quizForm = this.fb.group({
        Name: ['', Validators.required],
        Category: ['', Validators.required],
        QuizLogoUrl: ['', Validators.required]
      });
    }

  public ngOnInit(): void {
    this.authenticated$
      .pipe(
        filter(isAuthenticated => isAuthenticated),
        tap(() => {
          this.get();
        })
    ).subscribe();
  }

  get(){}
  
  cancel(): void {
    this.saving = false
    this.router.navigate(['/home'])
  }

  public addQuiz(quiz: IQuizModel): void {
    this.saving = true;
    this.quizRepository.createQuiz(quiz).subscribe({
      next: (response) => {
        console.log('Quiz created successfully:', response);
      },
      error: (error) => {
        this.saving = false
        console.error('Error creating quiz:', error);
      },
      complete: () => {
         this.router.navigate(['/home'])
      }
    });
  }
}
