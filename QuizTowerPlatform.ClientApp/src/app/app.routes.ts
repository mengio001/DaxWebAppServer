import { ActivatedRouteSnapshot, RouterStateSnapshot, RouterModule, Routes } from '@angular/router';
import { inject } from '@angular/core';
import { AuthorizationService } from './services/authorization.service';

import { QuizComponent } from './components/quiz/quiz.component';
import { QuizzesListComponent } from './components/quizzes-list/quizzes-list.component';

export const routes: Routes = [
    { 
        path: '', component: QuizzesListComponent 
    },
    {
      path: 'quiz/:id',
      component: QuizComponent,
      canActivate: [
        (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) =>
          inject(AuthorizationService).canReadQuizDetails(),
      ],
    },
  ];
