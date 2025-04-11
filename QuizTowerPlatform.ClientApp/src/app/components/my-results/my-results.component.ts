import { Component } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, filter, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { AuthenticationService, Session } from '../../services/authentication.service';
import { QuizRepositoryService } from '../../services/quiz-repository.service'
import { IMyResultsModel } from '../../types/achievement.classes.models';
import { MatDialog } from '@angular/material/dialog';
import { ShareDialogComponent } from '../share-dialog/share-dialog.component';

@Component({
  selector: 'app-my-results',
  templateUrl: './my-results.component.html',
  styleUrl: './my-results.component.scss'
})

export class MyResultsComponent {
  private readonly quizzes = new BehaviorSubject<IMyResultsModel[]>([]);
  public readonly quizzes$: Observable<IMyResultsModel[]> = this.quizzes;

  private readonly errors = new BehaviorSubject<string>('');
  public readonly error$: Observable<string> = this.errors;

  public session$: Observable<Session>;
  public isAuthenticated$: Observable<boolean>;
  public isAnonymous$: Observable<boolean>;
  public quizId: string = '';

  public constructor(readonly http: HttpClient, readonly auth: AuthenticationService, readonly router: Router, readonly service: QuizRepositoryService, readonly dialog: MatDialog) {
    this.session$ = auth.getSession();
    this.isAuthenticated$ = auth.getIsAuthenticated();
    this.isAnonymous$ = auth.getIsAnonymous();
  }

  public ngOnInit(): void {
    this.isAuthenticated$
      .pipe(
        filter(isAuthenticated => isAuthenticated),
        tap(() => {
          this.fetchMyResults();
        })
      ).subscribe();
  }

  private fetchMyResults(): void {
    this.service.getMyResults('Sony')
    .subscribe((response) => {
      this.quizzes.next(response);
    });
  }

  private readonly showError = (err: HttpErrorResponse) => {
    if (err.status !== 401) {
      this.errors.next(err.message);
    }
    throw err;
  }

  public getPercentage(quiz: IMyResultsModel): number {
    const total = quiz.UsersCorrectAnswers + quiz.UsersWrongAnswers;
    return total > 0 ? (100 / total) * quiz.UsersCorrectAnswers : 0;
  }

  public scoreLink = 'https://www.timelessmedia.nl/';

  openShareDialog(platform: string, contentLink: string): void {
    const shareUrls: any = {
      Facebook: `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(contentLink)}`,
      Twitter: `https://x.com/intent/post?url=${encodeURIComponent(contentLink)}&text=Check out my score!`,
      LinkedIn: `https://www.linkedin.com/sharing/share-offsite/?url=${encodeURIComponent(contentLink)}`,
    };

    if (platform === 'Copy') {
      navigator.clipboard.writeText(contentLink).then(() => {
        this.dialog.open(ShareDialogComponent, {
          data: {
            platformName: 'Clipboard',
            message: 'Link copied to clipboard!',
          },
        });
      }).catch(() => {
        this.dialog.open(ShareDialogComponent, {
          data: {
            platformName: 'Clipboard',
            message: 'Failed to copy the link. Please try again!',
          },
        });
      });
    } else {
      this.dialog.open(ShareDialogComponent, {
        width: '500px',
        data: { platform: platform, url: shareUrls[platform], message: '' },
      });
    }
  }
}