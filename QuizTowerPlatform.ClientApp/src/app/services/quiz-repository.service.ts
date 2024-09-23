import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, EMPTY, throwError, timer, catchError, BehaviorSubject, filter, of, Subject, switchMap, tap } from 'rxjs';
import { IQuizModel, IAnswerModel } from '../types/quiz.classes.models';

@Injectable({
    providedIn: 'root',
})

export class QuizRepositoryService {
    private _quiz: IQuizModel | undefined = <IQuizModel>{};
    private readonly errors = new Subject<{ message: string, code?: number }>();

    constructor(private http: HttpClient) { }

    private readonly showError = (err: HttpErrorResponse) => {
        if (err.status !== 401) {
            this.errors.next({ message: err.message, code: err.status });
        }
        throw err;
    };

    public createQuiz(quiz: IQuizModel): Observable<IQuizModel> {
        return this.http
            .post<IQuizModel>('/api/Quiz/CreateQuiz', {
                Name: quiz.Name,
                Category: +quiz.Category,
                QuizLogoUrl: quiz.QuizLogoUrl,
            })
            .pipe(catchError(this.showError));
    }

    public getQuizById(id: number): Observable<IQuizModel> {
        return this.http
            .get<IQuizModel>(`/api/Quiz/GetQuiz/${id}`)
            .pipe(catchError(this.showError));
    }

    public startQuiz(quizId: number, answers: IAnswerModel[]): Observable<any> {
        return this.http
            .post<IQuizModel>('/api/Quiz/StartQuiz', {
                QuizId: quizId,
                Answers: answers
            })
            .pipe(catchError(this.showError));
    }
}