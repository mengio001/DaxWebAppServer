import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router'
import { IQuizModel, IAnswerModel } from "../../types/quiz.classes.models";
import { QuizRepositoryService } from '../../services/quiz-repository.service'
import { BehaviorSubject, Observable } from 'rxjs';

@Component({
  selector: 'app-start-quiz',
  templateUrl: './start-quiz.component.html',
  styleUrl: './start-quiz.component.scss'
})

export class StartQuizComponent implements OnInit {
  public quiz: IQuizModel = <IQuizModel>{};
  selectedAnswers: IAnswerModel[] = [];
  isLoading: boolean = true;

  constructor(
    private route: ActivatedRoute,
    private service: QuizRepositoryService,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    const quizId = +(this.route.snapshot.paramMap.get('id') ?? -1);
    
    this.service.getQuizById(quizId).subscribe((quiz: IQuizModel) => {
      this.quiz = quiz;
      this.isLoading = false;
    });
  }

  onAnswerSelect(questionId: number, answer: string): void {
    const existingAnswer = this.selectedAnswers.find(a => a.QuestionId === questionId);
    if (existingAnswer) {
      existingAnswer.Answer = answer;
    } else {
      this.selectedAnswers.push({ QuestionId: questionId, Answer: answer });
    }
  }

  onSubmitQuiz(): void {
    this.service.startQuiz(this.quiz.Id, this.selectedAnswers).subscribe({
      next: (response) => {
        console.log('Quiz created successfully:', response);
      },
      error: (error) => {
        console.error('Error creating quiz:', error);
      },
      complete: () => {
         this.router.navigate(['/home'])
      }
    });
  }
}
