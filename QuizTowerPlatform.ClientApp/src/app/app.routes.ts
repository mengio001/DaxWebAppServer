import { ActivatedRouteSnapshot, RouterStateSnapshot, Routes } from '@angular/router';
import { inject } from '@angular/core';
import { AuthorizationService } from './services/authorization.service';

import { HomeComponent } from './home/home.component';
import { QuizComponent } from './components/quiz/quiz.component';
import { UserSessionComponent } from './user-session/user-session.component';

export const routes: Routes = [
  { 
      path: '',
      redirectTo: '/home',
      pathMatch: 'full'
  },
  {
      path: 'home', 
      component: HomeComponent 
  },
  {
    path: 'user-session', 
    component: UserSessionComponent 
  },
  {
    path: 'quiz/:id',
    component: QuizComponent,
    canActivate: [
      (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) =>
        inject(AuthorizationService).canReadQuizDetails(),
    ],
  },
  {
    path: '**',
    redirectTo: 'home'
  }
];